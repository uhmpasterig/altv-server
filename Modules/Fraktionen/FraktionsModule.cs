using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
namespace server.Modules.Fraktionen;

class FraktionsModuleMain : ILoadEvent, IPressedEEvent
{
  public static Dictionary<string, BadFrak> frakList = new Dictionary<string, BadFrak>();
  
  public async void OnLoad()
  {
    _logger.Startup("Fraktionen werden geladen...");
    await using var serverContext = new ServerContext();
    foreach(BadFrak _frak in serverContext.BadFrak)
    {
      _logger.Debug("Frak: " + _frak.name);
      frakList.Add(_frak.name.ToLower(), _frak);
    }
    _logger.Startup($"x{frakList.Count} Fraktionen wurden geladen!");
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    if(frakList.ContainsKey(player.job.ToLower()))
    {
      if(player.Position.Distance(frakList[player.job.ToLower()].Position) > 2) return false;
    }
    player.SendMessage("Du bist in der Fraktion: " + player.job, NOTIFYS.INFO);
    player.Emit("showFrak");
    return true;
  }

  public static BadFrak GetFrak(string name)
  {
    if(frakList.ContainsKey(name.ToLower()))
    {
      return frakList[name];
    }
    return null!;
  }

  public static void FrakToString(BadFrak frak)
  {
    Alt.Log($"Frak: {frak.name} - {frak.money} - {frak.logo} - {frak._pos} - {frak._posLager}");
  }
}
