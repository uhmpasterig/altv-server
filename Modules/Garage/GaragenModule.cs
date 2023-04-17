using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;

namespace server.Modules.Garage;
class GaragenModule : ILoadEvent, IPressedEEvent
{
  public async void OnLoad()
  {
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    return false;
  }
}
