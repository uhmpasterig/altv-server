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
  public static async void Execute(xPlayer player, string model)
  {   
    Position pos = player.Position;
    Rotation rot = player.Rotation;

    xVehicle veh = await _vehicleHandler.CreateVehicle(model, pos, rot);
    if(veh.Exists){
      player.SendChatMessage("Spawned Vehicle: "+model);
    }
  }


}
