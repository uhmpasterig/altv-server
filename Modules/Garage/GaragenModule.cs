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
    ServerContext _serverContext = new ServerContext();
    foreach(Models.Garage garage in _serverContext.Garage.ToList())
    {
      garage.ToString();
      garageList.Add(garage);
    }
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    return false;
  }
}
