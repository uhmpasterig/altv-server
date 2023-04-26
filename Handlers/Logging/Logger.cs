using AltV.Net;

namespace server.Logger;

public class Logger
{
  public static bool _clientLogsEnabled = false;

  public static void Log(string message)
  {
    _log("LOG", message, ConsoleColor.White);
  }

  public static void Startup(string message)
  {
    _log("STARTUP", message, ConsoleColor.Cyan);
  }

  public static void Debug(string message)
  {
    _log("DEBUG", message, ConsoleColor.DarkGray);
  }

  public static void Info(string message)
  {
    _log("INFO", message, ConsoleColor.Green);
  }

  public static void Warning(string message)
  {
    _log("WARN", message, ConsoleColor.Yellow);
  }

  public static void Error(string message)
  {
    _log("ERROR", message, ConsoleColor.Red);
  }

  public static void Exception(string message)
  {
    _log("EXCEPTION", message, ConsoleColor.Red);
  }

  public static void Fatal(string message)
  {
    _log("FATAL", message, ConsoleColor.Red);
  }

  private static void _log(string prefix, string message, ConsoleColor color)
  {
    Console.ForegroundColor = color;
    Console.WriteLine($"[{prefix}] {message}");
    if(_clientLogsEnabled) Alt.EmitAllClients("client:log", prefix, message);
  }

  public static void EnableClientLogging()
  {
    _clientLogsEnabled = true;
  }
}