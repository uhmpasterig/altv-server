using server.Core;
using server.Events;
using _logger = server.Logger.Logger;

namespace server.Modules.Items;

class PackedWeapons : IItemsLoaded
{
  public void ItemsLoaded()
  {
    Items.RegisterUsableItem("packed_weapon_pistol_mk2", (xPlayer player) =>
    {
      player.GiveSavedWeapon("weapon_pistol_mk2", 100, true);
    });

    Items.RegisterUsableItem("packed_weapon_bat", (xPlayer player) =>
    {
      player.GiveSavedWeapon("weapon_bat", 100, true);
    });
  }
}