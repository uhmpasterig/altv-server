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
using _logger = server.Logger.Logger;

namespace server.Modules.Inventory;

public class InventoryModule : IPressedIEvent, ILoadEvent
{
  internal static IPlayerHandler playerHandler = new PlayerHandler();
  internal Dictionary<xPlayer, List<int>> userOpenInventorys = new Dictionary<xPlayer, List<int>>();

  public async Task<bool> OnKeyPressI(xPlayer player)
  {
    IVehicleHandler vehicleHandler = new VehicleHandler();
    IStorageHandler storageHandler = new StorageHandler();
    List<object> uiStorages = new List<object>();
    List<int> openInventorys = new List<int>();

    xStorage playerStorage = await storageHandler.GetStorage(player.playerInventorys["inventory"]);
    playerStorage.AddItem("weste", 2);
    uiStorages.Add(playerStorage.GetData());
    openInventorys.Add(playerStorage.id);

    if (player.IsInVehicle)
    {
      xVehicle vehicle = (xVehicle)player.Vehicle;
      xStorage gloveStorage = await storageHandler.GetStorage(vehicle.storageIdGloveBox);
      openInventorys.Add(gloveStorage.id);
      uiStorages.Add(gloveStorage.GetData());
      goto load;
    }
    xVehicle closestVehicle = vehicleHandler.GetClosestVehicle(player.Position);
    if (closestVehicle != null)
    {
      xStorage trunkStorage = await storageHandler.GetStorage(closestVehicle.storageIdTrunk);
      openInventorys.Add(trunkStorage.id);
      uiStorages.Add(trunkStorage.GetData());
      goto load;
    }
    xStorage closestStorage = storageHandler.GetClosestxStorage(player.Position);
    if (closestStorage != null)
    {
      openInventorys.Add(closestStorage.id);
      uiStorages.Add(closestStorage.GetData());
    }

  load:
    userOpenInventorys[player] = openInventorys;
    player.Emit("inventory:open", JsonConvert.SerializeObject(uiStorages));
    return true;
  }

  public async Task<bool> DragCheck(InventoryItem fromi, InventoryItem toi, xStorage from, xStorage to) 
  {
    if (fromi == null && toi == null) return false;
    _logger.Log("1");
    if(to.weight + (fromi.weight * fromi.count) > to.maxWeight) return false;
    _logger.Log("2");
    
    if(fromi != null && toi != null){
      if(fromi!.name == toi.name ){
        if(fromi.count + toi.count <= toi.stackSize){
          toi.count += fromi.count;
          from.items.Remove(fromi);
        } else {
          int diff = toi.stackSize - toi.count;
          toi.count = toi.stackSize;
          fromi.count -= diff;
        }
        return true;
      }
    }
    if(toi == null) {
      if(to.items.Count >= to.slots) return false;
    }
    if(fromi != null) {
      from.items.Remove(fromi);
      to.items.Add(fromi);
    } 
    if(toi != null) {
      to.items.Remove(toi);
      from.items.Add(toi);
    }
    return true;
  }

  public async void OnLoad()
  {
    AltAsync.OnClient<IPlayer, int, int, int, int, int>("inventory:moveItem", async (player, fslot, tslot, fromStorage, toStorage, count) =>
    {
      xPlayer playerr = (xPlayer)player;
      IStorageHandler storageHandler = new StorageHandler();
      xStorage from = await storageHandler.GetStorage(fromStorage);
      xStorage to = await storageHandler.GetStorage(toStorage);
      InventoryItem item = from.items.Find(x => x.slot == fslot)!;
      InventoryItem item2 = to.items.Find(x => x.slot == tslot)!;
      
      if(count == 0){
        count = item.count;
      }
      try{
        await DragCheck(item, item2, from, to);
      } catch(Exception e){
        _logger.Log(e.Message);
      }

      List<object> uiStorages = new List<object>();
      foreach (int storageId in userOpenInventorys[(xPlayer)player])
      {
        xStorage storage = await storageHandler.GetStorage(storageId);
        uiStorages.Add(storage.GetData());
      }

      player.Emit("inventory:open", JsonConvert.SerializeObject(uiStorages));
    });
  }
}