using server.Events;
using server.Core;
using server.Models;
using server.Handlers.Entities;
using AltV.Net.Data;
using AltV.Net.Async;
using Newtonsoft.Json;
using _logger = server.Logger.Logger;
using server.Handlers.Vehicle;

namespace server.Modules.xMenu;

public class xMenu : ILoadEvent
{
  IVehicleHandler _vehicleHandler = new VehicleHandler();
  private void ToggleLock(xPlayer player)
  {
    xVehicle veh = _vehicleHandler.GetClosestVehicle(player.Position, 5);
    if (veh == null) return;
    if (!veh.hasControl(player)) return;

    veh.Locked = !veh.Locked;
    player.SendMessage("Vehicle", $"Das Fahrzeug ist nun {(veh.Locked ? "geschlossen" : "geöffnet")} [{veh.vehicleId}]", 5000, NOTIFYS.INFO);
  }
  private void ToggleEngine(xPlayer player) { }
  private void ToggleTrunk(xPlayer player) { }

  public void OnLoad()
  {
    /* player.SendMessage("Vehicle", "Du hast E gedrückt", 5000, NOTIFYS.INFO);
    player.SendMessage("Vehicle", "Du hast E gedrückt", 5000, NOTIFYS.ERROR); */

    AltAsync.OnClient<xPlayer>("ToggleLock", (player) =>
    {
      _logger.Log("ToggleLock");
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