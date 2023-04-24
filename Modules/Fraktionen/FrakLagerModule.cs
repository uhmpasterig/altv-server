using server.Core;
using AltV.Net;
using AltV.Net.Async;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;
using server.Util.Fraktionen;
using server.Modules.Inventory;

namespace server.Modules.Fraktionen;

class FrakLagerModule : ILoadEvent
{
  public async void OnLoad()
  {
    AltAsync.OnClient<xPlayer>("fraktion:lager:open", async (player) => {
      if(!player.job_perm.Contains("faction.storage") && !player.job_perm.Contains("faction.leader")) return;
      Fraktion fraktion = FraktionsModuleMain.GetFrak(player.job);
      InventoryModule.OpenStorage(player, fraktion.storage_id);
    });
  }
}