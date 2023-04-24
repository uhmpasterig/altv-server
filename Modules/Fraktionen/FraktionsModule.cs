using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using server.Util.Fraktionen;
using server.Handlers.Storage;
namespace server.Modules.Fraktionen;

class FraktionsModuleMain : ILoadEvent, IPressedEEvent
{
  public static Dictionary<string, Faction> frakList = new Dictionary<string, Faction>();
  public static Dictionary<int, Faction_ug> frakUgList = new Dictionary<int, Faction_ug>();
  static ServerContext _serverContext = new ServerContext();
  static IStorageHandler storageHandler = new StorageHandler();

  public async void OnLoad()
  {
    foreach (Faction _frak in _serverContext.Factions.ToList())
    {
      Dictionary<int, Faction_rank> _raenge = new Dictionary<int, Faction_rank>();
      foreach(Faction_rank _rang in _serverContext.Faction_ranks.Where(r => r.fraktions_id == _frak.id).ToList()) _raenge.Add(_rang.rank_id, _rang);
      _frak.raenge = _raenge;

      Faction_ug _ug = _serverContext.Faction_ugs.FirstOrDefault(u => u.id == _frak.ug_id)!;
      frakUgList.Add(_frak.id, _ug);

      _logger.Debug($"Fraktion: {_frak.name} wurde geladen!");
      _logger.Debug($"Fraktion: {_frak.name} hat x{_raenge.Count} Ränge!");
      _logger.Debug($"Fraktion: {_frak.name} hat {_ug.name} als Untergruppierung!");

      frakList.Add(_frak.name.ToLower(), _frak);
    }
    _logger.Startup($"x{frakList.Count} Fraktionen wurden geladen!");
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    if (frakList.ContainsKey(player.job.ToLower()))
    {
      if (player.Position.Distance(frakList[player.job.ToLower()].Position) > 2) return false;
    }
    Faction frak = frakList[player.job.ToLower()];
    xStorage storage =  await storageHandler.GetStorage(frak.storage_id);

    player.SendMessage("Du bist in der Fraktion: " + player.job, NOTIFYS.INFO);
    player.Emit("frontend:open", "faction", new FraktionsWriter(frak, player, storage));
    return true;
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