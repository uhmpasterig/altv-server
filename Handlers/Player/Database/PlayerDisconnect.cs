using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using server.Core;
using server.Models;
using Microsoft.EntityFrameworkCore;
using server.Events;
using Newtonsoft.Json;
using System.Diagnostics;
using server.Config;

using server.Handlers.Logger;

namespace server.Handlers.Player;

public partial class PlayerHandler : IPlayerDisconnectEvent
{
  public async void OnPlayerDisconnect(xPlayer player, string reason)
  {
    await player.Save();
    Players.Remove(player.id);
  }
}