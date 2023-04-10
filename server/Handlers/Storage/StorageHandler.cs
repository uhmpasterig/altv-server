using AltV.Net;
using server.Core;
using server.Models;
using AltV.Net.Data;
using Newtonsoft.Json;
using _logger = server.Logger.Logger;

namespace server.Handlers.Storage;

public class StorageHandler : IStorageHandler
{
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
    using var serverContext = new ServerContext();
    var storage = await serverContext.Storages.FindAsync(storageId);
    if (storage == null)
      return false;

    Storages.Add(storage.id, new xStorage(storage));
    return true;
  }

  public void UnloadStorage(int storageId)
  {
    if (!Storages.TryGetValue(storageId, out var storage))
      return;

    Storages.Remove(storageId);
  }

  public async Task<int> CreateStorage(string name, int slots, float maxWeight)
  {
    using var serverContext = new ServerContext();
    var storage = new Models.Storage
    {
      name = name,
      slots = slots,
      maxWeight = maxWeight
    };

    await serverContext.Storages.AddAsync(storage);
    await serverContext.SaveChangesAsync();
    return storage.id;
  }

  public async Task SaveAllStorages()
  {
    _logger.Log("Saving all storages");
    await using var serverContext = new ServerContext();
    foreach (var storage in Storages.Values)
    {
      var dbStorage = await serverContext.Storages.FindAsync(storage.id);

      dbStorage.name = storage.name;
      dbStorage.slots = storage.slots;
      dbStorage.maxWeight = storage.maxWeight;
      dbStorage._items = JsonConvert.SerializeObject(storage.items);

      _logger.Log($"Saved storage {storage.id}");
    }
    await serverContext.SaveChangesAsync();
  }

  public xStorage GetClosestxStorage(Position position, int range = 2)
  {
    return Storages.Values.FirstOrDefault(v => position.Distance(v.Position) < range)!;
  }
}