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

  [Command("frakweap")]
  public async static void FrakWeap(xPlayer player)
  {
    IStorageHandler _storageHandler = new StorageHandler();
    
    Models.BadFrak frak = FraktionsModuleMain.GetFrak(player.job);
    xStorage inventory = await _storageHandler.GetStorage(player.playerInventorys["inventory"]);
    if(frak == null) return;
    inventory.AddItem(frak.weapon, 1);
    inventory.AddItem("packed_weapon_pistol_mk2", 1);
  }

  [Command("useitem")]
  public static void UseItem(xPlayer player, string name)
  {
    Items.UseItem(player, name);
    _logger.Log("www");
  }

  [Command("giveitem")]
  public async static void GiveItem(xPlayer player, string name, int amount = 1)
  {
    IStorageHandler _storageHandler = new StorageHandler();
    xStorage inventory = await _storageHandler.GetStorage(player.playerInventorys["inventory"]);
    inventory.AddItem(name, amount);
  }

  [Command("jobinfo")]
  public static void JobInfo(xPlayer player)
  {
    player.SendChatMessage($"Du bist im Job: {player.job} mit dem Rang: {player.job_rank}");
    Models.BadFrak frak = FraktionsModuleMain.GetFrak(player.job);
    FraktionsModuleMain.FrakToString(frak);
  }

  [Command("createprop")]
  public static void CreateProp(xPlayer player, string prop = "prop_roadcone02a")
  {
    player.Emit("propCreator", "createprop", prop);
  }
}
