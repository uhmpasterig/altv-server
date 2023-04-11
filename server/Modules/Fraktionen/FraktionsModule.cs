using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;

namespace server.Modules.Fraktionen;

class FraktionsModuleMain : ILoadEvent
{
  public static Dictionary<string, BadFrak> frakList = new Dictionary<string, BadFrak>();
  
  public async void OnLoad()
  {
    await using var serverContext = new ServerContext();
    foreach(BadFrak _frak in serverContext.BadFrak)
    {
      Alt.Log("Frak: " + _frak.name);
      frakList.Add(_frak.name, _frak);
    }
  }

  public static BadFrak GetFrak(string name)
  {
    if(frakList.FirstOrDefault(x => x.Key.ToLower() == name.ToLower()).Key != null)
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
