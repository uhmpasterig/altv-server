using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using server.Util.Fraktionen;
namespace server.Modules.Fraktionen;

class FraktionsModuleMain : ILoadEvent, IPressedEEvent
{
  public static Dictionary<string, Fraktion> frakList = new Dictionary<string, Fraktion>();
  public static Dictionary<int, Fraktion_ug> frakUgList = new Dictionary<int, Fraktion_ug>();

  public async void OnLoad()
  {
    await using var serverContext = new ServerContext();
    foreach (Fraktion _frak in serverContext.Fraktionen)
    {
      
      List<Fraktion_rang> _raenge = serverContext.Fraktionen_range.Where(r => r.fraktions_id == _frak.id).ToList();
      _frak.raenge = _raenge;
      
      Fraktion_ug _ug = serverContext.Fraktionen_ugs.FirstOrDefault(u => u.id == _frak.ug_id)!;
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
    Fraktion frak = frakList[player.job.ToLower()];
    player.SendMessage("Du bist in der Fraktion: " + player.job, NOTIFYS.INFO);
    player.Emit("frontend:open", "faction", new FraktionsWriter(frak, player));
    return true;
  }

  public static Fraktion GetFrak(string name)
  {
    if (frakList.ContainsKey(name.ToLower()))
    {
      return frakList[name];
    }
    return null!;
  }

  public static void FrakToString(Fraktion frak)
  {
    Alt.Log($"Frak: {frak.name} - {frak.money} - {frak._pos} - {frak._posLager}");
  }
}