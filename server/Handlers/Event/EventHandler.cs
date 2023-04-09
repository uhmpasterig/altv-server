using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using server.Handlers.Timer;
using server.Core;
using server.Events;
using _logger = server.Logger.Logger;

namespace server.Handlers.Event;

public class EventHandler : IEventHandler
{
  private readonly ITimerHandler _timerHandler;
  private readonly IEnumerable<IPlayerConnectEvent> _playerConnectedEvents;
  private readonly IEnumerable<IPlayerDisconnectEvent> _playerDisconnectedEvents;
  private readonly IEnumerable<ILoadEvent> _loadEvents;

  private readonly IEnumerable<IItemsLoaded> _itemsLoadedEvent;


  // keypress event
  private readonly IEnumerable<IPressedEEvent> _pressedEEvents;
  private readonly IEnumerable<IPressedIEvent> _pressedIEvents;

  // timer event


  public EventHandler(ITimerHandler timerHandler,
                      IEnumerable<IPlayerConnectEvent> playerConnectedEvents,
                      IEnumerable<IPlayerDisconnectEvent> playerDisconnectEvents,
                      IEnumerable<ILoadEvent> loadEvents,
                      IEnumerable<IPressedEEvent> pressedEEvents,
                      IEnumerable<IPressedIEvent> pressedIEvents,
                      IEnumerable<IItemsLoaded> itemsLoadedEvent
                      )
  {
    AltAsync.OnClient<IPlayer>("PressE", OnKeyPressE);
    AltAsync.OnClient<IPlayer>("PressI", OnKeyPressI);
    AltAsync.OnServer("ItemsLoaded", ItemsLoaded);

    _timerHandler = timerHandler;
    _playerConnectedEvents = playerConnectedEvents;
    _playerDisconnectedEvents = playerDisconnectEvents;
    _loadEvents = loadEvents;
    _itemsLoadedEvent = itemsLoadedEvent;

    _pressedEEvents = pressedEEvents;
    _pressedIEvents = pressedIEvents;
  }

  public Task LoadHandlers()
  {
    foreach (var loadEvent in _loadEvents)
    {
      loadEvent.OnLoad();
    }
    _logger.Debug("Loading event handlers");
    AltAsync.OnPlayerConnect += async (IPlayer player, string reason) =>
    {
      foreach (var playerConnectEvent in _playerConnectedEvents)
      {
        playerConnectEvent.OnPlayerConnect(player, reason);
      }
    };

    AltAsync.OnPlayerDisconnect += async (IPlayer player, string reason) =>
    {
      foreach (var playerDisconnectEvent in _playerDisconnectedEvents)
      {
        playerDisconnectEvent.OnPlayerDisconnect(player, reason);
      }
    };

    return Task.CompletedTask;
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
    _logger.Info("OnKeyPressI");
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