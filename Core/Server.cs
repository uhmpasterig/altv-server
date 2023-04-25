using AltV.Net;
using server.Models;
using server.Handlers.Event;
using server.Handlers.Timer;
using server.Handlers.Vehicle;
using server.Handlers.Player;
using server.Handlers.Storage;
using AltV.Net.Elements.Entities;
using _logger = server.Logger.Logger;


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
    "weapon_specialcarbine_mk2",
    "weapon_specialcarbine",
    "weapon_pistol",
    "weapon_advancedrifle",
    "weapon_bullpuprifle",
    "weapon_bullpuprifle_mk2",
    "weapon_bat",
    "weapon_battleaxe"
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
    _logger.Startup("Server startet...");

    _logger.Startup("Lade Handler...");
    _eventHandler.LoadHandlers();
    _logger.Startup("Handler Geladen!");

    _logger.Startup("Lade Timer...");
    _logger.Startup("Timer Geladen!");
    foreach(Models.Player _player in _serverContext.Players.ToList())
    {
      _player.isOnline = false;
    }

    _serverContext.SaveChangesAsync();
    _logger.Startup("Server gestartet!");

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

    foreach (IPlayer player in Alt.GetAllPlayers())
    {
      player.Kick("Server wurde gestoppt");
    }
  }
}