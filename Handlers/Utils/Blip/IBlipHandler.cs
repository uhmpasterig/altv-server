using AltV.Net;
using server.Core;
using server.Models;
using AltV.Net.Data;

namespace server.Handlers.Blips;

public interface IBlipHandler
{
  public static Dictionary<int, Blip> blips { get; set; }
  public static void CreateBlip(Blip blip) { }
  public static void UpdateBlip(Blip blip) { }
  public static void DeleteBlip(int id) { }
  public void OnPlayerConnect(xPlayer player, string reason) { }
}