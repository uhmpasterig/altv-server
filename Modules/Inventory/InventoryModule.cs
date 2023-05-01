using server.Models;
using server.Core;
using server.Events;
using Newtonsoft.Json;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using server.Modules.Items;
using server.Config;
using server.Util.Inventory;

using server.Handlers.Vehicle;
using server.Handlers.Storage;
using server.Handlers.Player;
using server.Handlers.Items;

namespace server.Modules.Inventory;

public class InventoryModule : IPressedIEvent, ILoadEvent
{
  IStorageHandler _storageHandler;
  IPlayerHandler _playerHandler;
  IVehicleHandler _vehicleHandler;
  IItemHandler _itemHandler;

  public InventoryModule(IStorageHandler storageHandler, IPlayerHandler playerHandler, IVehicleHandler vehicleHandler, IItemHandler itemHandler)
  {
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
      /* var watch = System.Diagnostics.Stopwatch.StartNew();
      xPlayer playerr = (xPlayer)player;
      StorageHandler storageHandler = new StorageHandler();
      xStorage from = await storageHandler.GetStorage(fromStorage);
      xStorage to = await storageHandler.GetStorage(toStorage);
      InventoryItem? item = from.items.Find(x => x.slot == fslot);
      InventoryItem? item2 = to.items.Find(x => x.slot == tslot);
      if (item == null && item2 == null) return;
      if (from == null || to == null) return;

      if (count == 0 && item != null)
      {
        count = item!.count;
      }
      try
      {
        await DragCheck(item!, item2!, from, to, fslot, tslot, count);
      }
      catch (Exception e)
      {
        _logger.Log(e.Message);
      }
      from.CalculateWeight();
      to.CalculateWeight();

      List<xStorage> uiStorages = new List<xStorage>();
      foreach (int storageId in userOpenInventorys[(xPlayer)player])
      {
        xStorage? storage = await storageHandler.GetStorage(storageId);
        uiStorages.Add(storage!);
      }

      if (fromStorage != toStorage)
      {
        player.Emit("playAnim", "storage_pickup");
      }

      player.Emit("frontend:open", "inventar", new inventoryWriter(uiStorages));
      watch.Stop();
      var elapsedTicks = watch.ElapsedTicks;
      var elapsedMs = watch.ElapsedMilliseconds;
      var additonalInfo = $"Ticks: {elapsedTicks} | Milliseconds: {elapsedMs}"; */
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