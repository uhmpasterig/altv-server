using AltV.Net.Resources.Chat.Api;
using server.Core;

namespace server.Handlers.Command;
public interface ICommandHandler {
  public void OnCommand(xPlayer player, string commandName, string[] args);
  public void RegisterCommand(string commandName, Action<xPlayer, string[]> cb, bool restricted);
}