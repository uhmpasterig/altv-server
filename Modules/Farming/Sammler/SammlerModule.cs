using server.Core;
using server.Events;
using server.Handlers.Event;
using server.Handlers.Entities;
using server.Models;
using _logger = server.Logger.Logger;
using AltV.Net.Async;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;

namespace server.Modules.Farming.Sammler;
public class SammlerMain : ILoadEvent
{
  private readonly List<Models.sammler_farming_data> _sammler = new List<Models.sammler_farming_data>();

  public async void LoadSammler(Models.sammler_farming_data sammlerData)
  {
    foreach(Position _pos in sammlerData.PropPositions)
    {
      xEntity _entity = new xEntity();
      _entity.entityType = ENTITY_TYPES.PROP;
      _entity.dimension = (int)DIMENSIONEN.WORLD;
      _entity.position = _pos;
      _entity.range = 80;
      _entity.data.Add("model", sammlerData.prop);
      _entity.CreateEntity();
      // _entity.SetSyncedData("sideProducts", sammlerData.sideProducts);
      EntityHandler.Entities.Add(_entity.entity);
      _logger.Debug("Entity created");
    }
  }

  public async void OnLoad()
  {
    await using ServerContext serverContext = new ServerContext();
    _logger.Startup("Lade Sammler!");
    foreach (Models.sammler_farming_data sammler in serverContext.sammler_farming_data)
    {
      _sammler.Add(sammler);
      LoadSammler(sammler);
      _logger.Debug($"Sammler {sammler.name} geladen");
    }
    _logger.Startup($"x{_sammler.Count} Farming Sammler geladen");
  }
}