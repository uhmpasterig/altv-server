using server.Core;
using AltV.Net;
using AltV.Net.Async;
using server.Events;

using server.Models;

using server.Util.Factions;

namespace server.Modules.Factions;

class FactionStorageModule : ILoadEvent
{
  public FactionStorageModule()
  {
  }
  public async void OnLoad()
  {
   /*  AltAsync.OnClient<xPlayer>("fraktion:lager:open", async (player) => {
      if(!player.player_society.FactionPerms.Contains("faction.storage") && !player.player_society.FactionPerms.Contains("faction.leader")) return;
      Faction faction = await FactionModule.GetFaction(player.player_society.faction_id);
      InventoryModule.OpenStorage(player, faction.storage_id);
    }); */
  }
}