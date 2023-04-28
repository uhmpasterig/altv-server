using server.Core;
using server.Events;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Entities;
using server.Util.Shop;
using AltV.Net.Async;
using Newtonsoft.Json;
using server.Modules.Items;
using Microsoft.EntityFrameworkCore;
using server.Handlers.Vehicle;
using AltV.Net.Data;

namespace server.Modules.VehicleShop;

public enum VEHICLE_SHOP_TYPE : int
{
  LIMOUSINE = 0,
  SPORTWAGEN = 1,
  LKW = 2,
}

class ShopModule : ILoadEvent, IPressedEEvent
{
  IVehicleHandler _vehicleHandler = new VehicleHandler();
  ServerContext _serverContext = new ServerContext();
  public static List<Models.Vehicle_Shop> vehicleShopList = new List<Models.Vehicle_Shop>();

  public async void OnLoad()
  {
    _logger.Info("Loading Vehicle Shops...");
    vehicleShopList = await _serverContext.Vehicle_Shops.Include(v => v.Vehicles).ToListAsync();
    vehicleShopList.ForEach((shop) =>
    {
      _logger.Error("Shop: " + shop.name);
      shop.Vehicles.ForEach(async (_vehicle) =>
      {
        _logger.Error("Vehicle: " + _vehicle.model);
        xVehicle vehicle = await _vehicleHandler.CreateVehicle(_vehicle.model, _vehicle.Position, _vehicle.Rotation);
        vehicle.NumberplateText = "SHOP VEH";
        vehicle.Frozen = true;
        vehicle.PrimaryColorRgb = new Rgba(255, 255, 255, 255);
        vehicle.SecondaryColorRgb = new Rgba(0, 0, 0, 255);
        vehicle.Locked = true;
        vehicle.Collision = false;
        shop.xVehicles.Add(vehicle);
      });
      Blip.Blip.Create("Auto Händler", 326, 83, .75f, shop.Position);
    });
  }

  async void CreateText(string _text, Position position, string color = "WHITE")
  {
    xEntity text = new xEntity();
    text.position = position;
    text.dimension = (int)DIMENSIONEN.WORLD;
    text.entityType = ENTITY_TYPES.THREED_TEXT;
    text.range = 7;
    text.data.Add("color", color);
    text.data.Add("text", _text);
    text.CreateEntity();
  }

  public async void CreateTextForVehicle(Vehicle_Shop_Vehicle vehicle)
  {
    Position position = vehicle.Position + new Position(0, 0, 1.5f);
    CreateText("Fahrzeug:", position, "PURPLE");
    CreateText(vehicle.model, position - new Position(0,0,.1f));
    CreateText("Preis:", position - new Position(0,0,.2f), "GREEN");
    CreateText(vehicle.price.ToString(), vehicle.Position + new Position(0, 0, .3f));

    CreateText("Lager:", position - -new Position(0,0,.4f), "CYAN");
    CreateText($"Slots: {vehicle.slots}", position - new Position(0,0,.5f));
    CreateText($"Kilogramm: {vehicle.maxWeight}", position - new Position(0,0,.6f));
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    return false;
  }
}
