using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;

namespace server.Core;

public class xVehicle : AsyncVehicle, IxVehicle
{
  public xVehicle(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id)
  {
    toString = "OwnerID: " + ownerId;
  }

  public int vehicleId { get; set; }
  public int ownerId { get; set; }
  public int garageId { get; set; }
  public int storageIdTrunk { get; set; } = 0;
  public int storageIdGloveBox { get; set; } = 0;
  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }
  public string toString { get; }

  public bool isAccesable { get; set; } = true;
  public bool isLocked { get; set; } = true;
  public bool isEngineRunning { get; set; } = false;  

  public bool canTrunkBeOpened()
  {
    return isAccesable && !isLocked;    
  }


  public new IxVehicle ToAsync(IAsyncContext _) => this;
}

public partial interface IxVehicle : IVehicle, IAsyncConvertible<IxVehicle>
{
  public int ownerId { get; set; }
  public int garageId { get; set; }
  public int storageIdTrunk { get; set; }
  public int storageIdGloveBox { get; set; }
  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }
  public string toString { get { return "OwnerID: "+ownerId; } }
}