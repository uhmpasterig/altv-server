using server.Core;
using AltV.Net;
using AltV.Net.Async;
using server.Events;

using server.Models;

using server.Util.Factions;
using server.Config;
using server.Handlers.Logger;

namespace server.Modules.Factions;

class FactionShopModule : ILoadEvent
{
  ILogger _logger;
  public FactionShopModule(ILogger logger)
  {
    _logger = logger;
  }

  async void BuyItem(xPlayer player, string name, int price)
  {
    bool hasMoney = await player.HasMoney(price);
    if (!hasMoney) return;
    //TODO GIVE ITEM
    /* bool hasSpace = await player.GiveItem(name, 1);
    if(!hasSpace) return; */
    player.RemoveMoney(price);
  }

  public async void OnLoad()
  {
    AltAsync.OnClient<xPlayer, string>("faction:shop:buyItem", async (player, itemName) =>
    {
      if (itemName == "weapon_pistol_mk2")
      {
        BuyItem(player, itemName, Faction_Shop_Prices.main_weapon);
      }
      else if (itemName == player.player_society.Faction.weapon)
      {
        BuyItem(player, itemName, Faction_Shop_Prices.meele_weapon);
      }
      else
      {
        _logger.Log($"CHEATER!!!");
        return;
      }
    });
  }
}