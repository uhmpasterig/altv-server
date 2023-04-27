using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using server.Util.Factions;
using server.Handlers.Storage;
using server.Modules.Inventory;
using Microsoft.EntityFrameworkCore;

namespace server.Modules.Factions;

class FactionModule : ILoadEvent, IPressedEEvent
{
  public FactionModule()
  {
  }

  public static List<Faction> factionList = new List<Faction>();
  static ServerContext _serverContext = new ServerContext();
  static StorageHandler storageHandler = new StorageHandler();

  public async void OnLoad()
  {
    factionList = _serverContext.Factions.Include(f => f.Ranks).ToList();
    _logger.Startup($"x{factionList.Count} Fraktionen wurden geladen!");
  }

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
    List<Models.Player> players = _serverContext.Players.Where(p => p.job == frakname.ToLower()).ToList();
    return players;
  }

  public static async Task<Faction> GetFaction(string name)
  {
    return await _serverContext.Factions.FirstOrDefaultAsync(f => f.name == name);
  }

  public static async Task<Faction> GetFaction(int id)
  {
    return await _serverContext.Factions.FirstOrDefaultAsync(f => f.id == id);
  }

  public static async Task<Faction> GetFaction(Faction faction)
  {
    return factionList.Where(f => f.id == faction.id).FirstOrDefault();
  }
}