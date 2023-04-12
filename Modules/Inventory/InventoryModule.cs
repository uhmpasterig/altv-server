using server.Models;
using server.Handlers.Storage;
using server.Core;
using server.Events;
using Newtonsoft.Json;
using server.Handlers.Vehicle;
using _items = server.Modules.Items.Items;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using server.Modules.Items;
using server.Handlers.Player;
using server.Util.Inventory;
using _logger = server.Logger.Logger;

namespace server.Modules.Inventory;

public class InventoryModule : IPressedIEvent, ILoadEvent
{
  internal static IPlayerHandler playerHandler = new PlayerHandler();
  internal Dictionary<xPlayer, List<int>> userOpenInventorys = new Dictionary<xPlayer, List<int>>();
  IStorageHandler storageHandler = new StorageHandler();
  IVehicleHandler vehicleHandler = new VehicleHandler();

  public async Task<bool> OnKeyPressI(xPlayer player)
  {
    List<xStorage> uiStorages = new List<xStorage>();
    List<int> openInventorys = new List<int>();

    xStorage playerStorage = await storageHandler.GetStorage(player.playerInventorys["inventory"]);
    uiStorages.Add(playerStorage);
    openInventorys.Add(playerStorage.id);

    if (player.IsInVehicle)
    {
      xVehicle vehicle = (xVehicle)player.Vehicle;
      xStorage gloveStorage = await storageHandler.GetStorage(vehicle.storageIdGloveBox);
      openInventorys.Add(gloveStorage.id);
      uiStorages.Add(gloveStorage);
      goto load;
    }
    xVehicle closestVehicle = vehicleHandler.GetClosestVehicle(player.Position);
    if (closestVehicle != null)
    {
      xStorage trunkStorage = await storageHandler.GetStorage(closestVehicle.storageIdTrunk);
      openInventorys.Add(trunkStorage.id);
      uiStorages.Add(trunkStorage);
      goto load;
    }
    xStorage closestStorage = storageHandler.GetClosestxStorage(player.Position);
    if (closestStorage != null)
    {
      openInventorys.Add(closestStorage.id);
      uiStorages.Add(closestStorage);
    }

  load:
    userOpenInventorys[player] = openInventorys;
    player.Emit("frontend:open", "inventar", new inventoryWriter(uiStorages));
    return true;
  }
  public void OnLoad()
  {
    AltAsync.OnClient<IPlayer, int, int, int, int, int>("inventory:moveItem", async (player, fslot, tslot, fromStorage, toStorage, count) =>
    {
      xPlayer playerr = (xPlayer)player;
      IStorageHandler storageHandler = new StorageHandler();
      xStorage from = await storageHandler.GetStorage(fromStorage);
      xStorage to = await storageHandler.GetStorage(toStorage);
      InventoryItem item = from.items.Find(x => x.slot == fslot)!;
      InventoryItem item2 = to.items.Find(x => x.slot == tslot)!;
      if (item == null && item2 == null) return;
      if (from == null || to == null) return;

      if (count == 0 && item != null)
      {
        count = item!.count;
      }
      try
      {
        await DragCheck(item!, item2, from, to, fslot, tslot, count);
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
        xStorage storage = await storageHandler.GetStorage(storageId);
        uiStorages.Add(storage);
      }

      player.Emit("frontend:open", "inventar", new inventoryWriter(uiStorages));
    });

    AltAsync.OnClient<xPlayer, int, int>("inventory:useItem", (player, slot, storageId) =>
    {
      _items.UseItemFromSlot(player, slot, storageId);
    });

    AltAsync.OnClient<xPlayer, int, int>("inventory:removeItem", (player, slot, count) =>
    {
    });
  }

  // Ich weis das ist scheiße aber ich hab keine Lust mehr
  public async Task<bool> DragCheck(InventoryItem fromi, InventoryItem toi, xStorage from, xStorage to, int fslot, int tslot, int count)
  {
    if (fromi == null && toi == null) return false;
    if (to.id == from.id) goto move;
    _logger.Debug($"check1: {to.weight} + ({fromi?.weight} * {count}) > {to.maxWeight}");
    _logger.Debug($"check2: {from.weight} + ({toi?.weight} * {toi?.count}) > {from.maxWeight}");

    if (to.weight + (fromi?.weight * count) > to.maxWeight) return false;
    if (from.weight + (toi?.weight * toi?.count) > from.maxWeight)
    {
      if (toi == null) return false;
      if (fromi == null) return false;
      if (fromi.name == toi.name) goto move;
    };
  move:
    _logger.Debug("In check1");
    if (fromi != null && toi != null)
    {
      if (fromi!.name == toi.name && (fromi.count < fromi.stackSize && toi.count < toi.stackSize))
      {
        if (fromi.count + toi.count <= toi.stackSize)
        {
          toi.count += fromi.count;
          from.items.Remove(fromi);
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
    _logger.Debug("In check2");
    if (fromi != null)
    {
      _logger.Debug("In check3");
      if (count != fromi.count)
      {
        _logger.Debug("In check4");
        if (fromi.count - count <= 0) return false;
        fromi.count -= count;
        InventoryItem item = new InventoryItem(fromi.id, fromi.name, fromi.stackSize, fromi.weight, fromi.job, fromi.data, tslot, count);
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
  }

}