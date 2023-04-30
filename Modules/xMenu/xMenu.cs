using server.Events;
using server.Core;
using server.Models;
using server.Handlers.Entities;
using AltV.Net.Data;
using AltV.Net.Async;
using Newtonsoft.Json;

using server.Handlers.Vehicle;

namespace server.Modules.xMenu;

public class xMenu : ILoadEvent
{
  IVehicleHandler _vehicleHandler;
  public xMenu(IVehicleHandler vehicleHandler)
  {
    _vehicleHandler = vehicleHandler;
  }

  private async void ToggleLock(xPlayer player)
  {
    xVehicle veh = await _vehicleHandler.GetClosestVehicle(player.Position, 5);
    if (veh == null) return;
    if (!veh.hasControl(player)) return;

    veh.Locked = !veh.Locked;
    player.SendMessage("Vehicle", $"Das Fahrzeug ist nun {(veh.Locked ? "geschlossen" : "geöffnet")} [{veh.id}]", 5000, NOTIFYS.INFO);
  }
  private void ToggleEngine(xPlayer player) { }
  private async void ToggleTrunk(xPlayer player)
  {
    xVehicle veh = await _vehicleHandler.GetClosestVehicle(player.Position, 5);
    if (veh == null) return;
    if (!veh.hasControl(player)) return;

    veh.Trunk = !veh.Trunk;
    player.SendMessage("Vehicle", $"Das Fahrzeug ist nun {(veh.Trunk ? "geöffnet" : "geschlossen")} [{veh.id}]", 5000, NOTIFYS.INFO);
  }

  public void OnLoad()
  {
    /* player.SendMessage("Vehicle", "Du hast E gedrückt", 5000, NOTIFYS.INFO);
    player.SendMessage("Vehicle", "Du hast E gedrückt", 5000, NOTIFYS.ERROR); */

    AltAsync.OnClient<xPlayer>("ToggleLock", (player) =>
    {
      ToggleLock(player);
    });

    AltAsync.OnClient<xPlayer>("ToggleEngine", (player) =>
    {
      ToggleEngine(player);
    });

    AltAsync.OnClient<xPlayer>("ToggleTrunk", (player) =>
    {
      ToggleTrunk(player);
    });
  }
}