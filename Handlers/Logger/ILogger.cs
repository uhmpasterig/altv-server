using AltV.Net;
using server.Core;
using server.Models;
using AltV.Net.Data;

namespace server.Handlers.Logger;

public interface ILogger
{
  public void Log(string message);
  public void Startup(string message);
  public void Debug(string message);
  public void Info(string message);
  public void Warning(string message);
  public void Error(string message);
  public void Exception(string message);
  public void Fatal(string message);
}