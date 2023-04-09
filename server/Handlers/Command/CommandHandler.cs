using server.Core;
using AltV.Net;
using AltV.Net.Resources.Chat.Api;
using _logger = server.Logger.Logger;

namespace server.Handlers.Command;

public class CommandHandler : ICommandHandler
{
  public void OnCommand(xPlayer player, string commandName, string[] args)
  {
    
  }
  public void RegisterCommand(string commandName, Action<xPlayer, string[]> cb, bool restricted)
  {
    
  }
}