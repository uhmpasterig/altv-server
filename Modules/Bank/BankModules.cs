using server.Core;
using server.Events;
using server.Models;

using server.Handlers.Entities;

using server.Enums;
using server.Handlers.Logger;
namespace server.Modules.Banking;

class BankModuleMain : ILoadEvent
{
  ILogger _logger;
  public BankModuleMain(ILogger logger)
  {
    _logger = logger;
  }

  ServerContext _serverContext = new ServerContext();
  public static List<Models.Bank> bankList = new List<Models.Bank>();

  public async void OnLoad()
  {
    foreach (Models.Bank bank in _serverContext.Banks)
    {
      xEntity ped = new xEntity();
      ped.position = bank.Position;
      ped.dimension = (int)DIMENSIONEN.WORLD;
      ped.entityType = ENTITY_TYPES.PED;
      ped.range = 100;
      ped.data.Add("model", bank.ped);
      ped.data.Add("heading", bank.heading);
      ped.CreateEntity();
      _logger.Exception($"Bank {bank.name} created");
      Blip.Blip.Create("Bank", 108, 2, .75f, bank.Position);

      bankList.Add(bank);
    }
  }
}