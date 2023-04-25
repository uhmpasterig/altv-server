using server.Core;
using server.Events;
using _logger = server.Logger.Logger;
using server.Handlers.Storage;
using _items = server.ModulesGoofy.Items.Items;

namespace server.Items;

class PackedWeapons : IItemsLoaded
{
  private IStorageHandler _storageHandler = new StorageHandler();
  public void ItemsLoaded()
  {
    async void UnpackFunc(xPlayer player, string item)
    {
      xStorage inv = await _storageHandler.GetStorage(player.playerInventorys["Inventar"]);
      bool succ = await player.GiveSavedWeapon(item, 100, true);
      if (!succ)
      {
        player.SendMessage($"Du hast kein Platz fuer {item}", NOTIFYS.ERROR);
        inv.AddItem(item, 1);
      }
    }

    _items.RegisterUsableItem("weapon_specialcarbine_mk2", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_specialcarbine_mk2");
    });

    _items.RegisterUsableItem("weapon_specialcarbine", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_specialcarbine");
    });
    _items.RegisterUsableItem("weapon_pistol", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_pistol");
    });

    _items.RegisterUsableItem("weapon_advancedrifle", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_advancedrifle");
    });

    _items.RegisterUsableItem("weapon_bullpuprifle", async (xPlayer player) =>
   {
     UnpackFunc(player, "weapon_bullpuprifle");
   });

    _items.RegisterUsableItem("weapon_bullpuprifle_mk2", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_bullpuprifle_mk2");
    });

    _items.RegisterUsableItem("weapon_bat", async (xPlayer player) =>
   {
     UnpackFunc(player, "weapon_bat");
   });

    _items.RegisterUsableItem("weapon_battleaxe", async (xPlayer player) =>
    {
      UnpackFunc(player, "weapon_battleaxe");
    });
  }
}