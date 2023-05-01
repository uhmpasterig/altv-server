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

    AltAsync.OnClient<xPlayer, int, int, int, int, int>("inventory:moveItem", async (player, fslot, tslot, fromStorage, toStorage, count) =>
    {
      _logger.Log($"fslot: {fslot}, tslot: {tslot}, fromStorage: {fromStorage}, toStorage: {toStorage}, count: {count}");

      xStorage? from = await _storageHandler.GetStorage(fromStorage);
      xStorage? to = await _storageHandler.GetStorage(toStorage);
      if (from == null || to == null) return;
      Storage_Item? item = await from.GetItem(fslot);
      Storage_Item? item2 = await to.GetItem(tslot);
      if (item == null) return;
      if (count == 0 || count > item.count) count = item.count;

      await DragItem(from, to, item!, item2, fslot, tslot, count);

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
  }

  private async Task DragItem(xStorage s1, xStorage s2, Storage_Item i1, Storage_Item? i2, int _s1, int _s2, int count)
  {
    // perform a weight check before making any changes
    if (s1.id != s2.id)
      if (!await s2.CanCarryItem(i1, count)) return;

    if (i2 == null)
      await SetIntoSlot(s1, s2, i1, _s1, _s2, count);
    else if (i1.item_id == i2.item_id)
      await MergeItems(s1, s2, i1, i2, count);
    else if (i1.item_id != i2.item_id)
      await SwapItems(s1, s2, i1, i2);
    else
      _logger.Error("Something went wrong while dragging items!");
  }

  private async Task SwapItems(xStorage s1, xStorage s2, Storage_Item i1, Storage_Item i2)
  {
    _logger.Log($"Swap Items: {i1.item_id} {i2.item_id}");
    // Check if the items are the same and not null
    if (i1 == null || i2 == null) return;
    // Remove the items from the slots
    await s1.RemoveItem(i1);
    await s2.RemoveItem(i2);

    // Swap the slots
    int slot = i1.slot;
    i1.slot = i2.slot;
    i2.slot = slot;
    // Add the items back to the slots
    await s1.AddItem(i2, i1.slot);
    await s2.AddItem(i1, i2.slot);
  }

  private async Task MergeItems(xStorage s1, xStorage s2, Storage_Item i1, Storage_Item i2, int count)
  {
    _logger.Log($"Merge Items: {i1.item_id} {i2.item_id}");
    // Check if the items are the same and not null
    if (i1 == null || i2 == null && i1.item_id != i2.item_id) return;

    // Set values needed for math
    int stackSize = i1.Item_Data.stackSize;
    int amount = i2.count + count;
    int newItemCount = (amount > stackSize) ? stackSize : amount;

    // Set the new values
    i2.count = newItemCount;
    i1.count -= count;

    // Update the items
    await s1.UpdateItem(i1);
    await s2.UpdateItem(i2);

    // Remove the item if the count is 0
    if (i1.count == 0) await s1.RemoveItem(i1);
    return;
  }

  private async Task SetIntoSlot(xStorage s1, xStorage s2, Storage_Item i1, int _s1, int _s2, int count)
  {
    _logger.Log($"SetIntoSlot: {s1.name} -> {s2.name} | {i1.item_id} | {_s1} -> {_s2} | {count}");
    // Check if the item is null
    if (i1 == null) return;

    // add logic for the count system so there stays an old stack of items inside the storage slot
    if (count < i1.count)
    {
      // Create a new item
      Storage_Item i2 = new Storage_Item(i1.Item_Data, i1.count - count, _s2);
      // Add the item
      await s1.AddItem(i2, _s1, true);
    }

    // simplify the code
    if (s1.id == s2.id)
    {
      // Set the new values
      i1.slot = _s2;
      i1.count = count;
      // Update the item
      await s1.UpdateItem(i1);
    }
    else
    {
      await s1.RemoveItem(i1);
      // Set the new values
      i1.slot = _s2;
      i1.count = count;
      i1.storage_id = s2.id;
      i1.Storage = s2;
      await s2.AddItem(i1, _s2);
    }
    return;
  }
}