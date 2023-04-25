using AltV.Net;
using server.Core;
using server.Models;
using AltV.Net.Data;
using Newtonsoft.Json;
using server.Util.Config;
using _logger = server.Logger.Logger;
using server.Modules.Items;

namespace server.Handlers.Storage;

public class StorageHandler
{
  ServerContext _serverContext = new ServerContext();
  public static readonly Dictionary<int, xStorage> Storages = new Dictionary<int, xStorage>();

  public StorageHandler()
  {
  }

  public async Task<xStorage> GetStorage(int storageId)
  {
    if (Storages.TryGetValue(storageId, out var storage))
      return storage;

    if (await LoadStorage(storageId))
      return Storages[storageId];

    return null!;
  }

  public async Task<bool> LoadStorage(int storageId)
  {

    var storage = await _serverContext.Storages.FindAsync(storageId);
    if (storage == null)
      return false;

    Storages.Add(storage.id, new xStorage(storage));
    _logger.Debug($"Storage {storageId} loaded into memory.");
    return true;
  }

  public async Task UnloadStorage(int storageId)
  {
    if (!Storages.TryGetValue(storageId, out var storage))
      return;

    await _serverContext.SaveChangesAsync();
    Storages.Remove(storageId);
    _logger.Debug($"Storage {storageId} unloaded from memory.");
  }

  public async Task<int> CreateStorage(string name, int slots, float maxWeight, Position? position, int ownerId = -1)
  {
    bool usePos = position == null ? false : true;
    if (position == null) position = new Position(0, 0, 0);

    var storage = new Models.Storage
    {
      name = name,
      slots = slots,
      maxWeight = maxWeight,
      ownerId = ownerId,
      Position = position,
      usePos = usePos,
      _items = JsonConvert.SerializeObject(new List<InventoryItem>())
    };

    await _serverContext.Storages.AddAsync(storage);
    await _serverContext.SaveChangesAsync();
    return storage.id;
  }

  public async Task SaveAllStorages()
  {
    foreach (var storage in Storages.Values)
    {
      var dbStorage = await _serverContext.Storages.FindAsync(storage.id);

      dbStorage!.name = storage.name;
      dbStorage.slots = storage.slots;
      dbStorage.maxWeight = storage.maxWeight;
      dbStorage._items = JsonConvert.SerializeObject(storage.items);
    }
    await _serverContext.SaveChangesAsync();
  }

  public async Task CreateAllStorages(xPlayer player)
  {
    foreach (StorageConfig.StorageData storageData in StorageConfig.StoragesDieJederHabenSollte)
    {
      if (player.playerInventorys.ContainsKey(storageData.name)) continue;
      _logger.Debug($"Creating storage {storageData.name} for player {player.name}.");

      int storageId = await CreateStorage(storageData.name, storageData.slots, storageData.maxWeight, storageData.position, player.id);
      player.playerInventorys.Add(storageData.name, storageId);
      await _serverContext.SaveChangesAsync();
    }
  }

  public xStorage GetClosestxStorage(xPlayer player, int range = 2)
  {
    return Storages.Values.FirstOrDefault(v => player.Position.Distance(v.Position) < range && v.ownerId == player.id);
  }
}