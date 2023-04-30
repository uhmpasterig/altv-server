using server.Core;
using AltV.Net;
using AltV.Net.Async;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using server.Util.Factions;
using server.Config;

namespace server.Modules.Factions;

class FactionShopModule : ILoadEvent
{
  public FactionShopModule()
  {
  }

  async void BuyItem(xPlayer player, string name, int price)
  {
    bool hasMoney = await player.HasMoney(price);
    if (!hasMoney) return;

    bool hasSpace = await player.GiveItem(name, 1);
    if(!hasSpace) return;
    player.RemoveMoney(price);
  }

  public async void OnLoad()
  {
    AltAsync.OnClient<xPlayer, string>("faction:shop:buyItem", async (player, itemName) =>
    {
      if (itemName == "weapon_pistol_mk2") {
        BuyItem(player, itemName, Faction_Shop_Prices.main_weapon);
      } else if(itemName == player.player_society.Faction.weapon) {
        BuyItem(player, itemName, Faction_Shop_Prices.meele_weapon);
      } else {
        _logger.Log($"CHEATER!!!");
        return;
      }
    });
  }
}