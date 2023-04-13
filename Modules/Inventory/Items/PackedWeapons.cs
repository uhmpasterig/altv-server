using server.Core;
using server.Events;
using _logger = server.Logger.Logger;
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
      bool succ = await player.GiveSavedWeapon(weapon, 100, true);
      if(!succ){
        player.SendMessage($"Du hast kein Platz fuer {item}", NOTIFYS.ERROR);
        inv.AddItem(item, 1);
      }
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