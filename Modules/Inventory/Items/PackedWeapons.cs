/* using server.Core;
using server.Events;

using server.Handlers.Storage;

namespace server.Modules.Items;

class PackedWeapons : IItemsLoaded
{
  private IStorageHandler _storageHandler = new StorageHandler();
  public void ItemsLoaded()
  {
    async void UnpackFunc(xPlayer player, string item)
    {
      xStorage inv = await _storageHandler.GetStorage(player.boundStorages["Inventar"]);
      bool succ = await player.GiveSavedWeapon(item, 100, true);
      if (!succ)
      {
        player.SendMessage($"Du hast kein Platz fuer {item}", NOTIFYS.ERROR);
        inv.AddItem(item, 1);
      }
    }

    Items.RegisterUsableItem("weapon_specialcarbine_mk2", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_specialcarbine_mk2");
    });

    Items.RegisterUsableItem("weapon_specialcarbine", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_specialcarbine");
    });

    Items.RegisterUsableItem("weapon_pistol", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_pistol");
    });

    Items.RegisterUsableItem("weapon_advancedrifle", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_advancedrifle");
    });

    Items.RegisterUsableItem("weapon_bullpuprifle", async (xPlayer player) =>
   {
     UnpackFunc(player, "weapon_bullpuprifle");
   });

    Items.RegisterUsableItem("weapon_bullpuprifle_mk2", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_bullpuprifle_mk2");
    });

    Items.RegisterUsableItem("weapon_bat", async (xPlayer player) =>
   {
     UnpackFunc(player, "weapon_bat");
   });

    Items.RegisterUsableItem("weapon_battleaxe", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_battleaxe");
    });
  }
} */