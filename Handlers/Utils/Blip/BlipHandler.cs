using AltV.Net;
using server.Models;
using server.Events;
using server.Core;

namespace server.Handlers.Blips;

public class BlipHandler : IBlipHandler, IPlayerConnectEvent
{
  public static Dictionary<int, Blip> blips = new Dictionary<int, Blip>();

  private static int GenerateBlipId()
  {
    int id = blips.Count;
    while (blips.ContainsKey(id))
    {
      id++;
    }
    return id;
  }

  public static void CreateBlip(Blip blip)
  {
    int id = GenerateBlipId();
    blip.id = id;
    blips.Add(id, blip);
  }

  public static void UpdateBlip(Blip blip)
  {
    blips[blip.id] = blip;
  }

  public static void DeleteBlip(int id)
  {
    blips.Remove(id);
  }

  public async void OnPlayerConnect(xPlayer player, string reason)
  {
    foreach (KeyValuePair<int, Blip> blip in blips)
    {
      // emit to client
    }
  }
}