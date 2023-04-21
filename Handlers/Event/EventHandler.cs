using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using server.Handlers.Timer;
using server.Core;
using server.Events;
using server.Extensions;
using _logger = server.Logger.Logger;

namespace server.Handlers.Event;

public class EventHandler : IEventHandler
{
  private readonly ITimerHandler _timerHandler;
  private readonly IEnumerable<IPlayerConnectEvent> _playerConnectedEvents;
  private readonly IEnumerable<IPlayerDisconnectEvent> _playerDisconnectedEvents;
  private readonly IEnumerable<IPlayerDeadEvent> _playerDeadEvents;
  private readonly IEnumerable<ILoadEvent> _loadEvents;

  public readonly IEnumerable<IItemsLoaded> _itemsLoadedEvent;

  // Timer event
  private readonly IEnumerable<IFiveSecondsUpdateEvent> _fiveSecondsUpdateEvents;

  // keypress event
  private readonly IEnumerable<IPressedEEvent> _pressedEEvents;
  private readonly IEnumerable<IPressedIEvent> _pressedIEvents;

  // timer event


  public EventHandler(ITimerHandler timerHandler,
                      IEnumerable<IPlayerConnectEvent> playerConnectedEvents,
                      IEnumerable<IPlayerDisconnectEvent> playerDisconnectEvents,
                      IEnumerable<IPlayerDeadEvent> playerDeadEvents,
                      IEnumerable<ILoadEvent> loadEvents,
                      IEnumerable<IItemsLoaded> itemsLoadedEvent,
                      IEnumerable<IFiveSecondsUpdateEvent> fiveSecondsUpdateEvents,

                      IEnumerable<IPressedEEvent> pressedEEvents,
                      IEnumerable<IPressedIEvent> pressedIEvents
                      )
  {
    AltAsync.OnClient<IPlayer>("PressE", OnKeyPressE);
    AltAsync.OnClient<IPlayer>("PressI", OnKeyPressI);
    AltAsync.OnServer("ItemsLoaded", ItemsLoaded);
    _timerHandler = timerHandler;
    _playerConnectedEvents = playerConnectedEvents;
    _playerDisconnectedEvents = playerDisconnectEvents;
    _playerDeadEvents = playerDeadEvents;
    _loadEvents = loadEvents;
    _itemsLoadedEvent = itemsLoadedEvent;
    _fiveSecondsUpdateEvents = fiveSecondsUpdateEvents;

    _pressedEEvents = pressedEEvents;
    _pressedIEvents = pressedIEvents;
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
      _playerConnectedEvents?.ForEach(playerConnectEvent => playerConnectEvent.OnPlayerConnect(player, reason));

    AltAsync.OnPlayerDisconnect += async (IPlayer player, string reason) =>
      _playerDisconnectedEvents?.ForEach(playerDisconnectEvent => playerDisconnectEvent.OnPlayerDisconnect(player, reason));

    AltAsync.OnPlayerDead += async (IPlayer player, IEntity killer, uint weapon) =>
      _playerDeadEvents?.ForEach(playerDeadEvent => playerDeadEvent.OnPlayerDeath(player, killer, weapon));

    _timerHandler.AddInterval(1000 * 5, async (s, e) =>
      _fiveSecondsUpdateEvents?.ForEach(fiveSecondsUpdateEvent => fiveSecondsUpdateEvent.OnFiveSecondsUpdate()));

    return Task.CompletedTask;
  }

  public async void OnCommand(IPlayer iplayer, string commandName)
  {
    xPlayer player = (xPlayer)iplayer;
  }

  public async void OnKeyPressE(IPlayer iplayer)
  {
    xPlayer player = (xPlayer)iplayer;
    if (!player.CanInteract()) return;
    foreach (var pressedEEvent in _pressedEEvents)
    {
      if (await pressedEEvent.OnKeyPressE((xPlayer)player)) return;
    }
  }

  public async void OnKeyPressI(IPlayer iplayer)
  {
    xPlayer player = (xPlayer)iplayer;
    if (!player.CanInteract()) return;
    foreach (var pressedIEvent in _pressedIEvents)
    {
      if (await pressedIEvent.OnKeyPressI((xPlayer)player)) return;
    }
  }

  public void ItemsLoaded()
  {
    _logger.Info("ItemsLoaded");
    foreach (var itemsLoadedEvent in _itemsLoadedEvent)
    {
      itemsLoadedEvent.ItemsLoaded();
    }
  }
}