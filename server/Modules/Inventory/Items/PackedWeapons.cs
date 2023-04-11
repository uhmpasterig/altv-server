using server.Core;
using server.Events;
using _logger = server.Logger.Logger;
using server.Core;
using server.Handlers.Storage;

namespace server.Modules.Items;

class PackedWeapons : IItemsLoaded
{
  private IStorageHandler _storageHandler = new StorageHandler();
  public void ItemsLoaded()
  {
    Items.RegisterUsableItem("packed_weapon_pistol_mk2", async (xPlayer player) =>
    {
      xStorage inv = await _storageHandler.GetStorage(player.playerInventorys["inventory"]);
      if(inv.RemoveItem("packed_weapon_pistol_mk2", 1))
        player.GiveSavedWeapon("weapon_pistol_mk2", 100, true);
      else
        player.SendMessage("Du hast kein Pistol MK2 Pack", NOTIFYS.ERROR);
    });

    Items.RegisterUsableItem("packed_weapon_bat", async (xPlayer player) =>
    {
      xStorage inv = await _storageHandler.GetStorage(player.playerInventorys["inventory"]);
      if(inv.RemoveItem("packed_weapon_bat", 1))
        player.GiveSavedWeapon("weapon_bat", 100, true);
      else
        player.SendMessage("Du hast kein Baseball Schlaeger", NOTIFYS.ERROR);
    });
  }
}