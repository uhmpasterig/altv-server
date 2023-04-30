/* using server.Core;
using server.Events;
using server.Handlers.Entities;
using server.Models;

using server.Handlers.Storage;
using server.Util.Workstation;
using server.Handlers.Player;
using Microsoft.EntityFrameworkCore;
using AltV.Net.Data;
using AltV.Net.Async;

namespace server.Modules.Workstation;

internal class ProcessData
{
}

public class WorkstationModule : ILoadEvent, IOneMinuteUpdateEvent, IPressedEEvent
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

    AltAsync.OnClient<xPlayer, int>("workstation:startProcess", StartProgress);
    AltAsync.OnClient<xPlayer>("workstation:cancelProcess", StopProgress);
  }

  public async void OnOneMinuteUpdate()
  {
    PlayerHandler.Players.ToList().ForEach(async (kvp) =>
    {
      xPlayer player = kvp.Value;
      if (player.player_factory.selected_process == -1) return;
      xStorage inputStorage = await _storageHandler.GetStorage(kvp.Value.boundStorages["Warenannahme Fabrik"]);
      xStorage outputStorage = await _storageHandler.GetStorage(kvp.Value.boundStorages["Warenausgabe Fabrik"]);
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

  public async Task<bool> ContainsItem(xPlayer player, xStorage _in, xStorage _out, Factory_Processes process)
  {
    bool hasEnoughItems = true;

    process.inputItemsList.ForEach((inputItem) =>
    {
      if (_in.GetItemAmount(inputItem.item) < inputItem.amount)
      {
        player.SendMessage("Du hast nicht genug " + inputItem.item + " um diesen Prozess zu starten. Die Produktion der Fabrik ist pausiert!", NOTIFYS.ERROR);
        hasEnoughItems = false;
        player.player_factory.selected_process = -1;
      };
    });

    if ((_out.slots <= _out.items.Count) ||
              (_out.maxWeight <= _out.weight + process.weightNeeded))
    {
      player.SendMessage("Deine Fabrik ist Voll. Die Produktion der Fabrik ist pausiert!", NOTIFYS.ERROR);
      player.player_factory.selected_process = -1;
      hasEnoughItems = false;
    };
    return hasEnoughItems;
  }

  public async void ProcessItem(xPlayer player, xStorage inputStorage, xStorage outputStorage, Factory_Processes process)
  {
    if (!await ContainsItem(player, inputStorage, outputStorage, process)) return;
    process.inputItemsList.ForEach((inputItem) =>
    {
      inputStorage.RemoveItem(inputItem.item, inputItem.amount);
    });
    process.outputItemsList.ForEach((outputItem) =>
    {
      outputStorage.AddItem(outputItem.item, outputItem.amount);
    });
  }

  public async void StartProgress(xPlayer player, int processId)
  {
    xStorage inputStorage = await _storageHandler.GetStorage(player.boundStorages["Warenannahme Fabrik"]);
    xStorage outputStorage = await _storageHandler.GetStorage(player.boundStorages["Warenausgabe Fabrik"]);
    Factory_Processes? process = factoryProcesses.FirstOrDefault(x => x.id == processId);
    if (process == null) return;
    if (inputStorage == null || outputStorage == null) return;
    if (!await ContainsItem(player, inputStorage, outputStorage, process)) return;
    player.player_factory.selected_process = processId;
    player.player_factory.ticksDone = 0;
    player.SendMessage($"Die Produktion der Fabrik wurde gestartet! Das herstellen dauert pro Item: {process.ticksPerProcess} Minuten", NOTIFYS.INFO);
  }

  public async void StopProgress(xPlayer player)
  {
    player.player_factory.selected_process = -1;
    player.player_factory.ticksDone = 0;
    player.SendMessage("Die Produktion der Fabrik wurde pausiert!", NOTIFYS.INFO);
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    if (player.Position.Distance(factoryPosition) > 2) return false;
    player.Emit("frontend:open", "workstation", new WorkStationWriter(factoryProcesses, player.player_factory.selected_process));
    return true;
  }
} */