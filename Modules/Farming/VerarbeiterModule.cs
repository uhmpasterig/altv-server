using server.Core;
using server.Events;
using server.Handlers.Entities;
using server.Models;
using _logger = server.Logger.Logger;
using server.Handlers.Storage;
using server.Util.Farming;
using server.Handlers.Vehicle;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;

namespace server.Modules.Farming.Verarbeiter;

internal class ProcessData
{
  public xVehicle vehicle { get; set; }
  public xPlayer player { get; set; }
  public Farming_Processor verarbeiter { get; set; }
  public xStorage trunk { get; set; }
  public bool isRunning { get; set; } = true;
  public int stepsDone { get; set; } = 0;
  public int stepsToDo { get; set; } = 0;

  public void RemoveAndAddItems()
  {
    int amount = stepsDone * verarbeiter.ratio;
    bool hasEnough = trunk.RemoveItem(verarbeiter.inputitem, amount);
    if (!hasEnough) return;
    trunk.AddItem(verarbeiter.outputitem, stepsDone);
    vehicle.isAccesable = true;
    player.SendMessage($"Du hast {stepsDone}x {verarbeiter.outputitem} erhalten", NOTIFYS.INFO);
  }

  public ProcessData(xVehicle vehicle, xPlayer player, Farming_Processor verarbeiter, xStorage trunk, int stepsToDo = 1)
  {
    this.vehicle = vehicle;
    this.player = player;
    this.verarbeiter = verarbeiter;
    this.trunk = trunk;
    this.isRunning = true;
    this.stepsToDo = stepsToDo;
  }
}

public class VerarbeiterModule : ILoadEvent, IFiveSecondsUpdateEvent, IPressedEEvent
{
  public VerarbeiterModule()
  {
  }

  internal static IVehicleHandler _vehicleHandler = new VehicleHandler();
  internal static IStorageHandler _storageHandler = new StorageHandler();

  private List<Farming_Processor> _verarbeiter = new List<Farming_Processor>();
  private List<ProcessData> _processes = new List<ProcessData>();

  public async void ProcessTrunk(xVehicle vehicle, xPlayer player, int stepsToDo = 1)
  {
    xStorage trunk = await _storageHandler.GetStorage(vehicle.storage_id_trunk);
    if(_processes.Any(x => x.vehicle == vehicle)) return;
    Farming_Processor verarbeiter = _verarbeiter.Find(x => x.Position.Distance(player.Position) < 40)!;
    
    if(!trunk.HasItem(verarbeiter.inputitem, verarbeiter.ratio)) {
      player.SendMessage($"Du hast nicht genug {verarbeiter.inputitem} dabei", NOTIFYS.ERROR);
      return;
    };

    if (verarbeiter == null) return;
    vehicle.isAccesable = false;

    ProcessData processData = new ProcessData(vehicle, player, verarbeiter, trunk, stepsToDo);
    _processes.Add(processData);
    int timeInMilliseconds = 5000 * stepsToDo;
    int mins = timeInMilliseconds / 60000;
    int seconds = (timeInMilliseconds - (mins * 60000)) / 1000;
    player.SendMessage($"ETA: {mins} Minuten und {seconds} Sekunden", NOTIFYS.INFO);
    player.StartProgressBar(timeInMilliseconds);
  }

  public async Task<int> StepsVehicleCanDo(xVehicle vehicle)
  {
    xStorage trunk = await _storageHandler.GetStorage(vehicle.storage_id_trunk);
    int steps = 0;
    Farming_Processor verarbeiter = _verarbeiter.Find(x => x.Position.Distance(vehicle.Position) < 40)!;
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
    foreach (Farming_Processor verarbeiter in serverContext.Farming_Processors.ToList())
    {
      xEntity ped = new xEntity();
      ped.position = verarbeiter.Position;
      ped.dimension = (int)DIMENSIONEN.WORLD;
      ped.entityType = ENTITY_TYPES.PED;
      ped.range = 100;
      ped.data.Add("model", verarbeiter.ped);
      ped.data.Add("heading", verarbeiter.heading);

      ped.CreateEntity();

      _verarbeiter.Add(verarbeiter);
    }
    
    AltAsync.OnClient<IPlayer, int>("verarbeiter:verarbeitenVehId", async (iplayer, vehicleId) =>
    {
      xPlayer player = (xPlayer)iplayer;
      xVehicle vehicle = await _vehicleHandler.GetVehicle(vehicleId);
      if (vehicle == null) return;
      ProcessTrunk(vehicle, player, await StepsVehicleCanDo(vehicle));
    });
  }

  public async Task<bool> OnKeyPressE(xPlayer player)
  {
    if (player.IsDead) return false;
    foreach (Farming_Processor verarbeiter in _verarbeiter.ToList())
    {
      if (verarbeiter.Position.Distance(player.Position) < 2)
      {

        List<xVehicle> vehicles = new List<xVehicle>();
        foreach (xVehicle veh in await _vehicleHandler.GetVehiclesInRadius(player.Position, 20))
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