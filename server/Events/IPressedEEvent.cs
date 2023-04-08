using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using server.Core;


namespace server.Events;
public interface IPressedEEvent
{
  Task<bool> OnKeyPressE(xPlayer player);
}
