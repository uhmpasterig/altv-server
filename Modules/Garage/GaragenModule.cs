using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using AltV.Net.Async;
using server.Handlers.Entities;

namespace server.Modules.Garage;

enum GARAGE_TYPES
{
  pkw = 1,
  lkw = 2
}

class GaragenModule : ILoadEvent, IPressedEEvent
{
  public static List<Models.Garage> garageList = new List<Models.Garage>();

  public async void OnLoad()
  {
    ServerContext _serverContext = new ServerContext();
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

        return true;
      }
    }
    return false;
  }
}
