using server.Core;
using server.Events;
using server.Handlers.Entities;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Storage;
using server.Util.Farming;
namespace server.Modules.Farming.Verarbeiter;
using server.Handlers.Vehicle;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;

internal class ProcessData
{
  public xVehicle vehicle { get; set; }
  public xPlayer player { get; set; }
  public verarbeiter_farming_data verarbeiter { get; set; }
  public xStorage trunk { get; set; }
  public bool isRunning { get; set; } = true;
  public int stepsDone { get; set; } = 0;
  public int stepsToDo { get; set; } = 0;

  public void RemoveAndAddItems()
  {
    int amount = stepsDone / verarbeiter.ratio;
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

public class VerarbeiterMain : ILoadEvent, IFiveSecondsUpdateEvent, IPressedEEvent
{
  internal static IVehicleHandler _vehicleHandler = new VehicleHandler();
  internal static IStorageHandler _storageHandler = new StorageHandler();

  private List<verarbeiter_farming_data> _verarbeiter = new List<verarbeiter_farming_data>();
  private List<ProcessData> _processes = new List<ProcessData>();

  public async void ProcessTrunk(xVehicle vehicle, xPlayer player, int stepsToDo = 1)
  {
    xStorage trunk = await _storageHandler.GetStorage(vehicle.storageIdTrunk);
    vehicle.isAccesable = false;
    if(_processes.Any(x => x.vehicle == vehicle)) return;
    verarbeiter_farming_data verarbeiter = _verarbeiter.Find(x => x.Position.Distance(player.Position) < 40)!;
    if (verarbeiter == null) return;
    ProcessData processData = new ProcessData(vehicle, player, verarbeiter, trunk, stepsToDo);
    _processes.Add(processData);
    player.SendMessage("Verarbeitung gestartet", NOTIFYS.INFO);
    int timeInMin = 5000 * stepsToDo;
    timeInMin = timeInMin / 1000 / 60;
    player.SendMessage($"ETA: {timeInMin} Minuten", NOTIFYS.INFO);
  }

  public async Task<int> StepsVehicleCanDo(xVehicle vehicle)
  {
    xStorage trunk = await _storageHandler.GetStorage(vehicle.storageIdTrunk);
    int steps = 0;
    verarbeiter_farming_data verarbeiter = _verarbeiter.Find(x => x.Position.Distance(vehicle.Position) < 40)!;
    if (verarbeiter == null) return 0;
    int amount = trunk.GetItemAmount(verarbeiter.inputitem);
    if (amount == 0) return 0;
    steps = amount / verarbeiter.ratio;
    return steps;
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
      _logger.Exception(verarbeiter._pos);
      ped.position = verarbeiter.Position;
      ped.dimension = (int)DIMENSIONEN.WORLD;
      ped.entityType = ENTITY_TYPES.PED;
      ped.range = 100;
      ped.data.Add("model", "u_m_y_babyd");
      ped.CreateEntity();

      _verarbeiter.Add(verarbeiter);
      _logger.Debug($"Loaded Entity and Verarbeiter for {verarbeiter.name}");
    }
    AltAsync.OnClient<IPlayer, int>("verarbeiter:verarbeitenVehId", async (iplayer, vehicleId) =>
    {
      xPlayer player = (xPlayer)iplayer;
      xVehicle vehicle = _vehicleHandler.GetVehicle(vehicleId);
      if (vehicle == null) return;
      ProcessTrunk(vehicle, player, await StepsVehicleCanDo(vehicle));
    });
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    if (player.IsDead) return false;
    foreach (verarbeiter_farming_data verarbeiter in _verarbeiter.ToList())
    {
      if (verarbeiter.Position.Distance(player.Position) < 2)
      {

        List<xVehicle> vehicles = new List<xVehicle>();
        foreach (xVehicle veh in _vehicleHandler.GetVehiclesInRadius(player.Position, 20))
        {
          if (vehicles.Contains(veh)) continue;
          vehicles.Add(veh);
        }
        player.Emit("frontend:open", "verarbeiter", new verarbeiterWriter(vehicles));
        return true;

      }
    }
    return false;
  }
}