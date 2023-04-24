using System;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using server.Core;
using AltV.Net.Resources.Chat.Api;
using server.Handlers.Vehicle;
using AltV.Net.Data;
using AltV.Net;
using _logger = server.Logger.Logger;
using server.Modules.Fraktionen;
using server.Handlers.Storage;
using server.Modules.Items;

namespace server.Commands;

internal class WeaponCommands : IScript
{
  [Command("giveweapon")]
  public static void GiveWeapon(xPlayer player, string name, int ammo = 100)
  {
    player.GiveSavedWeapon(name, ammo);
  }

  [Command("revive")]
  public static void Revive(xPlayer player)
  {
    player.Revive();
  }

/*   [Command("frakweap")]
  public async static void FrakWeap(xPlayer player)
  {
    IStorageHandler _storageHandler = new StorageHandler();
    
    Modles.Faction frak = FraktionsModuleMain.GetFrak(player.job);
    xStorage inventory = await _storageHandler.GetStorage(player.playerInventorys["Inventar"]);
    if(frak == null) return;
    inventory.AddItem(frak.weapon, 1);
    inventory.AddItem("packed_specialcarbine", 1);
  }
 */
  [Command("useitem")]
  public static void UseItem(xPlayer player, string name)
  {
    Items.UseItem(player, name);
  }

  [Command("giveitem")]
  public async static void GiveItem(xPlayer player, string name, int amount = 1)
  {
    IStorageHandler _storageHandler = new StorageHandler();
    xStorage inventory = await _storageHandler.GetStorage(player.playerInventorys["Inventar"]);
    inventory.AddItem(name, amount);
  }

  [Command("createprop")]
  public static void CreateProp(xPlayer player, string prop = "prop_roadcone02a")
  {
    player.Emit("propCreator", "createprop", prop);
  }

  [Command("ttest")]
  public static void Test(xPlayer player)
  {
    player.SetDlcClothes(11, 4, 0, 0, Alt.Hash("mp_m_executive_01"));
  }

  [Command("ttest2")]
  public static void Test2(xPlayer player)
  {
    player.SetDlcClothes(11, 14, 0, 0, 0);
  }

  [Command("ttest3")]
  public static void Test3(xPlayer player)
  {
    player.SetDlcClothes(11, 23, 2, 0, 0);
  }
}