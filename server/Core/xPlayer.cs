using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Async;
using server.Modules.Weapons;
using Newtonsoft.Json;
using _logger = server.Logger.Logger;
using AltV.Net.Resources.Chat.Api;

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
  public int id { get; set; }
  public string name { get; set; }

  public Dictionary<string, int> playerInventorys { get; set; }

  public List<xWeapon> weapons { get; set; }
  
  public string job { get; set; }
  public int job_rank { get; set; }
  public Dictionary<string, bool> job_perm { get; set; } 

  public DateTime creationDate { get; set; }
  public DateTime lastLogin { get; set; }
  public DIMENSIONEN dimensionType { get; set; }

  public int isDead { get; set; }

  public xPlayer(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id)
  {
    id = 0;
    name = "";
    playerInventorys = new Dictionary<string, int>();
    weapons = new List<xWeapon>();
    job = "";
    job_rank = 0;
    job_perm = new Dictionary<string, bool>();
    creationDate = DateTime.Now;
  }

  public void SendMessage(string message, NOTIFYS notifyType)
  {
    this.SendChatMessage(message);
  }

  public bool CanInteract()
  {
    if(this.isDead == 1)
    {
      this.SendMessage("Du kannst nichts machen, während du tot bist!", NOTIFYS.ERROR);
      return false;
    }

    return true;
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
    _logger.Log($"LoadWeaponsFromDb: {weapons}");
    this.weapons = JsonConvert.DeserializeObject<List<xWeapon>>(weapons)!;
    foreach(xWeapon weapon in this.weapons)
    {
      this.GiveWeapon(Alt.Hash(weapon.name), weapon.ammo, false);
    }
  }

  public void GiveSavedWeapon(string name, int ammo = 100, bool hold = false, string job = null!)
  {
    if(Server._serverWeapons.Find(x => x == name) == null)
    {
      this.SendMessage("Dieses Waffe existiert nicht!", NOTIFYS.ERROR);
      return;
    }

    if(weapons.Find(x => x.name == name.ToLower()) != null)
    {
      this.SendMessage("Du hast dieses Waffe bereits!", NOTIFYS.ERROR);
      return;
    }

    xWeapon weapon = new xWeapon(0, name, ammo, job);
    this.weapons.Add(weapon);
    this.GiveWeapon(Alt.Hash(name), ammo, hold);
  }

  public new IxPlayer ToAsync(IAsyncContext _) => this;
}

public partial interface IxPlayer : IPlayer, IAsyncConvertible<IxPlayer>
{
  int id { get; set; }
  string name { get; set; }
  DateTime creationDate { get; set; }
}