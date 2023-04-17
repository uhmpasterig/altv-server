using server.Core;
using server.Events;
using server.Handlers.Event;
using server.Handlers.Entities;
using server.Models;
using _logger = server.Logger.Logger;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Data;
using server.Handlers.Storage;
using server.Handlers.Vehicle;
using Newtonsoft.Json;

namespace server.Modules.Farming.Verarbeiter;

internal class ProcessData {
  public xVehicle vehicle { get; set; }
  public xPlayer player { get; set; }
  public verarbeiter_farming_data verarbeiter { get; set; }
  public xStorage trunk { get; set; }
  public bool isRunning { get; set; } = true;
  public int stepsDone { get; set; } = 0;
  public int stepsToDo { get; set; } = 0;

  public void RemoveAndAddItems() {
    int amount = stepsDone * verarbeiter.ratio;
    bool hasEnough = trunk.RemoveItem(verarbeiter.inputitem, amount);
    if (!hasEnough) return;
    trunk.AddItem(verarbeiter.outputitem, amount);
    vehicle.isAccesable = true;
    player.SendMessage("Verarbeitung abgeschlossen", NOTIFYS.INFO);
    player.SendMessage($"Du hast {amount}x {verarbeiter.outputitem} erhalten", NOTIFYS.INFO);
  }

  public ProcessData(xVehicle vehicle, xPlayer player, verarbeiter_farming_data verarbeiter, xStorage trunk, int stepsToDo = 1)
  {
    this.vehicle = vehicle;
    this.player = player;
    this.verarbeiter = verarbeiter;
    this.trunk = trunk;
    this.isRunning = true;
    this.stepsToDo = stepsToDo;
  }
}

public class VerarbeiterMain : ILoadEvent, IFiveSecondsUpdateEvent, IPlayerDeadEvent
{
  private List<verarbeiter_farming_data> _verarbeiter = new List<verarbeiter_farming_data>();
  private List<ProcessData> _processes = new List<ProcessData>();
  
  public async void ProcessTrunk(xVehicle vehicle, xPlayer player, int stepsToDo = 1)
  {
    IStorageHandler _storageHandler = new StorageHandler();
    xStorage trunk = await _storageHandler.GetStorage(vehicle.storageIdTrunk);
    vehicle.isAccesable = false;
    verarbeiter_farming_data verarbeiter = _verarbeiter.Find(x => x.Position.Distance(player.Position) < 100000)!;
    _logger.Log("Player in verarbeiter is dead3");
    if (verarbeiter == null) return;
    _logger.Log("Player in verarbeiter is dead4");
    ProcessData processData = new ProcessData(vehicle, player, verarbeiter, trunk, stepsToDo);
    _processes.Add(processData);
    player.SendMessage("Verarbeitung gestartet", NOTIFYS.INFO);
    int timeInMin = 5000 * stepsToDo;
    timeInMin = timeInMin / 1000 / 60;
    player.SendMessage($"ETA: {timeInMin} Minuten", NOTIFYS.INFO);
  }

  public async void OnFiveSecondsUpdate()
  {
    foreach (ProcessData processData in _processes.ToList())
    {
      if (!processData.isRunning) continue;
      processData.stepsDone++;
      _logger.Log($"Verarbeiter: {processData.stepsDone}/{processData.stepsToDo}");
      if (processData.stepsDone >= processData.stepsToDo)
      {
        processData.isRunning = false;
        processData.RemoveAndAddItems();
        processData.stepsDone = 0;
        processData.stepsToDo = 0;
        _processes.Remove(processData);
        continue;
      }
    }
  }

  public async void OnLoad()
  {
    await using ServerContext serverContext = new ServerContext();
    _logger.Startup("Verarbeiter loaded");
    foreach (verarbeiter_farming_data verarbeiter in serverContext.verarbeiter_farming_data)
    {
      xEntity ped = new xEntity();
      ped.position = verarbeiter.Position;
      ped.dimension = (int)DIMENSIONEN.WORLD;
      ped.entityType = ENTITY_TYPES.PED;
      ped.range = 100;
      ped.data.Add("model", "u_m_y_babyd");
      _verarbeiter.Add(verarbeiter);
      _logger.Debug($"Loaded Entity and Verarbeiter for {verarbeiter.name}");
    }
  }

  public async void OnPlayerDeath(IPlayer iplayer, IEntity killer, uint weapon)
  {
    IVehicleHandler _vehicleHandler = new VehicleHandler();
    xPlayer player = (xPlayer)iplayer;
    xVehicle vehicle = _vehicleHandler.GetVehicle(1);
    _logger.Log("Player in verarbeiter is dead");
    if (vehicle == null) return;
    _logger.Log("Player in verarbeiter is dead2");
    ProcessTrunk(vehicle, player, 2);
  }
}