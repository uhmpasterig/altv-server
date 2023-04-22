using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using AltV.Net.Async;
using server.Handlers.Entities;
using server.Handlers.Vehicle;
using server.Util.Garage;
using server.Modules.Blip;

namespace server.Modules.Garage;

enum GARAGE_TYPES
{
  PKW = 1,
  LKW = 2
}

enum GARAGE_SPRITES : int
{
  PKW = 357,
  LKW = 357
}

enum GARAGE_COLORS : int
{
  PKW = 3,
  LKW = 81
}

class GARAGE_NAMES
{
  static Dictionary<string, string> _names = new Dictionary<string, string>()
  {
    { "PKW", "PKW-Garage" },
    { "LKW", "LKW-Garage" }
  };

  public static string GetName(string name)
  {
    return _names[name];
  }
}

class GaragenModule : ILoadEvent, IPressedEEvent
{
  ServerContext _serverContext = new ServerContext();
  IVehicleHandler _vehicleHandler = new VehicleHandler();
  public static List<Models.Garage> garageList = new List<Models.Garage>();

  public static Dictionary<string, int> GetGarageBlipByType(int type)
  {
    string typeName = Enum.GetName(typeof(GARAGE_TYPES), type)!;

    Dictionary<string, int> dict = new Dictionary<string, int>();
    dict.Add("sprite", (int)Enum.Parse(typeof(GARAGE_SPRITES), typeName));
    dict.Add("color", (int)Enum.Parse(typeof(GARAGE_COLORS), typeName));
    return dict;
  }

  public async void OnLoad()
  {
    foreach (Models.Garage garage in _serverContext.Garage.ToList())
    {
      foreach (Models.GarageSpawns spawn in _serverContext.GarageSpawns.Where(x => x.garage_id == garage.id).ToList())
      {
        garage.garageSpawns.Add(spawn);
      }

      xEntity ped = new xEntity();
      ped.position = garage.Position;
      ped.dimension = (int)DIMENSIONEN.WORLD;
      ped.entityType = ENTITY_TYPES.PED;
      ped.range = 100;
      ped.data.Add("model", garage.ped);
      ped.data.Add("heading", garage.heading);
      ped.CreateEntity();

      Dictionary<string, int> blip = GetGarageBlipByType(garage.type);
      Blip.Blip.Create(GARAGE_NAMES.GetName(Enum.GetName(typeof(GARAGE_TYPES), garage.type)!),
        blip["sprite"], blip["color"], .75f, garage.Position);

      garageList.Add(garage);
    }

    AltAsync.OnClient<xPlayer, int>("parkVehicle", async (player, vehicleId) =>
    {
      Models.Garage? garage = garageList.FirstOrDefault(x => x.Position.Distance(player.Position) < 30);
      Models.Vehicle? vehicle = _serverContext.Vehicle.FirstOrDefault(x => x.id == vehicleId);
      if (vehicle == null) return;
      Models.GarageSpawns spawn = await GetFreeSpawn(garage!);
      if (spawn == null) return;
      await _vehicleHandler.CreateVehicleFromDb(vehicle, spawn.Position, spawn.Rotation);
      // if (type == "einparken")
      //   xVehicle vehicle = _vehicleHandler.GetVehicle(vehicleId);
      //   if (vehicle == null) return;
      //   vehicle.storeInGarage(garage.id);
    });

    AltAsync.OnClient<xPlayer, int, string, string, bool>("garageOverwriteVehicle", async (player, vehid, name, keyword, important) =>
    {
      Models.Vehicle? vehicle = _serverContext.Vehicle.FirstOrDefault(x => x.id == vehid);
      if (vehicle == null) return;
      Dictionary<xPlayer, Dictionary<string, object>>? data = vehicle.UIData;
      if (!data.ContainsKey(player)) {
        data.Add(player, new Dictionary<string, object>());
        _logger.Warning("add player to vehicle");
      };
      _logger.Warning("overwrite vehicle");
      vehicle.UIData = data;
      await _serverContext.SaveChangesAsync();
    });
  }

  public async Task<Models.GarageSpawns> GetFreeSpawn(Models.Garage garage)
  {
    if (garage.garageSpawns.Count == 0) return null;
    foreach (Models.GarageSpawns spawn in garage.garageSpawns.ToList())
    {
      if (_vehicleHandler.GetClosestVehicle(spawn.Position, 1) == null)
        return spawn;
    }
    return null;
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    if (player.IsDead) return false;
    foreach (Models.Garage garage in garageList.ToList())
    {
      if (garage.Position.Distance(player.Position) < 2)
      {
        // List<xVehicle> inVeh = await _vehicleHandler.GetVehiclesInRadius(garage.Position, 30);
        List<Models.Vehicle> outVeh = await _vehicleHandler.GetVehiclesInGarage(garage.id);

        player.Emit("frontend:open", "garage", new garagenWriter(
          outVeh,
          garage.name,
          player));
        return true;
      }
    }
    return false;
  }
}
