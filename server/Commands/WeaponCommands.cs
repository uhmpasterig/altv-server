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

namespace server.Commands;

internal class WeaponCommands : IScript
{
  [Command("giveweapon")]
  public static void GiveWeapon(xPlayer player, string name, int ammo = 100)
  {
    player.GiveSavedWeapon(name, ammo);
  }

  [Command("jobinfo")]
  public static void JobInfo(xPlayer player)
  {
    player.SendChatMessage($"Du bist im Job: {player.job} mit dem Rang: {player.job_rank}");
    Models.BadFrak frak = FraktionsModuleMain.GetFrak(player.job);
    FraktionsModuleMain.FrakToString(frak);
  }
}
