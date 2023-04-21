using AltV.Net;
using server.Core;
using server.Models;
using AltV.Net.Data;
using Newtonsoft.Json;
using _logger = server.Logger.Logger;

namespace server.Handlers.Storage;

public class StorageHandler : IStorageHandler
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

  public async Task<int> CreateStorage(string name, int slots, float maxWeight)
  {
    var storage = new Models.Storage
    {
      name = name,
      slots = slots,
      maxWeight = maxWeight
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

  public xStorage GetClosestxStorage(xPlayer player, int range = 2)
  {
    _logger.Log($"Player {player.Name} [{player.id}] is searching for a storage.");
    
    foreach(xStorage storage in Storages.Values)
    {
      _logger.Log($"Checking storage {storage.id}.");
      _logger.Log($"Distance: {player.Position.Distance(storage.Position)}");
      _logger.Log($"Range: {range}.");
      _logger.Log($"Owner: {storage.ownerId}.");
      _logger.Log($"Is player owner: {storage.ownerId == player.id}.")
      if (player.Position.Distance(storage.Position) < range)
      {
        _logger.Log($"Player {player.Name} [{player.id}] found a storage.");
        return storage;
      }
    }

    return Storages.Values.FirstOrDefault(v => player.Position.Distance(v.Position) < range && v.ownerId == player.id);
  }
}