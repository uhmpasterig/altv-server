using server.Core;
using AltV.Net;
using server.Events;

using server.Models;
using AltV.Net.Async;
using server.Util.Garage;
using Microsoft.EntityFrameworkCore;


using server.Handlers.Vehicle;
using server.Handlers.Logger;

namespace server.Modules.Garage;

public class GaragenModule : ILoadEvent, IPressedEEvent
{
  ILogger _logger;
  IVehicleHandler _vehicleHandler;
  public GaragenModule(ILogger logger, IVehicleHandler vehicleHandler)
  {
    _logger = logger;
    _vehicleHandler = vehicleHandler;
  }

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
    ServerContext _serverContext = new ServerContext();
    garageList = _serverContext.Garages
      .Include(x => x.GarageSpawns)
      .ToList();

    foreach (Models.Garage garage in garageList)
    {
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
    }

    AltAsync.OnClient<xPlayer, int>("parkVehicle", async (player, vehicleId) =>
    {
      Models.Garage? garage = garageList.FirstOrDefault(x => x.Position.Distance(player.Position) < 30);
      Models.Vehicle vehicle = await _vehicleHandler.GetDbVehicle(vehicleId);
      if (vehicle == null) return;
      Models.GarageSpawn spawn = await GetFreeSpawn(garage!);
      if (spawn == null) return;
      await _vehicleHandler.CreateVehicleFromDb(vehicle, spawn.Position, spawn.Rotation);
    });

    AltAsync.OnClient<xPlayer, int, string, string, bool>("garageOverwriteVehicle", async (player, vehid, name, keyword, important) =>
    {
      Models.Vehicle? vehicle = await _vehicleHandler.GetDbVehicle(vehid);
      if (vehicle == null) return;
      Dictionary<int, Dictionary<string, object>>? data = vehicle.vehicle_data.UIData;

      if (!data.ContainsKey(player.id))
      {
        data.Add(player.id, new Dictionary<string, object>());
        Dictionary<string, object> playerData = data[player.id];
        playerData.Add("name", name);
        playerData.Add("keyword", keyword);
        playerData.Add("fav", important);
      }
      else
      {
        Dictionary<string, object> playerData = data[player.id];
        playerData["name"] = name;
        playerData["keyword"] = keyword;
        playerData["fav"] = important;
      };
      vehicle.vehicle_data.UIData = data;

      await _vehicleHandler.SaveDbVehicle();
    });
  }

  public async Task<Models.GarageSpawn> GetFreeSpawn(Models.Garage garage)
  {
    if (garage.GarageSpawns.Count == 0) return null;
    foreach (Models.GarageSpawn spawn in garage.GarageSpawns.ToList())
    {
      if (await _vehicleHandler.GetClosestVehicle(spawn.Position, 1) == null)
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
        List<Models.Vehicle> outVehs = await _vehicleHandler.GetVehiclesInGarage(player, garage.id);

        player.Emit("frontend:open", "garage", new garagenWriter(
          outVehs,
          garage.name,
          player));
        return true;
      }
    }
    return false;
  }
}
