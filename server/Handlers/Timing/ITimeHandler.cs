using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace server.Handlers.Timer;
public interface ITimerHandler
{
  void AddTimeout(double timeout, ElapsedEventHandler handler);
  void AddInterval(double interval, ElapsedEventHandler handler);
  void StopAllIntervals();
}