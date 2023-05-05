using AltV.Net;

namespace server.Handlers.Logger;

public class Logger : ILogger
{
  public void Log(string message)
  {
    _log("LOG", message, ConsoleColor.White);
  }

  public void Startup(string message)
  {
    _log("STARTUP", message, ConsoleColor.Cyan);
  }

  public void Debug(string message)
  {
    _log("DEBUG", message, ConsoleColor.DarkGray);
  }

  public void Info(string message)
  {
    _log("INFO", message, ConsoleColor.Green);
  }

  public void Warning(string message)
  {
    _log("WARN", message, ConsoleColor.Yellow);
  }

  public void Error(string message)
  {
    _log("ERROR", message, ConsoleColor.Red);
  }

  public void Exception(string message)
  {
    _log("EXCEPTION", message, ConsoleColor.Red);
  }

  public void Fatal(string message)
  {
    _log("FATAL", message, ConsoleColor.Red);
  }

  private void _log(string prefix, string message, ConsoleColor color)
  {
    Console.ForegroundColor = color;
    Console.WriteLine($"[{prefix}] {message}");
  }
}