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

namespace server.Modules.Garage;

enum GARAGE_TYPES
{
  pkw = 1,
  lkw = 2
}

class GaragenModule : ILoadEvent, IPressedEEvent
{
  ServerContext _serverContext = new ServerContext();
  IVehicleHandler _vehicleHandler = new VehicleHandler();
  public static List<Models.Garage> garageList = new List<Models.Garage>();

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
      ped.CreateEntity();

      garageList.Add(garage);
    }
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    if (player.IsDead) return false;
    foreach (Models.Garage garage in garageList.ToList())
    {
      if (garage.Position.Distance(player.Position) < 2)
      {
        List<xVehicle> inVeh = await _vehicleHandler.GetVehiclesInRadius(garage.Position, 20);
        _logger.Log(inVeh.Count);
        List<Models.Vehicle> outVeh = await _vehicleHandler.GetVehiclesInGarage(garage.id);

        player.Emit("frontend:open", "garage", new garagenWriter(
          inVeh,
          outVeh,
          garage.name));
        return true;
      }
    }
    return false;
  }
}
