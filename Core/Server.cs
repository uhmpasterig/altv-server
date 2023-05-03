using AltV.Net;
using server.Models;

using server.Handlers.Logger;
using server.Handlers.Event;
using server.Handlers.Timer;
using server.Handlers.Player;

using AltV.Net.Elements.Entities;
using AltV.Net.EntitySync;
using AltV.Net.EntitySync.SpatialPartitions;
using AltV.Net.EntitySync.ServerEvent;



namespace server.Core;

public class Server : IServer
{
  ILogger _logger = new Logger();
  IEventHandler _eventHandler;
  ITimerHandler _timerHandler;
  IPlayerHandler _playerHandler;
  PlayerContext _playerContext;

  public Server(ILogger logger, PlayerContext playerContext, IPlayerHandler playerHandler, IEventHandler eventHandler, ITimerHandler timerHandler)
  {
    _logger = logger;
    _playerContext = playerContext;
    _playerHandler = playerHandler;
    _eventHandler = eventHandler;
    _timerHandler = timerHandler;
  }

  public void Start()
  {
    _logger.Startup("Server startet...");

    _logger.Startup("Lade Handler...");
    _eventHandler.LoadHandlers();
    _logger.Startup("Handler Geladen!");

    _logger.Startup("Server gestartet!");

  }

  public async Task Stop()
  {
    _timerHandler.StopAllIntervals();

    foreach (IPlayer player in Alt.GetAllPlayers())
    {
      player.Kick("Server wurde gestoppt");
    }
  }
}