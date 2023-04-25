using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using server.Util.Fraktionen;
using server.Handlers.Storage;
using server.Modules.Inventory;

namespace server.Modules.Fraktionen;

class FraktionsModuleMain : ILoadEvent, IPressedEEvent
{
  public FraktionsModuleMain()
  {
  }

  public static Dictionary<string, Faction> frakList = new Dictionary<string, Faction>();
  public static Dictionary<int, Faction_ug> frakUgList = new Dictionary<int, Faction_ug>();
  static ServerContext _serverContext = new ServerContext();
  static StorageHandler storageHandler = new StorageHandler();

  public async void OnLoad()
  {
    foreach (Faction _frak in _serverContext.Factions.ToList())
    {
      Dictionary<int, Faction_rank> _raenge = new Dictionary<int, Faction_rank>();
      foreach (Faction_rank _rang in _serverContext.Faction_ranks.Where(r => r.fraktions_id == _frak.id).ToList()) _raenge.Add(_rang.rank_id, _rang);
      _frak.raenge = _raenge;

      Faction_ug _ug = _serverContext.Faction_ugs.FirstOrDefault(u => u.id == _frak.ug_id)!;
      frakUgList.Add(_frak.id, _ug);

      frakList.Add(_frak.name.ToLower(), _frak);
    }
    _logger.Startup($"x{frakList.Count} Fraktionen wurden geladen!");
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    if (!frakList.ContainsKey(player.job.ToLower())) return false;

    if (player.Position.Distance(frakList[player.job.ToLower()].Position) < 2) {
      OpenFrakMenu(player); 
      return true;
    }

    if (player.Position.Distance(frakList[player.job.ToLower()].StoragePosition) < 2){
      OpenFrakStorage(player);
      return true;
    }
    
    return false;
  }

  static async void OpenFrakMenu(xPlayer player)
  {
    Faction frak = frakList[player.job.ToLower()];
    xStorage storage = await storageHandler.GetStorage(frak.storage_id);
  }

  static async void OpenFrakStorage(xPlayer player)
  {
    Faction frak = frakList[player.job.ToLower()];
    InventoryModule.OpenStorage(player, player.boundStorages["Fraktions Tresor"]);
  }

  public static string GetRankName(Faction frak, int rank)
  {
    return frak.raenge.FirstOrDefault(r => r.Key == rank)!.Value.label;
  }

  public static List<Models.Player> GetFrakMembers(string frakname)
  {
    List<Models.Player> players = _serverContext.Players.Where(p => p.job == frakname.ToLower()).ToList();
    return players;
  }

  public static Faction GetFrak(string name)
  {
    if (frakList.Where(f => f.Key.ToLower() == name.ToLower()) != null)
    {
      return frakList[name];
    }
    return null!;
  }
}