using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using server.Util.Fraktionen;
using server.Handlers.Storage;
using server.Modules.Inventory;
using Microsoft.EntityFrameworkCore;

namespace server.Modules.Fraktionen;

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
    if (!factionList.Contains(player.player_society.Faction)) return false;

    if (player.Position.Distance(player.player_society.Faction.Position) < 2) {
      _logger.Log($"Fraktions Menu von {player.Name} wurde geöffnet!");
      OpenFrakMenu(player); 
      return true;
    }

    if (player.Position.Distance(player.player_society.Faction.StoragePosition) < 2){
      _logger.Log($"Fraktions Tresor von {player.Name} wurde geöffnet!");
      OpenFrakStorage(player);
      return true;
    }
    
    return false;
  }

  static async void OpenFrakMenu(xPlayer player)
  {
    xStorage storage = await storageHandler.GetStorage(player.player_society.Faction.storage_id);
    player.Emit("frontend:open", "faction", new FraktionsWriter(player.player_society.Faction, player, storage));
  }

  static async void OpenFrakStorage(xPlayer player)
  {
    InventoryModule.OpenStorage(player, player.boundStorages["Fraktions Tresor"]);
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
}