using server.Core;
using server.Handlers.Player;
using AltV.Net.Data;
using AltV.Net.Enums;
using server.Events;
using server.Models;
using _logger = server.Logger.Logger;
using AltV.Net.Async;
using Microsoft.EntityFrameworkCore;
using server.Handlers.Storage;

namespace server.Handlers.Vehicle;

public enum OWNER_TYPES : int
{
  PLAYER,
  FACTION,
  BUSINESS
}

public enum VEHICLE_TYPES : int
{
  PKW,
  LKW,
  PLANE,
  BOAT
}

public class VehicleHandler : IVehicleHandler, ILoadEvent
{
  public static IVehicleHandler Instance = new VehicleHandler();
  
  public static ServerContext _vehicleCtx = new ServerContext();
  IStorageHandler _storageHandler = new StorageHandler();
  public static readonly Dictionary<int, xVehicle> Vehicles = new Dictionary<int, xVehicle>();

  public VehicleHandler() { }

  public async Task AddVehicleShopVehicle(xPlayer player, Vehicle_Shop_Vehicle _veh, Position position, Rotation rotation, int garageId = -1)
  {
    Models.Vehicle veh = new Models.Vehicle();
    veh.owner_id = player.id;
    veh.owner_type = (int)OWNER_TYPES.PLAYER;
    veh.type = (int)VEHICLE_TYPES.PKW;
    veh.garage_id = garageId;
    veh.model = _veh.model;

    veh.storage_id_glovebox = await _storageHandler.CreateStorage($"Handschuhfach [{veh.id}]", _veh.slots, _veh.maxWeight, new Position(0, 0, 0));
    veh.storage_id_trunk = await _storageHandler.CreateStorage($"Kofferraum [{veh.id}]", _veh.slots, _veh.maxWeight, new Position(0, 0, 0));

    veh.Position = position;
    veh.Rotation = rotation;

    veh.lastAction = DateTime.Now;
    veh.creationDate = DateTime.Now;

    veh.vehicle_data = new Vehicle_Data();
    veh.vehicle_data.plate = "";
    veh.vehicle_data._uidata = "{}";
    veh.vehicle_data.keys = "[]";
    veh.vehicle_keys = new List<Vehicle_Key>();
    _vehicleCtx.Vehicles.Add(veh);
    _vehicleCtx.SaveChanges();
  }

  public async Task<xVehicle> CreateVehicle(string model, Position position, Rotation rotation)
  {
    xVehicle vehicle = (xVehicle)await AltAsync.CreateVehicle(model, position, rotation);
    return vehicle;
  }

  public async Task<xVehicle> CreateVehicleFromDb(Models.Vehicle vehicle)
  {
    xVehicle xvehicle = await CreateVehicle(vehicle.model, vehicle.Position, vehicle.Rotation);
    return await SetVehicleData(xvehicle, vehicle);
  }

  public async Task<xVehicle> CreateVehicleFromDb(Models.Vehicle vehicle, Position position, Rotation rotation)
  {
    xVehicle xvehicle = await CreateVehicle(vehicle.model, position, rotation);
    return await SetVehicleData(xvehicle, vehicle);
  }

  public async Task SaveDbVehicleInGarage(xVehicle vehicle, int garage_id)
  {
    try
    {
      VehicleHandler.Vehicles.Remove(vehicle.id);
      var dbVehicle = _vehicleCtx.Vehicles.Find(vehicle.id);
      if (dbVehicle == null) return;
      dbVehicle.garage_id = garage_id;
      _vehicleCtx.SaveChanges();

      vehicle.Destroy();
      await _storageHandler.UnloadStorage(vehicle.storage_trunk);
      await _storageHandler.UnloadStorage(vehicle.storage_glovebox);
    }
    catch (Exception e)
    {
      _logger.Exception(e.Message);
    }
  }

