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

  // timer events 
  private readonly ITimerHandler _timerHandler;
  private readonly IEnumerable<IOneMinuteUpdateEvent> _oneMinuteUpdateEvents;

  ILogger _logger;
  public EventHandler(
    ILogger logger,
    ITimerHandler timerHandler,
    IEnumerable<IPlayerConnectEvent> playerConnectedEvents,
    IEnumerable<IPlayerDisconnectEvent> playerDisconnectEvents,
    IEnumerable<IPlayerDeadEvent> playerDeadEvents,
    IEnumerable<ILoadEvent> loadEvents,
    IEnumerable<IOneMinuteUpdateEvent> oneMinuteUpdateEvents
  )
  {
    _logger = logger;
    _playerConnectedEvents = playerConnectedEvents;
    _playerDisconnectedEvents = playerDisconnectEvents;
    _playerDeadEvents = playerDeadEvents;
    _loadEvents = loadEvents;
    _timerHandler = timerHandler;
    _oneMinuteUpdateEvents = oneMinuteUpdateEvents;
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

    _timerHandler.AddInterval(1000 * 60, async (s, e) =>
      _oneMinuteUpdateEvents?.ForEach(oneMinuteUpdateEvent => oneMinuteUpdateEvent.OnOneMinuteUpdate()));

    return Task.CompletedTask;
  }
}