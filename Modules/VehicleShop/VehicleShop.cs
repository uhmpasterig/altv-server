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
    vehicleShopList.ForEach((shop) => {
      _logger.Error("Shop: " + shop.name);
      shop.Vehicles.ForEach(async (_vehicle) => {
        _logger.Error("Vehicle: " + _vehicle.model);
        xVehicle vehicle = await _vehicleHandler.CreateVehicle(_vehicle.model, _vehicle.Position, _vehicle.Rotation);
        vehicle.Frozen = true;
        vehicle.PrimaryColorRgb = new Rgba(255, 255, 255, 255);
        vehicle.SecondaryColorRgb = new Rgba(255, 255, 255, 255);
        vehicle.Locked = true;
        vehicle.Collision = false;

        shop.xVehicles.Add(vehicle);
      });
    });
  }


  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    return false;
  }
}
