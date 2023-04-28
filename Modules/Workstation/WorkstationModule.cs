using server.Core;
using server.Events;
using server.Handlers.Entities;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Storage;
using server.Util.Farming;
using server.Handlers.Player;
using Microsoft.EntityFrameworkCore;
using AltV.Net.Data;
using server.Modules.Inventory;

namespace server.Modules.Workstation;

internal class ProcessData
{
}

public class WorkstationModule : ILoadEvent, IFiveSecondsUpdateEvent, IPressedEEvent
{
  public WorkstationModule()
  {
  }

  internal static Position factoryPosition = new Position(890.0176f, -2490.1187f, 27.454224f);
  internal static IPlayerHandler _playerHandler = new PlayerHandler();
  internal static IStorageHandler _storageHandler = new StorageHandler();
  internal static List<Factory_Processes> factoryProcesses = new List<Factory_Processes>();

  public async void OnLoad()
  {
    ServerContext _serverContext = new ServerContext();
    xEntity ped = new xEntity();
    ped.position = factoryPosition;
    ped.dimension = (int)DIMENSIONEN.WORLD;
    ped.entityType = ENTITY_TYPES.PED;
    ped.range = 80;
    ped.data.Add("model", "csb_g");
    ped.data.Add("heading", 0);
    ped.CreateEntity();

    factoryProcesses = await _serverContext.Factory_Processes.ToListAsync();
  }

  public async void OnFiveSecondsUpdate()
  {
    PlayerHandler.Players.ToList().ForEach(async (kvp) =>
    {
      xPlayer player = kvp.Value;
      if(player.player_factory.selected_process == -1) return;
      xStorage inputStorage = await _storageHandler.GetStorage(kvp.Value.boundStorages["Fabrik Input"]);
      xStorage outputStorage = await _storageHandler.GetStorage(kvp.Value.boundStorages["Fabrik Output"]);
      int selected_process = kvp.Value.player_factory.selected_process;
      Factory_Processes? process = factoryProcesses.FirstOrDefault(x => x.id == selected_process);
      if (process == null) return;
      if (inputStorage == null || outputStorage == null) return;
      player.player_factory.ticksDone++;

      if (player.player_factory.ticksDone >= process.ticksPerProcess)
      {
        player.player_factory.ticksDone = 0;
        ProcessItem(player, inputStorage, outputStorage, process);
      }
    });
  }

  public async void ProcessItem(xPlayer player, xStorage inputStorage, xStorage outputStorage, Factory_Processes process)
  {
    #region Checks & Notifys
    bool hasEnoughItems = true;
    process.inputItemsList.ForEach((inputItem) =>
    {
      if (inputStorage.GetItemAmount(inputItem.item) < inputItem.amount)
      {
        player.SendMessage("Du hast nicht genug " + inputItem.item + " um diesen Prozess zu starten. Die Produktion der Fabrik ist pausiert!", NOTIFYS.ERROR);
        hasEnoughItems = false;
        player.player_factory.selected_process = -1;
      };
    });
    if (!hasEnoughItems) goto end;
    if ((outputStorage.slots <= outputStorage.items.Count) ||
              (outputStorage.maxWeight <= outputStorage.weight + process.weightNeeded))
    {
      player.SendMessage("Deine Fabrik ist Voll. Die Produktion der Fabrik ist pausiert!", NOTIFYS.ERROR);
      player.player_factory.selected_process = -1;
      goto end;
    };
    #endregion
    process.inputItemsList.ForEach((inputItem) =>
    {
      inputStorage.RemoveItem(inputItem.item, inputItem.amount);
    });
    process.outputItemsList.ForEach((outputItem) =>
    {
      outputStorage.AddItem(outputItem.item, outputItem.amount);
    });

  end: return;
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    return true;
  }
}