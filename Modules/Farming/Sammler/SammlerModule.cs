using server.Core;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;

namespace server.Modules.Farming.Sammler;
public class SammlerMain : ILoadEvent
{
  private readonly List<Models.sammler_farming_data> _sammler = new List<Models.sammler_farming_data>();

  public async void OnLoad()
  {
    await using ServerContext serverContext = new ServerContext();
    _logger.Startup("Lade Sammler!");
    foreach (Models.sammler_farming_data sammler in serverContext.sammler_farming_data)
    {
      _sammler.Add(sammler);
      _logger.Debug($"Sammler {sammler.name} geladen");
      sammler.ToString();
    }
    _logger.Startup($"x{_sammler.Count} Farming Sammler geladen");
  }
}