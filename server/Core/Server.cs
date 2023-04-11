using AltV.Net;
using server.Models;
using server.Handlers.Event;
using server.Handlers.Timer;
using server.Handlers.Vehicle;
using server.Handlers.Player;
using server.Handlers.Storage;
using AltV.Net.Elements.Entities;


namespace server.Core;

public class Server : IServer
{
  private readonly ServerContext _serverContext;
  private readonly IEventHandler _eventHandler;
  private readonly ITimerHandler _timerHandler;
  private readonly IVehicleHandler _vehicleHandler;
  private readonly IPlayerHandler _playerHandler;
  private readonly IStorageHandler _storageHandler;

  public static List<string> _serverWeapons = new List<string>() {
    "weapon_pistol",
    "weapon_pistol_mk2",
    "weapon_bat",
    "weapon_combatpistol",
    "weapon_pistol50",
  };

  public Server(ServerContext serverContext, IVehicleHandler vehicleHandler, IPlayerHandler playerHandler, IEventHandler eventHandler, ITimerHandler timerHandler, IStorageHandler storageHandler)
  {
    _serverContext = serverContext;
    _vehicleHandler = vehicleHandler;
    _playerHandler = playerHandler;
    _eventHandler = eventHandler;
    _timerHandler = timerHandler;
    _storageHandler = storageHandler;
  }

  public void Start()
  {
    Alt.Log("Server started");
    _eventHandler.LoadHandlers();

    foreach(Models.Player player in _serverContext.Player)
    {
      player.isOnline = false;
    }
    _serverContext.SaveChangesAsync();
  }

  public async Task SaveAll()
  {
    await _storageHandler.SaveAllStorages();
    await _vehicleHandler.SaveAllVehicles();
    await _playerHandler.SaveAllPlayers();
  }

  public async Task Stop()
  {
    _timerHandler.StopAllIntervals();
    await SaveAll();
    
    foreach(IPlayer player in Alt.GetAllPlayers())
    {
      player.Kick("Server wurde gestoppt");
    }
  }
}