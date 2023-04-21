using server.Core;
using server.Events;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Entities;
using server.Util.Shop;
using AltV.Net.Async;
using Newtonsoft.Json;

namespace server.Modules.Shop;

enum SHOP_TYPES
{
  ITEMSHOP = 1,
  WEAPONSHOP = 2,
  TOOLSHOP = 3
}

enum SHOP_SPRITES : int
{
  ITEMSHOP = 52,
  WEAPONSHOP = 110,
  TOOLSHOP = 566
}

enum SHOP_COLORS : int
{
  ITEMSHOP = 2,
  WEAPONSHOP = 1,
  TOOLSHOP = 81
}

class SHOP_NAMES
{
  static Dictionary<string, string> _names = new Dictionary<string, string>()
  {
    { "ITEMSHOP", "Supermarkt" },
    { "WEAPONSHOP", "Waffenladen" },
    { "TOOLSHOP", "Baumarkt" }
  };

  public static string GetName(string name)
  {
    return _names[name];
  }
}

class GaragenModule : ILoadEvent, IPressedEEvent
{
  ServerContext _serverContext = new ServerContext();
  public static List<Models.Shop> shopList = new List<Models.Shop>();

  public static Dictionary<string, int> GetShopBlipByType(int type)
  {
    string typeName = Enum.GetName(typeof(SHOP_TYPES), type)!;

    Dictionary<string, int> dict = new Dictionary<string, int>();
    dict.Add("sprite", (int)Enum.Parse(typeof(SHOP_SPRITES), typeName));
    dict.Add("color", (int)Enum.Parse(typeof(SHOP_COLORS), typeName));
    return dict;
  }

  public async void OnLoad()
  {
    foreach (Models.Shop shop in _serverContext.Shop.ToList())
    {
      foreach (Models.ShopItems shopItem in _serverContext.ShopItems.Where(x => x.type == shop.type).ToList())
      {
        shopItem.price = (int)(shopItem.price * shop.tax);
        shop.items.Add(shopItem);
      }

      xEntity ped = new xEntity();
      ped.position = shop.Position;
      ped.dimension = (int)DIMENSIONEN.WORLD;
      ped.entityType = ENTITY_TYPES.PED;
      ped.range = 100;
      ped.data.Add("model", shop.ped);
      ped.data.Add("heading", shop.heading);
      ped.CreateEntity();

      Dictionary<string, int> blip = GetShopBlipByType(shop.type);
      string name = SHOP_NAMES.GetName(Enum.GetName(typeof(SHOP_TYPES), shop.type)!);
      shop.typeString = name;

      Blip.Blip.Create(name,
        blip["sprite"], blip["color"], 1, shop.Position);

      shopList.Add(shop);
    }

    AltAsync.OnClient<xPlayer, string>("frontend:shop:buyWarenkorb", async (player, warenkorb) => {
      CheckOut(player, warenkorb);
    });
  }

  public async void CheckOut(xPlayer player, string warenkorb)
  {
    List<Models.ShopItems>? items = JsonConvert.DeserializeObject<List<Models.ShopItems>>(warenkorb);
    if(items == null) return;
    foreach(Models.ShopItems _item in items)
    {
      Models.ShopItems? item = shopList.Find(x => x.Position.Distance(player.Position) < 10).items.Find(x => x.id == _item.id);
      if(item == null) continue;
      bool hasMoney = await player.HasMoney(item.price * _item.count); 
      if(!hasMoney) {
        _logger.Log($"{player.Name} hat nicht genug Geld für {item.item} ({item.price})");
        continue;
      };

      bool hasSpace = await player.GiveItem(item.item, _item.count);
      if(!hasSpace) {
        _logger.Log($"{player.Name} hat nicht genug Platz für {item.item} ({_item.count})");
        continue;
      };
      player.RemoveMoney(item.price * _item.count);
      _logger.Log($"{player.Name} hat {item.item} ({_item.count}) für {item.price * _item.count} gekauft");
    }
    _logger.Log($"{player.Name} hat den Warenkorb gekauft");
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    foreach (Models.Shop shop in shopList.ToList())
    {
      if (shop.Position.Distance(player.Position) < 2)
      {
        player.Emit("frontend:open", "shop", new shopWriter(shop));
        return true;
      }
    }
    return false;
  }
}
