using System;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using server.Core;
using AltV.Net.Resources.Chat.Api;
using server.Handlers.Vehicle;
using AltV.Net.Data;
using AltV.Net;
using Newtonsoft.Json;
using _logger = server.Logger.Logger;
using server.Models;

namespace server.Commands;

internal class AdminVehicle : IScript
{
  internal static IVehicleHandler _vehicleHandler = new VehicleHandler();

  [Command("car")]
  public static async void Car(xPlayer player, string model)
  {
    Position pos = player.Position;
    Rotation rot = player.Rotation;

    xVehicle? veh = await _vehicleHandler.CreateVehicle(model, pos, rot);
    if (veh == null)
    {
      player.SendChatMessage("Invalid Model");
    }

    if (veh.Exists)
    {
      player.SendChatMessage("Spawned Vehicle: " + model);
    }
  }

  [Command("dv")]
  public async static void DeleteVehicle(xPlayer player, int range = 5)
  {
    Position pos = player.Position;
    List<xVehicle> vehicles = await _vehicleHandler.GetVehiclesInRadius(pos, range);

    foreach (xVehicle veh in vehicles)
    {
      veh.Destroy();
    }
  }

  [Command("repair")]
  public static void RepairVehicle(xPlayer player, int range = 5)
  {
    Position pos = player.Position;
    xVehicle veh = (xVehicle)player.Vehicle;

    if (veh.Exists)
    {
      veh.Repair();
    }
  }


  [Command("fulltune")]
  public static void FullTune(xPlayer player, int range = 5)
  {
    Position pos = player.Position;
    xVehicle veh = (xVehicle)player.Vehicle;

    player.SendChatMessage("Vehicle Exists: " + veh.Exists);
    if (veh.Exists)
    {
      player.SendChatMessage("Set Fulltune");
      _vehicleHandler.SetModByType(veh, VehicleModType.Horns, 20);
      _vehicleHandler.SetModByType(veh, VehicleModType.Spoilers, 2);
      _vehicleHandler.SetModByType(veh, VehicleModType.Engine, 3);
      _vehicleHandler.SetModByType(veh, VehicleModType.Transmission, 2);
      _vehicleHandler.SetModByType(veh, VehicleModType.Suspension, 3);
      _vehicleHandler.SetModByType(veh, VehicleModType.Turbo, 0);
      _vehicleHandler.SetModByType(veh, VehicleModType.Brakes, 2);
      _vehicleHandler.SetModByType(veh, VehicleModType.Plate, 3);
      _vehicleHandler.SetModByType(veh, VehicleModType.WindowTint, 2);
    }
  }

  [Command("coords")]
  public static void GetCoords(xPlayer player, int range = 5)
  {
    string pos = JsonConvert.SerializeObject(player.Position);
    string rot = JsonConvert.SerializeObject(player.Rotation);

    _logger.Info("Position: ");
    _logger.Log(pos);
    _logger.Info("Rotation: ");
    _logger.Log(rot);
    _logger.Info("Heading: ");
    _logger.Log(RotationMath.YawToHeading(player.Rotation.Yaw).ToString());

    if (player.Vehicle != null)
    {
      string vehPos = JsonConvert.SerializeObject(player.Vehicle.Position);
      string vehRot = JsonConvert.SerializeObject(player.Vehicle.Rotation);

      _logger.Info("Vehicle Position: ");
      _logger.Log(vehPos);
      _logger.Info("Vehicle Rotation: ");
      _logger.Log(vehRot);
    }
  }

  [Command("addshop")]
  public async static void AddShop(xPlayer player, string name, int type = 1)
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
  public async static void AddGarage(xPlayer player, string name, int type = 1)
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
    _serverContext.Garage.Add(garage);
    await _serverContext.SaveChangesAsync();
  }


  [Command("addgaragespawn")]
  public async static void AddGarageSpawn(xPlayer player, int garage_id = 1)
  {

    ServerContext _serverContext = new ServerContext();
    xVehicle veh = (xVehicle)player.Vehicle;

    if (veh.Exists)
    {
      Models.GarageSpawns spawn = new Models.GarageSpawns()
      {
        Position = player.Vehicle.Position,
        Rotation = player.Vehicle.Rotation,
        garage_id = garage_id
      };
      _serverContext.GarageSpawns.Add(spawn);
      await _serverContext.SaveChangesAsync();
    }
  }
}
