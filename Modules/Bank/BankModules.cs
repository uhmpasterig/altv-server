using server.Core;
using AltV.Net;
using server.Events;
using server.Handlers.Event;
using server.Models;
using _logger = server.Logger.Logger;

namespace server.Modules.Banking;

class BankingModuleMain : ILoadEvent
{
  public async void OnLoad()
  {
    _logger.Startup("BANKEN werden geladen... ACH FICK MICH DOCH");
  }

}
