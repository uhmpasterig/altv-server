using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;


namespace server.Events;
public interface IPlayerDisconnectEvent
{
  void OnPlayerDisconnect(IPlayer player, string reason);
}
