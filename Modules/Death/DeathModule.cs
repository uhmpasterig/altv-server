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
    Alt.Log("Player " + iplayer.Name + " is dead");
    Alt.Log("Player " + iplayer.Name + " is dead");
    Alt.Log("Player " + iplayer.Name + " is dead");
    Alt.Log("Player " + iplayer.Name + " is dead");
    xPlayer player = (xPlayer)iplayer;
    player.SetDead(1);
  }
}