using server.Core;
using server.Events;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Entities;
using server.Util.ClothShop;
using Microsoft.EntityFrameworkCore;

namespace server.Modules.Clothing;

class ClothShopModule : ILoadEvent, IPressedEEvent
{
  ServerContext _serverContext = new ServerContext();
  public ClothShopModule()
  {
  }

  public static List<Cloth_Shop> allShops = new List<Cloth_Shop>();

  public async void OnLoad()
  {
    allShops = await _serverContext.Cloth_Shops.Include(s => s.cloth_items).ToListAsync();
    allShops.ForEach((shop) =>
    {
      Blip.Blip.Create("Kleidungsladen", 73, 37, .75f, shop.Position);
    });
  }

  public async Task<Cloth_Shop> GetShop(int id)
  {
    return await _serverContext.Cloth_Shops.Include(s => s.cloth_items).FirstOrDefaultAsync(s => s.id == id);
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    allShops.ForEach(shop =>
    {
      if (player.Position.Distance(shop.Position) > 2) return;
      player.Emit("frontend:open", "clothshop", new ClothShopWriter(shop));
    });
    return false;
  }

}