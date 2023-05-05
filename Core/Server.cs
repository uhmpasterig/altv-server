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
using server.Contexts;

namespace server.Core;

public class Server : IServer
{
  ILogger _logger = new Logger();
  IEventHandler _eventHandler;
  ITimerHandler _timerHandler;

  public Server(ILogger logger, IEventHandler eventHandler, ITimerHandler timerHandler)
  {
    _logger = logger;
    _eventHandler = eventHandler;
    _timerHandler = timerHandler;
  }

  public void Start()
  {
    _logger.Startup("Server initializing...");
    _eventHandler.LoadHandlers();

    _logger.Startup("Server is ready to accept connections!");
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