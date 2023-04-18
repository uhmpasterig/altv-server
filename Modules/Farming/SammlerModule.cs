using server.Core;
using server.Events;
using server.Handlers.Event;
using server.Handlers.Entities;
using server.Models;
using _logger = server.Logger.Logger;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Data;
using server.Handlers.Storage;
using Newtonsoft.Json;

namespace server.Modules.Farming.Sammler;
public class SammlerMain : ILoadEvent, IPressedEEvent, IFiveSecondsUpdateEvent
{
  private List<sammler_farming_data> _sammler = new List<sammler_farming_data>();
  private Dictionary<xPlayer, int> _farmingPlayers;

  public async void LoadSammler(sammler_farming_data sammlerData)
  {
    sammlerData.PropPositions = JsonConvert.DeserializeObject<List<propData>>(sammlerData._propPositions)!;
    
    foreach (propData prop in sammlerData.PropPositions)
    {
      xEntity _entity = new xEntity();
      _entity.entityType = ENTITY_TYPES.PROP;
      _entity.dimension = (int)DIMENSIONEN.WORLD;
      _entity.position = prop.position;
      _entity.range = 20;
      _entity.data.Add("model", prop.model);
      _entity.CreateEntity();
      // _entity.SetSyncedData("sideProducts", sammlerData.sideProducts);
      sammlerData.Entities.Add(_entity);

      _logger.Debug("Entity created " + prop.position.ToString());
    }
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    if (_farmingPlayers.ContainsKey(player))
    {
      _farmingPlayers.Remove(player);
      player.Emit("stopAnim");
      return true;
    };
    sammler_farming_data _currentSammler = null!;
    // Get the Closest Sammler
    _sammler.ForEach((sammler) =>
    {
      if (sammler.Position.Distance(player.Position) < 80)
      {
        _currentSammler = sammler;
      }
    });
    if (_currentSammler == null) return false;
    // Get the Closest Prop of the closest Farming field
    xEntity _currentEntity = null!;
    _currentSammler.Entities.ForEach((entity) =>
    {
      if (entity.position.Distance(player.Position) < 2)
      {
        _currentEntity = entity;
      }
    });
    if (_currentEntity == null) return false;

    if (await player.HasItem(_currentSammler.tool) == false)
    {
      player.SendMessage("Du benötigst ein " + _currentSammler.tool, NOTIFYS.ERROR);
      return false;
    };

    _logger.Debug("Entity found");
    player.Emit("pointAtCoords", _currentEntity.entity.Position.X, _currentEntity.entity.Position.Y, _currentEntity.entity.Position.Z);
    player.Emit("playAnim", "melee@large_wpn@streamed_core_fps", "ground_attack_on_spot", -1, 1);

    _farmingPlayers.Add(player, _currentSammler.id);
    return true;
  }

  public async void OnLoad()
  {
    _farmingPlayers = new Dictionary<xPlayer, int>();

    await using ServerContext serverContext = new ServerContext();
    _logger.Startup("Lade Sammler!");
    foreach (sammler_farming_data sammler in serverContext.sammler_farming_data)
    {
      _sammler.Add(sammler);
      LoadSammler(sammler);
      _logger.Debug($"Sammler {sammler.name} geladen");
    }
    _logger.Startup($"x{_sammler.Count} Farming Sammler geladen");

    AltAsync.OnClient<xPlayer, string, string, string, string>("createroutenprop", async (player, route, pos, rot, prop) =>
    {
      _sammler.ForEach(async (sammler) =>
      {
        if (sammler.name == route)
        {
          propData _prop = new propData(
            JsonConvert.DeserializeObject<Rotation>(rot),
            JsonConvert.DeserializeObject<Position>(pos),
            prop
          );

          sammler.PropPositions.Add(_prop);
          sammler._propPositions = JsonConvert.SerializeObject(sammler.PropPositions);
          xEntity _entity = new xEntity();
          _entity.entityType = ENTITY_TYPES.PROP;
          _entity.dimension = (int)DIMENSIONEN.WORLD;
          _entity.position = JsonConvert.DeserializeObject<Position>(pos);
          _entity.range = 100;
          _entity.data.Add("model", prop);
          _entity.CreateEntity();

          sammler.Entities.Add(_entity);

          await using ServerContext serverContext = new ServerContext();
          serverContext.sammler_farming_data.Update(sammler);
          await serverContext.SaveChangesAsync();
        }
      });
    });
  }

  public sammler_farming_data GetSammler(int id)
  {
    foreach (sammler_farming_data sammler in _sammler)
    {
      if (sammler.id == id) return sammler;
    }
    return null!;
  }

  public async Task<bool> FarmingStep(xPlayer player, sammler_farming_data feld)
  {
    if (player == null) return false;
    if (feld == null) return false;
    IStorageHandler _storageHandler = new StorageHandler();
    xStorage inv = await _storageHandler.GetStorage(player.playerInventorys["inventory"]!);
    int random = new Random().Next(feld.amountmin, feld.amountmax);
    inv.AddItem(feld.item, random);
    player.SendMessage("Du hast " + random + " " + Items.Items.GetItem(feld.item).name + " gesammelt", NOTIFYS.INFO);

    return true;
  }

  public async void OnFiveSecondsUpdate()
  {
    foreach (KeyValuePair<xPlayer, int> kvp in _farmingPlayers)
    {
      if (kvp.Key == null)
      {
        _farmingPlayers.Remove(kvp.Key!);
        continue;
      };
      bool done = await FarmingStep(kvp.Key, GetSammler(kvp.Value));
      if (!done) _farmingPlayers.Remove(kvp.Key!);
    }
  }

}