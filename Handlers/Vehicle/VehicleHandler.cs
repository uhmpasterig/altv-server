using server.Core;
using server.Handlers.Player;
using AltV.Net.Data;
using AltV.Net.Enums;
using server.Events;
using server.Models;
using _logger = server.Logger.Logger;
using AltV.Net.Async;

namespace server.Handlers.Vehicle;
public class VehicleHandler : IVehicleHandler, ILoadEvent
{
  public static readonly Dictionary<int, xVehicle> Vehicles = new Dictionary<int, xVehicle>();

  public VehicleHandler() { }

  public async Task<xVehicle> CreateVehicle(string model, Position position, Rotation rotation)
  {
    xVehicle vehicle = (xVehicle)await AltAsync.CreateVehicle(model, position, rotation);
    return vehicle;
  }

  public async Task<xVehicle> CreateVehicleFromDb(Models.Vehicle vehicle)
  {
    xVehicle xvehicle = await CreateVehicle(vehicle.model, vehicle.Position, vehicle.Rotation);
    _logger.Debug($"Created vehicle with model {vehicle.model} at {vehicle.Position}");
    return await SetVehicleData(xvehicle, vehicle);
  }

  public async Task<xVehicle> SetVehicleData(xVehicle xvehicle, Models.Vehicle vehicle)
  {
    if (Vehicles.ContainsKey(vehicle.id)) return null!;
    Vehicles.Add(vehicle.id, xvehicle);
    xvehicle.vehicleId = vehicle.id;

    xvehicle.ownerId = vehicle.ownerId;
    xvehicle.storageIdGloveBox = vehicle.storageIdGloveBox;
    xvehicle.storageIdTrunk = vehicle.storageIdTrunk;

    xvehicle.model = vehicle.model;

    xvehicle.PrimaryColor = (byte)vehicle.color;
    xvehicle.SecondaryColor = (byte)vehicle.color2;
    await xvehicle.SetNumberplateTextAsync(vehicle.plate);

    return xvehicle;
  }

  public async Task SaveVehicle(xVehicle xvehicle)
  {
    await using ServerContext serverContext = new ServerContext();
    Models.Vehicle vehicle = await serverContext.Vehicle.FindAsync(xvehicle.vehicleId);
    if (vehicle != null)
    {
      vehicle.Position = xvehicle.Position;
      vehicle.Rotation = xvehicle.Rotation;
    }
    else
    {
      _logger.Error($"Vehicle with id {xvehicle.vehicleId} not found in database");
    }
    await serverContext.SaveChangesAsync();
  }

  public async Task SaveAllVehicles()
  {
    await using ServerContext serverContext = new ServerContext();
    _logger.Log($"Found {Vehicles.Count} vehicles in memory");
    foreach (var vehicle in Vehicles.Values)
    {
      Models.Vehicle dbVehicle = serverContext.Vehicle.Find(vehicle.vehicleId);

      if (dbVehicle == null) continue;
      dbVehicle.Position = vehicle.Position;
      dbVehicle.Rotation = vehicle.Rotation;
    }
    await serverContext.SaveChangesAsync();
  }

  [Obsolete]
  public bool SetModByType(xVehicle veh, VehicleModType modType, byte id)
  {
    veh.ModKit = 1;
    bool isModSet = veh.SetMod((byte)modType, id);
    return isModSet;
  }

  public xVehicle GetClosestVehicle(Position position, int range = 2)
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

  public xVehicle GetVehicle(int id)
  {
    return Vehicles.Values.FirstOrDefault(v => v.vehicleId == id)!;
  }

  public async Task<List<Models.Vehicle>> GetVehiclesInGarage(int garageId)
  {
    await using ServerContext serverContext = new ServerContext();
    List<Models.Vehicle> vehicles = serverContext.Vehicle.Where(v => v.garageId == garageId).ToList();
    return vehicles;
  }

  public async void OnLoad()
  {
    await using ServerContext serverContext = new ServerContext();
    foreach (Models.Vehicle vehicle in serverContext.Vehicle.Where(v => v.garageId == -1))
    {
      _logger.Debug($"Loading vehicle with id {vehicle.id} from database");
      await CreateVehicleFromDb(vehicle);
    }
  }
}
