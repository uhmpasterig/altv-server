using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Async;
using server.Modules.Weapons;
using Newtonsoft.Json;
using _logger = server.Logger.Logger;
using AltV.Net.Resources.Chat.Api;
using AltV.Net.Data;
using server.Handlers.Storage;
using server.Models;

namespace server.Core;

public enum DIMENSIONEN
{
  WORLD,
  HOUSE,
  CAMPER,
  STORAGEROOM,
  PVP
}

public enum NOTIFYS
{
  INFO,
  ERROR,
  SUCCESS,
  WARNING
}

public partial class xPlayer : AsyncPlayer, IxPlayer
{
  ServerContext _serverContext = new ServerContext();

  private string[] _notifys = new string[] { "default", "error", "success", "warning" };

  public int id { get; set; }
  public string name { get; set; }

  public int cash { get; set; }
  public int bank { get; set; }

  public Dictionary<string, int> playerInventorys { get; set; }

  public List<xWeapon> weapons { get; set; }

  public string job { get; set; }
  public int job_rank { get; set; }
  public List<string> job_perm { get; set; }

  public DateTime creationDate { get; set; }
  public DateTime lastLogin { get; set; }
  public DIMENSIONEN dimensionType { get; set; }

  public Dictionary<string, object> dataCache { get; set; }

  public int isDead { get; set; }

  public xPlayer(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id)
  {
    id = 0;
    name = "";
    playerInventorys = new Dictionary<string, int>();
    weapons = new List<xWeapon>();
    job = "";
    job_rank = 0;
    job_perm = new List<string>();
    lastLogin = DateTime.Now;

    dataCache = new Dictionary<string, object>();
  }

  public void SendMessage(string message, NOTIFYS notifyType)
  {
    this.SendChatMessage(message);
  }

  public void SendMessage(string title, string text, int time, NOTIFYS notifyType)
  {
    this.Emit("clientNotify", title, text, time, _notifys[(int)notifyType]);
  }

  public bool CanInteract()
  {
    if (this.isDead == 1)
    {
      this.SendMessage("Du kannst nichts machen, während du tot bist!", NOTIFYS.ERROR);
      return false;
    }

    return true;
  }

  public async Task<bool> GiveItem(string name, int count)
  {
    IStorageHandler storageHandler = new StorageHandler();
    xStorage inv = await storageHandler.GetStorage(this.playerInventorys["Inventar"]);
    if (inv == null)
    {
      this.SendMessage("Du hast kein Inventar!", NOTIFYS.ERROR);
      return false;
    }
    return inv.AddItem(name, count);
  }

  public void SetPlayerInventoryId(string key, int value)
  {
    if (playerInventorys.ContainsKey(key))
    {
      playerInventorys[key] = value;
    }
    else
    {
      playerInventorys.Add(key, value);
    }
  }

  public void LoadWeaponsFromDb(string weapons)
  {
    this.weapons = JsonConvert.DeserializeObject<List<xWeapon>>(weapons)!;
    foreach (xWeapon weapon in this.weapons)
    {
      this.GiveWeapon(Alt.Hash(weapon.name), weapon.ammo, false);
    }
  }

  public Task<bool> GiveSavedWeapon(string name, int ammo = 100, bool hold = false, string job = null!)
  {
    if (Server._serverWeapons.Find(x => x == name) == null)
    {
      this.SendMessage("Dieses Waffe existiert nicht!", NOTIFYS.ERROR);
      return Task.FromResult(false);
    }

    if (weapons.Find(x => x.name == name.ToLower()) != null)
    {
      this.SendMessage("Du hast dieses Waffe bereits!", NOTIFYS.ERROR);
      return Task.FromResult(false);
    }

    xWeapon weapon = new xWeapon(0, name, ammo, job);
    this.weapons.Add(weapon);
    this.GiveWeapon(Alt.Hash(name), ammo, hold);
    return Task.FromResult(true);
  }

  public void SetDead(int isDead)
  {
    this.isDead = isDead;
    this.Emit("player:dead", isDead);
  }

  public void Revive()
  {
    this.SetDead(0);
    this.ClearBloodDamage();
    this.Spawn(this.Position, 0);
    this.Health = this.MaxHealth;
  }

  public async Task<bool> HasItem(string name, int count = 1)
  {
    IStorageHandler _storageHandler = new StorageHandler();
    xStorage inv = await _storageHandler.GetStorage(this.playerInventorys["Inventar"]);
    return inv.HasItem(name, count);
  }

  public async void GiveMoney(int amount)
  {
    this.cash += amount;
  }

  public async void RemoveMoney(int amount)
  {
    this.cash -= amount;
  }

  public async Task<bool> HasMoney(int amount)
  {
    return this.cash >= amount;
  }

  public async void SaveMoney()
  {
    Models.Player? player = await _serverContext.Players.FindAsync(this.id);
    player.cash = this.cash;
    player.bank = this.bank;
    await _serverContext.SaveChangesAsync();
  }


  public new IxPlayer ToAsync(IAsyncContext _) => this;
}

public partial interface IxPlayer : IPlayer, IAsyncConvertible<IxPlayer>
{
  int id { get; set; }
  string name { get; set; }
  DateTime creationDate { get; set; }
}