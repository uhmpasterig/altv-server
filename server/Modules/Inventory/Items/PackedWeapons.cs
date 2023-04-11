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
    async void UnpackFunc(xPlayer player, string item, string weapon)
    {
      xStorage inv = await _storageHandler.GetStorage(player.playerInventorys["inventory"]);
      if(inv.RemoveItem("packed_weapon_bat", 1)){
        bool succ = player.GiveSavedWeapon("weapon_bat", 100, true);
        if(!succ)
          player.SendMessage("Du hast kein Platz fuer den Baseball Schlaeger", NOTIFYS.ERROR);
          inv.AddItem("packed_weapon_bat", 1);
      }
      else
        player.SendMessage("Du hast kein Baseball Schlaeger", NOTIFYS.ERROR);
    }

    Items.RegisterUsableItem("packed_weapon_pistol_mk2", async (xPlayer player) =>
    {
      UnpackFunc(player, "packed_weapon_pistol_mk2", "weapon_pistol_mk2");
    });

    Items.RegisterUsableItem("packed_weapon_bat", async (xPlayer player) =>
    {
      UnpackFunc(player, "packed_weapon_bat", "weapon_bat");
    });
  }
}