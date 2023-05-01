using System;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using server.Core;
using AltV.Net.Resources.Chat.Api;
using server.Handlers.Vehicle;
using AltV.Net.Data;
using AltV.Net;
using Newtonsoft.Json;
using server.Handlers.Logger;
using server.Models;

namespace server.Commands;

public class AdminVehicle : IScript
{
  [Command("r")]
  public static void Revive(xPlayer player)
  {
    player.Revive();
  }

  [Command("coords")]
  public void GetCoords(xPlayer player, int range = 5)
  {
    string pos = JsonConvert.SerializeObject(player.Position);
    string rot = JsonConvert.SerializeObject(player.Rotation);

    Console.WriteLine("Position: ");
    Console.WriteLine(pos);
    Console.WriteLine("Rotation: ");
    Console.WriteLine(rot);
    Console.WriteLine("Heading: ");
    Console.WriteLine(RotationMath.YawToHeading(player.Rotation.Yaw).ToString());

    if (player.Vehicle != null)
    {
      string vehPos = JsonConvert.SerializeObject(player.Vehicle.Position);
      string vehRot = JsonConvert.SerializeObject(player.Vehicle.Rotation);

      Console.WriteLine("Vehicle Position: ");
      Console.WriteLine(vehPos);
      Console.WriteLine("Vehicle Rotation: ");
      Console.WriteLine(vehRot);
    }
  }

  [Command("addshop")]
  public async void AddShop(xPlayer player, string name, int type = 1)
  {
    ServerContext _serverContext = new ServerContext();
    Position pos = player.Position;
    Rotation rot = player.Rotation;
    pos.Z = pos.Z - 1;
    Models.Shop shop = new Models.Shop()
    {
      name = name,
      type = type,
      Position = player.Position,
      heading = RotationMath.YawToHeading(player.Rotation.Yaw)
    };
    _serverContext.Shops.Add(shop);
    await _serverContext.SaveChangesAsync();
  }

  [Command("addgarage")]
  public async void AddGarage(xPlayer player, string name, int type = 1)
  {
    ServerContext _serverContext = new ServerContext();
    Position pos = player.Position;
    Rotation rot = player.Rotation;
    pos.Z = pos.Z - 1;
    Models.Garage garage = new Models.Garage()
    {
      name = name,
      type = type,
      Position = player.Position,
      heading = RotationMath.YawToHeading(player.Rotation.Yaw)

    };
    _serverContext.Garages.Add(garage);
    await _serverContext.SaveChangesAsync();
  }


  [Command("addgaragespawn")]
  public async void AddGarageSpawn(xPlayer player, int garage_id = 1)
  {

    ServerContext _serverContext = new ServerContext();
    xVehicle veh = (xVehicle)player.Vehicle;

    if (veh.Exists)
    {
      Models.GarageSpawn spawn = new Models.GarageSpawn()
      {
        Position = player.Vehicle.Position,
        Rotation = player.Vehicle.Rotation,
        garage_id = garage_id
      };
      _serverContext.GarageSpawns.Add(spawn);
      await _serverContext.SaveChangesAsync();
    }
  }

  [Command("headoverlay")]
  public void HeadOverlay(xPlayer player, int overlayId, int overlayValue, float opacity = 1.0f)
  {
    player.SetHeadOverlay((byte)overlayId, (byte)overlayValue, opacity);
  }

  [Command("haircolor")]
  public void HairColor(xPlayer player, int colorId, int highlightColorId)
  {
    player.HairColor = (byte)colorId;
    player.HairHighlightColor = (byte)highlightColorId;
  }
}
