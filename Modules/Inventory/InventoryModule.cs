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
      if (item == null) return;
      if (item == null && item2 == null) return;
      watch.Stop();
      _logger.Log($"GetItem: Ticks: {watch.ElapsedTicks} | Milliseconds: {watch.ElapsedMilliseconds}");

      watch = System.Diagnostics.Stopwatch.StartNew();
      await DragItem(from, to, item!, item2, fslot, tslot, count);

      watch.Stop();
      _logger.Log($"Drag: Ticks: {watch.ElapsedTicks} | Milliseconds: {watch.ElapsedMilliseconds}");

      from.CalculateWeight();
      to.CalculateWeight();

      foreach (xPlayer p in storagePlayers[from.id])
      {
        p.Emit("inventory:update", new StorageWriter(from));
      }

      foreach (xPlayer p in storagePlayers[to.id])
      {
        p.Emit("inventory:update", new StorageWriter(to));
      }
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

  public async Task DragItem(xStorage s1, xStorage s2, Storage_Item i1, Storage_Item? i2, int _s1, int _s2, int count)
  {
    if (s1.id == s2.id)
    {
      await DragInsideStorage(s1, i1, i2, _s1, _s2, count);
      return;
    }
    else
    {
      await DragOutsideStorage(s1, s2, i1, i2, _s1, _s2, count);
      return;
    }
  }

  public async Task DragOutsideStorage(xStorage s1, xStorage s2, Storage_Item _i1, Storage_Item? i2, int _s1, int _s2, int count = 0)
  {
    if (_i1 == null) return;

    Storage_Item i1 = _i1;
    if (count != 0)
    {
      if (count > i1.count) count = i1.count;
      i1 = new Storage_Item(i1.Item_Data, count);
      _i1.count -= count;
      await s1.UpdateItem(_i1);
      await s1.AddItem(_i1, _s1);
    }
    else
    {
      count = i1.count;
    }

    // if its only 1 item
    if (i2 == null)
    {
      if (await s2.CanCarryItem(i1, count))
      {
        await s1.RemoveItem(i1);
        i1.storage_id = s2.id;
        i1.Storage = s2;
        i1.slot = _s2;
        await s2.AddItem(i1, _s2, true);
      }
      else
      {
        await s1.RemoveItem(i1);
        return;
      }
    }
  }


  public async Task DragInsideStorage(xStorage storage, Storage_Item i1, Storage_Item? i2, int _s1, int _s2, int count = 0)
  {
    if (i1 == null) return;

    // Check for the count and set it to the max if its 0 or higher than the count
    if (count == 0 || count > i1.count) count = i1.count;

    // If the Second item is Null just set the slot
    if (i2 == null)
    {
      i1.slot = _s2;
      await storage.UpdateItem(i1);
      return;
    }

    // If both items are the same sub from the first and fill the second
    if (i1.Item_Data.id == i2.Item_Data.id)
    {
      await this.MergeItems(storage, storage, i1, i2, count);
      return;
    }

    // if none of the above is right just swap the slots
    await SwapItems(storage, storage, i1, i2);
    return;
  }

  public async Task SwapItems(xStorage s1, xStorage s2, Storage_Item i1, Storage_Item i2)
  {
    if (i1 == null || i2 == null) return;

    int slot = i1.slot;
    i1.slot = i2.slot;
    i2.slot = slot;

    await s1.UpdateItem(i1);
    await s2.UpdateItem(i2);
  }

  public async Task MergeItems(xStorage s1, xStorage s2, Storage_Item i1, Storage_Item i2, int count)
  {
    // Check if the items are the same and not null
    if (i1 == null || i2 == null && i1.item_id != i2.item_id) return;
    // Set values needed for math
    int stackSize = i1.Item_Data.stackSize;
    int amount = i2.count + count;
    int newItemCount = (amount > stackSize) ? stackSize : amount;
    int newCount = (amount > stackSize) ? amount - stackSize : 0;

    // Set the new values
    i2.count = newItemCount;
    i1.count = newCount;

    // Update the items
    await s1.UpdateItem(i1);
    await s2.UpdateItem(i2);
    // Remove the item if the count is 0
    if (i1.count == 0) await s1.RemoveItem(i1);
    return;
  }

  /* // Ich weis das ist scheiße aber ich hab keine Lust mehr
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