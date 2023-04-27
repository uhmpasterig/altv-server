using server.Core;
using AltV.Net;
using AltV.Net.Async;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using server.Util.Factions;
using server.Modules.Inventory;
using Newtonsoft.Json;
using server.Handlers.Player;
using Microsoft.EntityFrameworkCore;

namespace server.Modules.Factions;

class FactionManageModule : ILoadEvent
{
  static IPlayerHandler playerHandler = new PlayerHandler();
  public FactionManageModule()
  {
  }

  async void SetPlayerFactionDatabase(xPlayer player, int target, List<string> perms, int rank)
  {
    ServerContext _serverContext = new ServerContext();
    Models.Player? offlinePlayer = await _serverContext.Players.Include(p => p.player_society).FirstAsync(p => p.id == target);
    if (offlinePlayer == null) return;
    offlinePlayer.player_society.FactionPerms = perms;
    offlinePlayer.player_society.faction_rank_id = rank;
    await _serverContext.SaveChangesAsync();
  }

  public async void OnLoad()
  {
    AltAsync.OnClient<xPlayer, int, string, int>("faction:leader:setMember", async (player, targetId, _perms, _rank) => {
      if(!(player.player_society.FactionPerms.Contains("faction.leader") || player.player_society.FactionPerms.Contains("faction.management"))) return;
      List<string> perms = JsonConvert.DeserializeObject<List<string>>(_perms)!;
      if(!player.player_society.FactionPerms.Contains("faction.leader") && perms.Contains("faction.management")) return;

      xPlayer? target = await playerHandler.GetPlayer(targetId);  
      if(target != null) {
        target.player_society.FactionPerms = perms;
        target.player_society.faction_rank_id = _rank;
      };
      SetPlayerFactionDatabase(player, targetId, perms, _rank);
      
    });
  }
}