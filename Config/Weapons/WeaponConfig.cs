using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;
namespace server.Config.Weapons;

public class WeaponConfig
{
  public static List<string> allowedWeapons = new List<string>() {
    "weapon_specialcarbine_mk2",
    "weapon_specialcarbine",
    "weapon_pistol",
    "weapon_advancedrifle",
    "weapon_bullpuprifle",
    "weapon_bullpuprifle_mk2",
    "weapon_bat",
    "weapon_battleaxe",
    "weapon_pistol_mk2"
  };

  public async static Task<bool> IsValidWeapon(string weaponName)
  {
    if (allowedWeapons.Contains(weaponName)) return true;
    else return false;
  }
}
