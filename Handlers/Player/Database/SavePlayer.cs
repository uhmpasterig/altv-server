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

public partial class PlayerHandler : IOneMinuteUpdateEvent
{
  public async void OnOneMinuteUpdate()
  {
    _logger.Log("Saving all player accounts to database");
    Players.ToList().ForEach(async (KeyValuePair<int, xPlayer> kvp) =>
    {
      _logger.Log($"Saving {kvp.Value.Name} to database");
      await kvp.Value.Save();
    });
  }
}
