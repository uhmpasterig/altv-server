using System;
using AltV.Net.Elements.Entities;
using server.Core;
using AltV.Net.Resources.Chat.Api;
using server.Handlers.Vehicle;
using AltV.Net.Data;
using AltV.Net;

namespace server.Commands;

internal class AdminVehicle : IScript
{
  internal static IVehicleHandler _vehicleHandler = new VehicleHandler();

  [Command("car")]
  public static async void Car(xPlayer player, string model)
  {
    Position pos = player.Position;
    Rotation rot = player.Rotation;

    xVehicle veh = await _vehicleHandler.CreateVehicle(model, pos, rot);
    if (veh.Exists)
    {
      player.SendChatMessage("Spawned Vehicle: " + model);
    }
  }

  [Command("dv")]
  public static void DeleteVehicle(xPlayer player, int range = 5)
  {
    Position pos = player.Position;
    List<xVehicle> vehicles = _vehicleHandler.GetVehiclesInRadius(pos, range);

    vehicles.ForEach(delegate (xVehicle veh)
    {
      veh.Destroy();
    });
  }
}
