using AltV.Net;
using server.Core;
using server.Models;
using AltV.Net.Data;
using Newtonsoft.Json;
using server.Config;

using server.Modules.Items;
using Microsoft.EntityFrameworkCore;
using server.Events;
using server.Handlers.Logger;

namespace server.Handlers.Storage;

public class StorageHandler : IStorageHandler, IOneMinuteUpdateEvent
{
  ServerContext _storageCtx = new ServerContext();
  public readonly List<xStorage> Storages = new List<xStorage>();

  ILogger _logger;
  public StorageHandler(ILogger logger)
  {
    _logger = logger;
  }

  public async void OnOneMinuteUpdate()
  {
    await SaveAllStorages();
  }

  public async Task<xStorage?> GetStorage(int id)
  {
    xStorage? storage = Storages.Where(s => s.id == id).FirstOrDefault();
    if (storage != null)
      return storage;

    if (await LoadStorage(id))
      return Storages[id];

    return null;
  }

  public async Task<bool> LoadStorage(int id)
  {
    Models.Storage? storage = await _storageCtx.Storages
      .Include(s => s.Items)
      .ThenInclude(i => i.Item_Data)
      .Where(s => s.id == id)
      .FirstOrDefaultAsync();

    if (storage == null)
      return false;

    Storages.Add(new xStorage(storage));
    _logger.Debug($"Storage {id} loaded into memory.");
    return true;
  }

  public async Task UnloadStorage(int id)
  {
    xStorage? storage = Storages.Where(s => s.id == id).FirstOrDefault();
    if (storage == null)
      return;

    Storages.Remove(storage);
    var storageMdl = await _storageCtx.Storages.FindAsync(id);
    if (storageMdl == null) return;
    storageMdl.maxWeight = storage.maxWeight;
    storageMdl.slots = storage.slots;
    storageMdl.Items = storage.Items;
    _logger.Debug($"Storage {id} unloaded from memory.");
  }

  public async Task UnloadStorage(xStorage storage)
  {
    if (storage != null)
      await UnloadStorage(storage.id);
  }

  public async Task<int> CreateStorage(string name, int slots, float maxWeight, Position? position, int ownerId = -1)
  {
    bool usePos = position == null ? false : true;
    Position _position = position ?? new Position(0, 0, 0);

    var storage = new Models.Storage
    {
      name = name,
      slots = slots,
      maxWeight = maxWeight,
      ownerId = ownerId,
      Position = _position,
      usePos = usePos,
      Items = new List<Storage_Item>()
    };
    await _storageCtx.Storages.AddAsync(storage);
    await _storageCtx.SaveChangesAsync();
    return storage.id;
  }

  public async Task SaveAllStorages()
  {
    foreach (var storage in Storages)
    {
      var storageMdl = await _storageCtx.Storages.FindAsync(storage.id);

      storageMdl.slots = storage.slots;
      storageMdl.maxWeight = storage.maxWeight;
      storageMdl.Items = storage.Items;
    }
    await _storageCtx.SaveChangesAsync();
    _logger.Debug($"All storages saved to the Database.");
  }

  public async Task CreateAllStorages(xPlayer player)
  {
    foreach (StorageConfig.StorageData storageData in StorageConfig.StoragesDieJederHabenSollte)
    {
      if (player.boundStorages.ContainsKey(storageData.local_id)) continue;
      int storageId = await CreateStorage(storageData.name, storageData.slots, storageData.maxWeight, storageData.position, player.id);
      player.boundStorages.Add(storageData.local_id, storageId);
    }
  }

  public async Task<xStorage?> GetClosestStorage(xPlayer player, int range = 2)
  {
    xStorage? storage = Storages
      .Where(v => v.usePos)
      .Where(v => player.Position.Distance(v.Position) < range)
      .OrderBy(v => player.Position.Distance(v.Position))
      .FirstOrDefault();

    return storage;
  }

  public async Task<List<xStorage>> GetViewableStorages(xPlayer player)
  {
    List<xStorage> storages = new List<xStorage>();

    storages.Add((await this.GetStorage(player.boundStorages[(int)STORAGES.INVENTORY]))!);

    if (player.IsInVehicle)
    {
      xVehicle vehicle = (xVehicle)player.Vehicle;
      if (vehicle.storage_glovebox == null) goto end;
      storages.Add((await this.GetStorage(vehicle.storage_glovebox.id))!);
    }
  end:
    return storages;
  }
}