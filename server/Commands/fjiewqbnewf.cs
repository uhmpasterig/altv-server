using System;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using server.Handlers.Vehicle;
using AltV.Net.Data;

namespace server.Commands;

class fjiewqbnewf
{
  internal static IVehicleHandler _vehicleHandler = new VehicleHandler();

  [Command("fjiewqbnewf")]
  public static void Execute(IPlayer player, string[] args)
  {
    _vehicleHandler.CreateVehicle("adder", new Position(0, 0, 0), new Rotation(0, 0, 0));
    player.SendChatMessage("fjiewqbnewf");
  }
}
