using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Async;


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

  public new IxPlayer ToAsync(IAsyncContext _) => this;
}

public partial interface IxPlayer : IPlayer, IAsyncConvertible<IxPlayer>
{
  int id { get; set; }
  string name { get; set; }
  DateTime creationDate { get; set; }
}