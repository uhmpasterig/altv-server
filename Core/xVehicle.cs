using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using AltV.Net.Enums;
using server.Models;
using server.Handlers.Vehicle;
using _logger = server.Logger.Logger;
using server.Handlers.Storage;

namespace server.Core;

public class xVehicle : AsyncVehicle
{
  public static ServerContext _serverContext = new ServerContext();
  public static IStorageHandler _storageHandler = new StorageHandler();
  public xVehicle(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id)
  {
  }

  public int id { get; set; }
  public int owner_id { get; set; }
  public int owner_type { get; set; }
  public int type { get; set; }
  public int garage_id { get; set; }
  public string model { get; set; }

  public int storage_id_trunk { get; set; } = 0;
  public int storage_id_glovebox { get; set; } = 0;

  public xStorage? storage_trunk { get; set; }
  public xStorage? storage_glovebox { get; set; }

  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }

  public double mileage { get; set; } = 0;


  public bool isAccesable { get; set; } = true;

  public void SetDataFromDatabase(Models.Vehicle vehicle)
  {
    this.id = vehicle.id;
    this.owner_id = vehicle.owner_id;
    this.owner_type = vehicle.owner_type;
    this.type = vehicle.type;
    this.garage_id = vehicle.garage_id;
    this.model = vehicle.model;
    this.storage_id_trunk = vehicle.storage_id_trunk;
    this.storage_id_glovebox = vehicle.storage_id_glovebox;
    this.lastAction = vehicle.lastAction;
    this.creationDate = vehicle.creationDate;
    this.mileage = vehicle.vehicle_data.mileage;
  }

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
    bool canUse = true;
    if (this.storage_trunk == null)
    {
      canUse = false;
    }
    canUse = isAccesable && !Locked && !Engine && Trunk;
    return canUse;
  }

  public bool hasControl(xPlayer player)
  {
    _logger.Log("Vehicle OwnerId: " + this.owner_id + " | PlayerId: " + player.id);
    return player.id == this.owner_id;
  }
}