using server.Core;
using server.Events;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Entities;
using server.Util.ClothShop;
using Microsoft.EntityFrameworkCore;
using AltV.Net.Async;

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

    AltAsync.OnClient<xPlayer, int>("clothshop:tryItem", TryOnPiece);
    AltAsync.OnClient<xPlayer>("clothshop:close", ResetPlayerCloth);
    AltAsync.OnClient<xPlayer, int>("clothshop:buyItem", BuyPiece);
  }

  public async void BuyPiece(xPlayer player, int cloth_id)
  {
    _logger.Log("BuyPiece");
    Cloth? cloth = ClothModule.GetCloth(cloth_id);
    if (cloth == null) return;
    string categoryName = ClothModule.GetCategoryName(cloth.component);
    if (!await player.HasMoney(cloth.price)) return;
    player.RemoveMoney(cloth.price);
    player.player_cloth.SetPiece(categoryName, cloth.id);
    player.SaveMoney();
    await player.LoadClothes(player.player_cloth);
  }

  public async void TryOnPiece(xPlayer player, int cloth_id)
  {
    _logger.Log("TryOnPiece");
    Cloth? cloth = ClothModule.GetCloth(cloth_id);
    if (cloth == null) return;
    _logger.Log("Cloth not null");
    player.SetDlcClothes(cloth.component, cloth.drawable, cloth.texture, cloth.palette, cloth.dlc);
  }

  public async void ResetPlayerCloth(xPlayer player)
  {
    await player.LoadClothes(player.player_cloth);
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