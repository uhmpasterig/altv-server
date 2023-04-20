using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using AltV.Net.Enums;
using server.Models;
using server.Handlers.Vehicle;
using _logger = server.Logger.Logger;

namespace server.Core;

public class xVehicle : AsyncVehicle, IxVehicle
{
  public xVehicle(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id)
  {
  }

  public int vehicleId { get; set; }
  public int ownerId { get; set; }
  public int garageId { get; set; }

  public string model { get; set; }
  public string name { get; set; }
  public string keyword { get; set; }
  public string licensePlate { get; set; }

  public int storageIdTrunk { get; set; } = 0;
  public int storageIdGloveBox { get; set; } = 0;
  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }

  public bool isAccesable { get; set; } = true;

  // States
  private bool _isLocked = false;
  private bool _isTrunkOpen = false;
  private bool _isEngineRunning = false;

  public bool Locked
  {
    get => _isLocked;
    set
    {
      this.LockState = value ? VehicleLockState.Locked : VehicleLockState.Unlocked;
      _isLocked = value;
    }
  }

  public bool Trunk
  {
    get => _isTrunkOpen;
    set
    {
      this.SetDoorState((byte)VehicleDoor.Trunk, value ? (byte)VehicleDoorState.OpenedLevel7 : (byte)VehicleDoorState.Closed);
      _isTrunkOpen = value;
    }
  }

  public bool Engine
  {
    get => _isEngineRunning;
    set
    {
      this.EngineOn = value;
      _isEngineRunning = value;
    }
  }

  public bool canTrunkBeOpened()
  {
    return isAccesable && !Locked && !Engine && Trunk;
  }

  public bool hasControl(xPlayer player)
  {
    return player.Id == this.ownerId;
  }

  public void storeInGarage(int gid)
  {
    //Todo : Unload Inventory
    try
    {
      VehicleHandler.Vehicles.Remove(this.vehicleId);
      ServerContext _serverContext = new ServerContext();
      var svehicle = _serverContext.Vehicle.Find(this.vehicleId);
      if (svehicle == null) return;
      svehicle.garageId = gid;
      _serverContext.SaveChanges();
      this.Destroy();
    }
    catch (Exception e)
    {
      _logger.Exception(e.Message);
    }
  }

  public new IxVehicle ToAsync(IAsyncContext _) => this;
}

public partial interface IxVehicle : IVehicle, IAsyncConvertible<IxVehicle>
{
  int vehicleId { get; set; }
  public int ownerId { get; set; }
  public int garageId { get; set; }
  public string model { get; set; }
  public string name { get; set; }
  public string keyword { get; set; }
  public string licensePlate { get; set; }
  public int storageIdTrunk { get; set; }
  public int storageIdGloveBox { get; set; }
  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }
}