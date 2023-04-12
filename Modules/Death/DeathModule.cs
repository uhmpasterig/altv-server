using server.Core;
using server.Events;
using server.Handlers.Event;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;

namespace server.Modules.Death;

class DeathModule : IPlayerDeadEvent
{
  public void OnPlayerDeath(IPlayer iplayer, IEntity ikiller, uint weapon)
  {
    xPlayer player = (xPlayer)iplayer;
    xPlayer killer = (xPlayer)ikiller;
    player.SetDead(1);
  }
}