using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using server.Core;
using server.Models;
using Microsoft.EntityFrameworkCore;
using server.Events;
using Newtonsoft.Json;
using System.Diagnostics;
using server.Config;

using server.Handlers.Logger;

namespace server.Handlers.Player;

public partial class PlayerHandler : IPlayerConnectEvent
{
  /* async Task CheckIdentifiers(xPlayer player, Models.Player dbPlayer)
  {
    if (dbPlayer.Identifier.HWID == 0)
    {
      dbPlayer.Identifier.HWID = player.HardwareIdHash;
    }
    else
    {
      if (dbPlayer.Identifier.HWID != player.HardwareIdHash)
      {
        player.Kick("You have been kicked for HWID spoofing");
        return;
      }
    }
  } */

  public async Task<xPlayer> LoadPlayerFromDatabase(xPlayer player)
  {
    var ctx = new PlayerContext().Instance;
    Models.Player? dbPlayer = await ctx.Players
      .Include(p => p.Weapons)
      .Include(p => p.WorldOffset)
      .Include(p => p.Identifier)
      .Include(p => p.Vitals)
      .Where(p => p.Identifier.launcher_name == player.Name)
      .FirstOrDefaultAsync();

    if (dbPlayer == null)
    {
      Console.WriteLine($"Player {player.Name} not found in database");
      return player;
    }

    await player.LoadPlayer(dbPlayer);

    await ctx.ClearInstance();
    return player;
  }

  public async void OnPlayerConnect(xPlayer player, string reason)
  {
    _logger.Log("Player connected: " + player.Name);
    // catch error 
    player = await LoadPlayerFromDatabase(player);
  }
}
