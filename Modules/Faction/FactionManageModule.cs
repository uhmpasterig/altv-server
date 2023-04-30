using server.Core;
using AltV.Net;
using AltV.Net.Async;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using server.Util.Factions;
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
    Models.Player? offlinePlayer = await _serverContext.Players.Include(p => p.player_society).FirstOrDefaultAsync(p => p.id == target);
    if (offlinePlayer == null) return;
    if(offlinePlayer.player_society.faction_rank_id == 12) {
      perms.Add("faction.leader");
    };

    offlinePlayer.player_society.FactionPerms = perms;

    if((offlinePlayer.player_society.faction_rank_id < player.player_society.faction_rank_id) || (rank < player.player_society.faction_rank_id)) {
      offlinePlayer.player_society.faction_rank_id = rank;
    };

    _logger.Info("SetMember: " + offlinePlayer.player_society.FactionPerms + " " + offlinePlayer.player_society.faction_rank_id);
    await _serverContext.SaveChangesAsync();
  }

  public async void OnLoad()
  {
    AltAsync.OnClient<xPlayer, int, string, int>("faction:leader:setMember", async (player, targetId, _perms, rank) => {
      _logger.Log($"{player.player_society.ToString()}");
      if(!(player.player_society.FactionPerms.Contains("faction.leader") || player.player_society.FactionPerms.Contains("faction.management"))) return;
      List<string> perms = JsonConvert.DeserializeObject<List<string>>(_perms)!;
      if(!player.player_society.FactionPerms.Contains("faction.leader") && perms.Contains("faction.management")) return;
      _logger.Info($"SetMember: {targetId} {perms.ToString()} {rank}");

      xPlayer? target = await playerHandler.GetPlayer(targetId);  

      if(target != null) {
        if(target.player_society.faction_rank_id == 12) {
          perms.Add("faction.leader");
        };

        target.player_society.FactionPerms = perms;
        if((target.player_society.faction_rank_id < player.player_society.faction_rank_id) || (rank < player.player_society.faction_rank_id)) {
          target.player_society.faction_rank_id = rank;
        };
      };
      SetPlayerFactionDatabase(player, targetId, perms, rank);
    });
  }
}