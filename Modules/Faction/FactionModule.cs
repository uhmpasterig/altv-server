using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using server.Util.Factions;
using server.Handlers.Storage;
using Microsoft.EntityFrameworkCore;

namespace server.Modules.Factions;

class FactionModule : IPressedEEvent
{
  public FactionModule()
  {
  }

  static StorageHandler storageHandler = new StorageHandler();

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    Faction faction = await GetFaction(player.player_society.Faction);
    if (faction == null) return false;

    if (player.Position.Distance(player.player_society.Faction.Position) < 2)
    {
      _logger.Log($"Fraktions Menu von {player.Name} wurde geöffnet!");
      OpenFrakMenu(player, faction);
      return true;
    }
    return false;
  }

  static async void OpenFrakMenu(xPlayer player, Faction faction)
  {
    xStorage storage = await storageHandler.GetStorage(faction.storage_id);
    player.Emit("frontend:open", "faction", new FactionWriter(faction, player, storage));
  }

  public static string GetRankName(Faction faction, int rank)
  {
    return faction.Ranks.FirstOrDefault(r => r.rank_id == rank).label;
  }

  public static List<Models.Player> GetFactionMembers(string frakname)
  {
    ServerContext serverContext = new ServerContext();
    Faction faction = GetFaction(frakname);

    List<Models.Player> players = serverContext.Players.Include(p => p.player_society).ThenInclude(p => p.Faction).Where(p => p.player_society.faction_id == faction.id).ToList();
    return players;
  }

  public static Faction GetFaction(string name)
  {
    ServerContext _serverContext = new ServerContext();
    return _serverContext.Factions.Include(f => f.Ranks).Include(f => f.Members).ThenInclude(m => m.Player).FirstOrDefault(f => f.name == name);
  }

  public static async Task<Faction> GetFaction(int id)
  {
    ServerContext _serverContext = new ServerContext();
    return await _serverContext.Factions.Include(f => f.Ranks).Include(f => f.Members).ThenInclude(m => m.Player).FirstOrDefaultAsync(f => f.id == id);
  }

  public static async Task<Faction> GetFaction(Faction faction)
  {
    ServerContext _serverContext = new ServerContext();
    return await _serverContext.Factions.Include(f => f.Ranks).Include(f => f.Members).ThenInclude(m => m.Player).FirstOrDefaultAsync(f => f.id == faction.id);
  }
}