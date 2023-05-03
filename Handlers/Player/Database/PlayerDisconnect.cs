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

public partial class PlayerHandler : IPlayerDisconnectEvent
{
  public async Task SavePlayerToDatabase(xPlayer player)
  {
    var ctx = new PlayerContext().Instance;

    var dbPlayer = await ctx.Players
      .Include(x => x.Weapons)
      .Include(x => x.WorldOffset)
      .Include(x => x.Vitals)
      .Where(x => x.Identifier.launcher_name == player.Name)
      .FirstOrDefaultAsync();

    dbPlayer.Weapons = player.Weapons;
    dbPlayer.WorldOffset = player.WorldOffset;
    dbPlayer.Vitals = player.Vitals;

    await ctx.ClearInstance();
  }

  public async void OnPlayerDisconnect(xPlayer player, string reason)
  {
    await SavePlayerToDatabase(player);
  }
}
