using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Async;
using server.Modules.Weapons;
using Newtonsoft.Json;
using _logger = server.Logger.Logger;
using AltV.Net.Resources.Chat.Api;
using AltV.Net.Data;
using server.Modules.Clothing;
using server.Handlers.Storage;
using server.Models;

namespace server.Core;

#region enums
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
#endregion

public partial class xPlayer : AsyncPlayer, IxPlayer
{
  ServerContext _serverContext = new ServerContext();

  private string[] _notifys = new string[] { "default", "error", "success", "warning" };

  public int id { get; set; }
  public string name { get; set; }
  public string ped { get; set; }
  public int cash { get; set; }
  public int bank { get; set; }

  public Dictionary<string, int> boundStorages { get; set; } = new Dictionary<string, int>();
  public List<xWeapon> weapons { get; set; } = new List<xWeapon>();

  public string job { get; set; }
  public int job_rank { get; set; }
  public List<string> job_perm { get; set; } = new List<string>();

  public DateTime creationDate { get; set; }
  public DateTime lastLogin { get; set; }

  public Dictionary<string, object> dataCache { get; set; } = new Dictionary<string, object>();

  public Player_Skin player_skin { get; set; }
  public Player_Cloth player_cloth { get; set; }

  public int isDead { get; set; }

  public xPlayer(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id)
  {
    dataCache = new Dictionary<string, object>();
  }

  public async Task SetDataFromDatabase(Models.Player _player)
  {
    this.id = _player.id;
    this.name = _player.name;
    this.ped = _player.ped;

    this.cash = _player.cash;
    this.bank = _player.bank;
    this.boundStorages = _player.boundStorages;
    this.weapons = _player.weapons;
    this.job = _player.job;
    this.job_rank = _player.job_rank;
    this.job_perm = _player.job_perm;
    this.dataCache = _player.dataCache;

    this.creationDate = _player.creationDate;
    this.lastLogin = _player.lastLogin;
    this.dataCache = _player.dataCache;

    this.player_skin = _player.player_skin;
    this.player_cloth = _player.player_cloth;
  }

  #region Methods
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
    IStorageHandler _storageHandler = new StorageHandler();
    xStorage inv = await _storageHandler.GetStorage(this.boundStorages["Inventar"]);
    if (inv == null)
    {
      this.SendMessage("Du hast kein Inventar!", NOTIFYS.ERROR);
      return false;
    }
    return inv.AddItem(name, count);
  }

  public void SetPlayerInventoryId(string key, int value)
  {
    if (boundStorages.ContainsKey(key))
    {
      boundStorages[key] = value;
    }
    else
    {
      boundStorages.Add(key, value);
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
    xStorage inv = await _storageHandler.GetStorage(this.boundStorages["Inventar"]);
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

  // SKIN
  public async Task LoadSkin(Player_Skin? skin = null)
  {
    _logger.Info($"Loading skin for {this.name}");
    if (skin == null) skin = this.player_skin;
    this.SetHeadBlendData(
            skin.shape1,
            skin.shape2,
            0,
            skin.skin1,
            skin.skin2,
            0,
            skin.shapeMix,
            skin.skinMix,
            0);

    this.SetEyeColor(skin.eyeColor);
    this.HairColor = skin.hairColor;
    this.HairHighlightColor = skin.hairColor2;
    this.SetClothes(2, skin.hair, skin.hair2, 0);

    _logger.Info($"Loaded skin for {this.name}");
  }

  public async Task SetClothPiece(int id)
  {
    _logger.Info($"Setting cloth with id {id} for {this.name}");
    Models.Cloth? cloth = ClothModule.GetCloth(id);
    if (cloth == null)
    {
      _logger.Error($"Cloth with id {id} not found!");
      return;
    }
    this.SetDlcClothes(cloth.component, cloth.drawable, cloth.texture, cloth.palette, cloth.dlc);
    _logger.Info($"Set cloth with id {id} for {this.name}");
  }

  public async Task LoadClothes(Player_Cloth? cloth = null)
  {
    _logger.Info($"Loading clothes for {this.name}");
    if (cloth == null) cloth = this.player_cloth;
    foreach(int id in cloth.ToList())
    {
      await this.SetClothPiece(id);
    }
    _logger.Info($"Loaded clothes for {this.name}");
  }

  #endregion
  public new IxPlayer ToAsync(IAsyncContext _) => this;
}

public partial interface IxPlayer : IPlayer, IAsyncConvertible<IxPlayer>
{
  int id { get; set; }
  string name { get; set; }
  DateTime creationDate { get; set; }
}