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
using server.Models;

namespace server.Commands;

internal class WeaponCommands : IScript
{
  static ServerContext _serverContext = new ServerContext();
  [Command("giveweapon")]
  public static void GiveWeapon(xPlayer player, string name, int ammo = 100)
  {
    player.GiveSavedWeapon(name, ammo);
  }

  [Command("r")]
  public static void Revive(xPlayer player)
  {
    player.Revive();
  }

  /* [Command("frakweap")]
  public async static void FrakWeap(xPlayer player)
  {
    StorageHandler _storageHandler = new StorageHandler();
    
    Modles.Faction frak = FraktionsModuleMain.GetFrak(player.job);
    xStorage inventory = await _storageHandler.GetStorage(player.boundStorages["Inventar"]);
    if(frak == null) return;
    inventory.AddItem(frak.weapon, 1);
    inventory.AddItem("packed_specialcarbine", 1);
  } */

  [Command("useitem")]
  public static void UseItem(xPlayer player, string name)
  {
    Items.UseItem(player, name);
  }

  [Command("giveitem")]
  public async static void GiveItem(xPlayer player, string name, int amount = 1)
  {
    StorageHandler _storageHandler = new StorageHandler();
    xStorage inventory = await _storageHandler.GetStorage(player.boundStorages["Inventar"]);
    inventory.AddItem(name, amount);
  }

  [Command("createprop")]
  public static void CreateProp(xPlayer player, string prop = "prop_roadcone02a")
  {
    player.Emit("propCreator", "createprop", prop);
  }

  [Command("anziehen")]
  public static void Test(xPlayer player, int id)
  {
    Models.Cloth cloth = _serverContext.Clothes.Where(c => c.id == id).FirstOrDefault()!;
    player.SetDlcClothes(cloth.component, cloth.drawable, cloth.texture, cloth.palette, cloth.dlc);
  }
}