  public async Task<xVehicle> SetVehicleData(xVehicle xvehicle, Models.Vehicle vehicle)
  {
    if (Vehicles.ContainsKey(vehicle.id)) return null!;
    Vehicles.Add(vehicle.id, xvehicle);
    xvehicle.SetDataFromDatabase(vehicle);

    xvehicle.PrimaryColorRgb = new Rgba((byte)vehicle.vehicle_data.r, (byte)vehicle.vehicle_data.g, (byte)vehicle.vehicle_data.b, 255);
    xvehicle.SecondaryColorRgb = new Rgba((byte)vehicle.vehicle_data.sr, (byte)vehicle.vehicle_data.sg, (byte)vehicle.vehicle_data.sb, 255);
    xvehicle.NumberplateText = vehicle.vehicle_data.plate;

    xvehicle.storage_glovebox = await _storageHandler.GetStorage(vehicle.storage_id_glovebox);
    xvehicle.storage_trunk = await _storageHandler.GetStorage(vehicle.storage_id_trunk);

    if (vehicle != null)
    {
      vehicle.Position = xvehicle.Position;
      vehicle.Rotation = xvehicle.Rotation;
      vehicle.garage_id = -1;
    }
    await _vehicleCtx.SaveChangesAsync();

    return xvehicle;
  }

  public async Task SaveVehicle(xVehicle xvehicle)
  {
    await using ServerContext serverContext = new ServerContext();
    Models.Vehicle? vehicle = await serverContext.Vehicles.FindAsync(xvehicle.id);
    if (vehicle != null)
    {
      vehicle.Position = xvehicle.Position;
      vehicle.Rotation = xvehicle.Rotation;
    }
    await serverContext.SaveChangesAsync();
  }

  public async Task SaveAllVehicles()
  {
    await using ServerContext serverContext = new ServerContext();
    _logger.Log($"Found {Vehicles.Count} vehicles in memory");
    foreach (var xvehicle in Vehicles.Values)
    {
      Models.Vehicle? vehicle = serverContext.Vehicles.Find(xvehicle.id);

      if (vehicle == null) continue;
      vehicle.Position = xvehicle.Position;
      vehicle.Rotation = xvehicle.Rotation;
    }
    await serverContext.SaveChangesAsync();
  }

  public bool SetModByType(xVehicle veh, VehicleModType modType, byte id)
  {
    veh.ModKit = 1;
    bool isModSet = veh.SetMod((byte)modType, id);
    return isModSet;
  }

  public async Task<xVehicle> GetClosestVehicle(Position position, int range = 2)
  {
    return Vehicles.Values.FirstOrDefault(v => v.Position.Distance(position) < range)!;
  }

  public async Task<List<xVehicle>> GetVehiclesInRadius(Position position, int range = 5)
  {
    List<xVehicle> vehicles = new List<xVehicle>();
    foreach (var vehicle in Vehicles.Values)
    {
      if (vehicle.Position.Distance(position) < range)
      {
        vehicles.Add(vehicle);
      }
    }
    return vehicles;
  }

  public async Task<Models.Vehicle> GetDbVehicle(int id)
  {
    return await _vehicleCtx.Vehicles.Include(v => v.vehicle_data).FirstAsync(v => v.id == id);
  }

  public async Task SaveDbVehicle()
  {
    await _vehicleCtx.SaveChangesAsync();
  }

  public async Task<xVehicle> GetVehicle(int id)
  {
    return Vehicles.Values.FirstOrDefault(v => v.id == id)!;
  }

  public async Task<List<Models.Vehicle>> GetVehiclesInGarage(xPlayer player, int garage_id)
  {

    List<Models.Vehicle> vehicles = await _vehicleCtx.Vehicles
      .Where(v => (v.garage_id == garage_id) &&
      ((v.owner_id == player.id && v.owner_type == (int)OWNER_TYPES.PLAYER) ||
      (v.owner_id == player.player_society.Faction.id && v.owner_type == (int)OWNER_TYPES.FACTION)))
      .Include(v => v.vehicle_data)
      .ToListAsync();

    List<Vehicle_Key> keyOwnedVehicles = await _vehicleCtx.Vehicle_Keys.Where(p =>
      (p.player_id == player.id) &&
      (p.Vehicle.garage_id == garage_id))
      .Include(v => v.Vehicle)
      .ThenInclude(v => v.vehicle_data)
      .ToListAsync();

    vehicles.AddRange(keyOwnedVehicles.Select(v => v.Vehicle));
    return vehicles;
  }

  public async void OnLoad()
  {
    foreach (Models.Vehicle vehicle in _vehicleCtx.Vehicles.Include(v => v.vehicle_data).Where(v => v.garage_id == -1).ToList())
    {
      await CreateVehicleFromDb(vehicle);
    }
  }
}
