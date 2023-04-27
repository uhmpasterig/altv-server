using server.Core;
using server.Events;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Entities;
using server.Util.Shop;
using AltV.Net.Async;
using Newtonsoft.Json;
using server.Modules.Items;

namespace server.Modules.VehicleShop;

public enum VEHICLE_SHOP_TYPE : int
{
  LIMOUSINE = 0,
  SPORTWAGEN = 1,
  LKW = 2,
}

class ShopModule : ILoadEvent, IPressedEEvent
{
  ServerContext _serverContext = new ServerContext();
  public static List<Models.Shop> vehicleShopList = new List<Models.Shop>();

  public async void OnLoad()
  {
  }


  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    return false;
  }
}
