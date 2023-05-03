using server.Core;


namespace server.Events;
public interface IPlayerDisconnectEvent
{
  void OnPlayerDisconnect(xPlayer player, string reason);
}
