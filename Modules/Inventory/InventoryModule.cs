using server.Models;
using server.Core;
using server.Events;
using Newtonsoft.Json;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using server.Config;
using server.Util.Inventory;

using server.Handlers.Vehicle;
using server.Handlers.Storage;
using server.Handlers.Player;
using server.Handlers.Items;
using server.Handlers.Logger;

namespace server.Modules.Inventory;

public class InventoryModule : IPressedIEvent, ILoadEvent
{
  ILogger _logger;
  IStorageHandler _storageHandler;
  IPlayerHandler _playerHandler;
  IVehicleHandler _vehicleHandler;
  IItemHandler _itemHandler;

  public InventoryModule(ILogger logger, IStorageHandler storageHandler, IPlayerHandler playerHandler, IVehicleHandler vehicleHandler, IItemHandler itemHandler)
  {
    _logger = logger;
    _storageHandler = storageHandler;
    _playerHandler = playerHandler;
    _vehicleHandler = vehicleHandler;
    _itemHandler = itemHandler;
  }

  internal static Dictionary<int, List<xPlayer>> storagePlayers = new Dictionary<int, List<xPlayer>>();

  public static async void OpenStorage(xPlayer player, int storage_id)
  {
    /* List<xStorage> uiStorages = new List<xStorage>();
    List<int> openInventorys = new List<int>();

    xStorage? playerStorage = await _storageHandler.GetStorage(player.boundStorages[1]);
    uiStorages.Add(playerStorage!);
    openInventorys.Add(playerStorage.id);

    xStorage? storage = await _storageHandler.GetStorage(storage_id)!;
    if (storage != null)
    {
      openInventorys.Add(storage.id);
      uiStorages.Add(storage);
    } */
    // player.Emit("frontend:open", "inventar", new inventoryWriter(uiStorages));
  }

  public async Task<bool> OnKeyPressI(xPlayer player)
  {
    List<xStorage> uiStorages = await _storageHandler.GetViewableStorages(player);

    uiStorages.ForEach(s =>
    {
      if (storagePlayers.ContainsKey(s.id))
        storagePlayers[s.id].Add(player);
      else
        storagePlayers.Add(s.id, new List<xPlayer>() { player });
    });

    player.Emit("frontend:open", "inventar", new InventoryWriter(uiStorages));
    return true;
  }

  public void OnLoad()
  {
    AltAsync.OnClient<IPlayer, int, int, int, int, int>("inventory:moveItem", async (player, fslot, tslot, fromStorage, toStorage, count) =>
    {
      _logger.Log("inventory:moveItem");
      _logger.Log($"fslot: {fslot}, tslot: {tslot}, fromStorage: {fromStorage}, toStorage: {toStorage}, count: {count}");
      var watch = System.Diagnostics.Stopwatch.StartNew();
      xPlayer playerr = (xPlayer)player;
      xStorage? from = await _storageHandler.GetStorage(fromStorage);
      xStorage? to = await _storageHandler.GetStorage(toStorage);
      if (from == null || to == null) return;

      Storage_Item? item = await from.GetItem(fslot);
      Storage_Item? item2 = await to.GetItem(tslot);
      if (item == null && item2 == null) return;

      if (fromStorage == toStorage)
      {
        if (item != null)
          item.slot = tslot;
        if (item2 != null)
          item2.slot = fslot;
      }
      else
      {
        if (item != null)
        {
          item.storage_id = toStorage;
          item.Storage = to;
          item.slot = tslot;
        }
        if (item2 != null)
        {
          item2.storage_id = fromStorage;
          item2.Storage = from;
          item2.slot = fslot;
        }
      }

      from.CalculateWeight();
      to.CalculateWeight();

      watch.Stop();
      var additonalInfo = $"Ticks: {watch.ElapsedTicks} | Milliseconds: {watch.ElapsedMilliseconds}";
    });

    AltAsync.OnClient<xPlayer, int>("inventory:useItem", (player, slot) =>
    {
      Console.WriteLine("Use Item", slot);
      _itemHandler.UseItem(player, null!, slot);
      // _items.UseItemFromSlot(player, slot, storageId);
    });

    AltAsync.OnClient<xPlayer, int, int, int>("inventory:throwItem", (player, slot, storageId, count) =>
    {
      // _items.RemoveItemFromSlot(slot, storageId, count);
    });
  }

  /*   // Ich weis das ist scheiße aber ich hab keine Lust mehr
    public async Task<bool> DragCheck(InventoryItem fromi, InventoryItem toi, xStorage from, xStorage to, int fslot, int tslot, int count)
    {
      if (fromi == null && toi == null) return false;
      if (to.id == from.id) goto move;

      if (to.weight + (fromi?.weight * count) > to.maxWeight) return false;
      if (from.weight + (toi?.weight * toi?.count) > from.maxWeight)
      {
        if (toi == null) return false;
        if (fromi == null) return false;
        if (fromi.name == toi.name) goto move;
      };
    move:
      if (count == 0 && fromi != null)
      {
        count = fromi!.count;
      }
      else if (fromi!.count < count)
      {
        return false;
      }
      if (fromi != null && toi != null)
      {
        if (fromi!.name == toi.name && (fromi.count < fromi.stackSize && toi.count < toi.stackSize))
        {
          if (fromi.count + toi.count <= toi.stackSize)
          {
            toi.count += count;
            fromi.count -= count;
            if (fromi.count <= 0)
            {
              from.items.Remove(fromi);
            }
          }
          else
          {
            int diff = toi.stackSize - toi.count;
            toi.count = toi.stackSize;
            fromi.count -= diff;
          }
          return true;
        }
      }
      if (from.weight + (toi?.weight * toi?.count) > from.maxWeight) return false;
      if (fromi != null)
      {
        if (count != fromi.count)
        {
          if (fromi.count - count <= 0) return false;
          fromi.count -= count;
          InventoryItem item = new InventoryItem(fromi.id, fromi.name, fromi.label, fromi.stackSize, fromi.weight, fromi.job, fromi.data, tslot, count);
          to.items.Add(item);
          return true;
        }
      }
      if (toi == null)
      {
        if (to.items.Count >= to.slots) return false;
      }
      if (fromi != null)
      {
        from.items.Remove(fromi);
        fromi.slot = tslot;
        to.items.Add(fromi);
      }
      if (toi != null)
      {
        to.items.Remove(toi);
        toi.slot = fslot;
        from.items.Add(toi);
      }
      return true;
    } */

}