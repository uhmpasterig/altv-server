using server.Core;
using server.Events;
using server.Handlers.Event;
using server.Handlers.Entities;
using server.Models;
using _logger = server.Logger.Logger;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Data;

namespace server.Modules.Farming.Sammler;
public class SammlerMain : ILoadEvent, IPressedEEvent
{
  private readonly List<sammler_farming_data> _sammler = new List<sammler_farming_data>();

  public async void LoadSammler(sammler_farming_data sammlerData)
  {
    foreach(Position _pos in sammlerData.PropPositions)
    {
      xEntity _entity = new xEntity();
      _entity.entityType = ENTITY_TYPES.PROP;
      _entity.dimension = (int)DIMENSIONEN.WORLD;
      _entity.position = _pos;
      _entity.range = 20;
      _entity.data.Add("model", sammlerData.prop);
      _entity.CreateEntity();
      // _entity.SetSyncedData("sideProducts", sammlerData.sideProducts);
      sammlerData.Entities.Add(_entity);

      _logger.Debug("Entity created");
    }
  }

  public async Task<bool> OnKeyPressE(xPlayer player) 
  {
    sammler_farming_data _currentSammler = null!;
    // Get the Closest Sammler
    _sammler.ForEach((sammler) =>
    {
      if (sammler.Position.Distance(player.Position) < 80) {
        _currentSammler = sammler;
      }
    });
    if (_currentSammler == null) return false;

    // Get the Closest Prop of the closest Farming field
    xEntity _currentEntity = null!;
    _currentSammler.Entities.ForEach((entity) =>
    {
      if (entity.position.Distance(player.Position) < 2) {
        _currentEntity = entity;
      }
    });
    if (_currentEntity == null) return false;
    _logger.Debug("Entity found");
    player.Emit("pointAtCoords", _currentEntity.entity.Position.X, _currentEntity.entity.Position.Y, _currentEntity.entity.Position.Z);
    return true;
  }

  public async void OnLoad()
  {
    await using ServerContext serverContext = new ServerContext();
    _logger.Startup("Lade Sammler!");
    foreach (sammler_farming_data sammler in serverContext.sammler_farming_data)
    {
      _sammler.Add(sammler);
      LoadSammler(sammler);
      _logger.Debug($"Sammler {sammler.name} geladen");
    }
    _logger.Startup($"x{_sammler.Count} Farming Sammler geladen");
  }
}