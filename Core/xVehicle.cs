using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using server.Models;
using server.Handlers.Vehicle;

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

  public string model { get; set; }
  public string licensePlate { get; set; }

  public int storageIdTrunk { get; set; } = 0;
  public int storageIdGloveBox { get; set; } = 0;
  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }
  public string toString { get; }

  public bool isAccesable { get; set; } = true;
  public bool isLocked { get; set; } = false;
  public bool isEngineRunning { get; set; } = false;

  public bool canTrunkBeOpened()
  {
    return isAccesable && !isLocked;
  }

  public void storeInGarage(int gid)
  {
    ServerContext _serverContext = new ServerContext();
    var svehicle = _serverContext.Vehicle.Find(this.vehicleId);
    if (svehicle == null) return;
    svehicle.garageId = gid;
    _serverContext.SaveChanges();
    this.Destroy();
    //Todo : Unload Inventory
    VehicleHandler.Vehicles.Remove(this.vehicleId);
  }

  public new IxVehicle ToAsync(IAsyncContext _) => this;
}

public partial interface IxVehicle : IVehicle, IAsyncConvertible<IxVehicle>
{
  int vehicleId { get; set; }
  public int ownerId { get; set; }
  public int garageId { get; set; }
  public string model { get; set; }
  public string licensePlate { get; set; }
  public int storageIdTrunk { get; set; }
  public int storageIdGloveBox { get; set; }
  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }
  public string toString { get { return "OwnerID: " + ownerId; } }
}