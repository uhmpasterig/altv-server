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
using Microsoft.EntityFrameworkCore;

namespace server.Modules.Farming.Sammler;
public class SammlerMain : ILoadEvent, IPressedEEvent, IFiveSecondsUpdateEvent
{
  public SammlerMain()
  {
  }

  public static List<Farming_Collector> _sammler = new List<Farming_Collector>();
  private Dictionary<xPlayer, int> _farmingPlayers;

  public static async Task<xEntity> CreatePropForRoute(Farming_Prop prop)
  {
    xEntity _entity = new xEntity();
    _entity.entityType = ENTITY_TYPES.PROP;
    _entity.dimension = (int)DIMENSIONEN.WORLD;
    _entity.position = prop.Position;
    _entity.range = 250;
    _entity.data.Add("model", prop.model);
    _entity.data.Add("rotation", JsonConvert.SerializeObject(prop.Rotation));
    _entity.CreateEntity();

    return _entity;
  }

  public async void LoadSammler(Farming_Collector sammlerData)
  {

    foreach (Farming_Prop prop in sammlerData.Props)
    {
      xEntity entity = await CreatePropForRoute(prop);
      sammlerData.Entities.Add(entity);
    }
  }

  public async void OnLoad()
  {
    _farmingPlayers = new Dictionary<xPlayer, int>();
    await using ServerContext serverContext = new ServerContext();
    _sammler = await serverContext.Farming_Collectors.Include(f => f.Props).ToListAsync();
    _sammler.ForEach((sammler) =>
    {
      LoadSammler(sammler);
    });
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {

    #region Checks if he can farm
    if (_farmingPlayers.ContainsKey(player))
    {
      _farmingPlayers.Remove(player);
      player.Emit("stopAnim");
      return true;
    };

    Farming_Collector _currentSammler = null!;
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
      if (entity.position.Distance(player.Position) < 3)
      {
        _currentEntity = entity;
      }
    });
    if (_currentEntity == null) return false;

    if (await player.HasItem(_currentSammler.tool) == false)
    {
      player.SendMessage("Du benötigst ein/eine " + Items.Items.GetItemLabel(_currentSammler.tool), NOTIFYS.ERROR);
      return false;
    };
    #endregion
    _logger.Debug("Entity found");
    player.Emit("pointAtCoords", _currentEntity.position.X, _currentEntity.position.Y, _currentEntity.position.Z);
    await Task.Delay(1000);
    player.Emit("playAnim", _currentSammler.animation);

    _farmingPlayers.Add(player, _currentSammler.id);
    return true;
  }

  public Farming_Collector GetSammler(int id)
  {
    foreach (Farming_Collector sammler in _sammler)
    {
      if (sammler.id == id) return sammler;
    }
    return null!;
  }

  public async Task<bool> FarmingStep(xPlayer player, Farming_Collector feld)
  {
    if (player == null) return false;
    if (feld == null) return false;
    IStorageHandler _storageHandler = new StorageHandler();
    xStorage inv = await _storageHandler.GetStorage(player.boundStorages["Inventar"]!);
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