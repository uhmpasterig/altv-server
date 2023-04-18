using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;

namespace server.Modules.Garage;
class GaragenModule : ILoadEvent, IPressedEEvent
{
  public static List<Models.Garage> garageList = new List<Models.Garage>();

  public async void OnLoad()
  {
    _logger.Warning("GaragenModule geladen");
    ServerContext _serverContext = new ServerContext();
    foreach (Models.Garage garage in _serverContext.Garage.ToList())
    {
      _logger.Warning("GaragenModule geladen1");
      foreach (Models.GarageSpawns spawn in _serverContext.GarageSpawns.Where(x => x.garage_id == garage.id).ToList())
      {
        _logger.Warning("GaragenSpawn geladen2");
        garage.garageSpawns.Add(spawn);
      }
      garageList.Add(garage);
    }
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    return false;
  }
}
