using System;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using server.Core;
using AltV.Net.Resources.Chat.Api;
using server.Handlers.Vehicle;
using AltV.Net.Data;
using AltV.Net;
using _logger = server.Logger.Logger;

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

    foreach(xVehicle veh in vehicles){
      veh.Destroy();
    }
  }

  [Command("repair")]
  public static void RepairVehicle(xPlayer player, int range = 5)
  {
    Position pos = player.Position;
    xVehicle veh = (xVehicle)player.Vehicle;

    if(veh.Exists){
      veh.Repair();
    }
  }

  [Command("fulltune")]
  public static void FullTune(xPlayer player, int range = 5)
  {
    Position pos = player.Position;
    xVehicle veh = (xVehicle)player.Vehicle;

    player.SendChatMessage("Vehicle Exists: "+veh.Exists);
    if(veh.Exists){
      player.SendChatMessage("Set Vehicle ModType.Horns");
      bool isSet = _vehicleHandler.SetModByType(veh, VehicleModType.Horns, 20);
      veh.SetWheelOnFire(0, true);
      veh.SetWheelOnFire(1, true);
      player.SendChatMessage("Set Vehicle Mod: "+isSet);
    }
  }
}
