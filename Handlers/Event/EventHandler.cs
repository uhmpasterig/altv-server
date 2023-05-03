using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using server.Handlers.Timer;
using server.Core;
using server.Events;
using server.Extensions;
using server.Handlers.Logger;

using System.Diagnostics;
using System.Threading.Tasks;

namespace server.Handlers.Event;

public class EventHandler : IEventHandler
{
  private readonly IEnumerable<IPlayerConnectEvent> _playerConnectedEvents;
  private readonly IEnumerable<IPlayerDisconnectEvent> _playerDisconnectedEvents;
  private readonly IEnumerable<IPlayerDeadEvent> _playerDeadEvents;
  private readonly IEnumerable<ILoadEvent> _loadEvents;

  // timer event

  ILogger _logger;
  public EventHandler(
    ILogger logger,
    ITimerHandler timerHandler,
    IEnumerable<IPlayerConnectEvent> playerConnectedEvents,
    IEnumerable<IPlayerDisconnectEvent> playerDisconnectEvents,
    IEnumerable<IPlayerDeadEvent> playerDeadEvents,
    IEnumerable<ILoadEvent> loadEvents
  )
  {
    _logger = logger;
    _playerConnectedEvents = playerConnectedEvents;
    _playerDisconnectedEvents = playerDisconnectEvents;
    _playerDeadEvents = playerDeadEvents;
    _loadEvents = loadEvents;
  }

  public Task LoadHandlers()
  {
    foreach (var loadEvent in _loadEvents)
    {
      _logger.Debug($"Loading event handler: {loadEvent.GetType().Name}");
      loadEvent.OnLoad();
    }
    _logger.Debug("Loading event handlers");

    AltAsync.OnPlayerConnect += async (IPlayer player, string reason) =>
      _playerConnectedEvents?.ForEach(playerConnectEvent => playerConnectEvent.OnPlayerConnect((xPlayer)player, reason));

    AltAsync.OnPlayerDisconnect += async (IPlayer player, string reason) =>
      _playerDisconnectedEvents?.ForEach(playerDisconnectEvent => playerDisconnectEvent.OnPlayerDisconnect((xPlayer)player, reason));

    AltAsync.OnPlayerDead += async (IPlayer player, IEntity killer, uint weapon) =>
      _playerDeadEvents?.ForEach(playerDeadEvent => playerDeadEvent.OnPlayerDeath(player, killer, weapon));

    return Task.CompletedTask;
  }
}