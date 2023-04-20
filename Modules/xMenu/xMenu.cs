using server.Events;
using server.Core;
using server.Models;
using server.Handlers.Entities;
using AltV.Net.Data;
using AltV.Net.Async;
using Newtonsoft.Json;
using _logger = server.Logger.Logger;

namespace server.Modules.xMenu;

public class xMenu : IPressedEEvent
{
  async void OnKeyPressE(xPlayer player) 
  {
    player.SendMessage("Vehicle", "Du hast E gedrückt", 5000, NOTIFYS.INFO);
    player.SendMessage("Vehicle", "Du hast E gedrückt", 5000, NOTIFYS.ERROR);
  }
}