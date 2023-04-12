using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;

namespace server.Events;
public interface IPlayerDeadEvent
{
  Task OnPlayerDeath(IPlayer player, IEntity killer, uint weapon);
}
