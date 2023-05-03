using server.Core;

namespace server.Events;
public interface IPlayerConnectEvent
{
  void OnPlayerConnect(xPlayer player, string reason);
}
