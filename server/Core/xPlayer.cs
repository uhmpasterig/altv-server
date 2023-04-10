using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Async;
using server.Modules.Weapons;
using Newtonsoft.Json;

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
  
  public DateTime creationDate { get; set; }
  public DateTime lastLogin { get; set; }
  public DIMENSIONEN dimensionType { get; set; }

  public xPlayer(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id)
  {
    id = 0;
    name = "";
    creationDate = DateTime.Now;
  }

  public void SendMessage(string message, NOTIFYS notifyType)
  {
    // this.Emit("client:notify", message, notifyType);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[ CLIENT NOTIFY ({notifyType}) ] {message}");
    Console.ResetColor();
  }

  public bool CanInteract()
  {
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
    this.weapons = JsonConvert.DeserializeObject<List<xWeapon>>(weapons)!;
    foreach(xWeapon weapon in this.weapons)
    {
      this.GiveWeapon(Alt.Hash(weapon.name), weapon.ammo, false);
    }
  }

  public void GiveSavedWeapon(string name, int ammo = 100, string job = null!)
  {
    xWeapon weapon = new xWeapon(0, name, ammo, job);
    this.weapons.Add(weapon);
    this.GiveWeapon(Alt.Hash(name), ammo, false);
  }

  public new IxPlayer ToAsync(IAsyncContext _) => this;
}

public partial interface IxPlayer : IPlayer, IAsyncConvertible<IxPlayer>
{
  int id { get; set; }
  string name { get; set; }
  DateTime creationDate { get; set; }
}