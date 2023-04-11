using AltV.Net;

namespace server.Logger;

public class Logger
{
  
  public static void Log(string message)
  {
    Console.WriteLine($"[LOG] {message}");
  }

  public static void Startup(string message)
  {
    Console.BackgroundColor = ConsoleColor.Blue;
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"[STARTUP] {message}");
  }

  public static void Debug(string message)
  {
    Console.WriteLine($"[DEBUG] {message}");
  }

  public static void Info(string message)
  {
    Console.WriteLine($"[INFO] {message}");
  }

  public static void Warning(string message)
  {
    Console.WriteLine($"[WARN] {message}");
  }

  public static void Error(string message)
  {
    Console.WriteLine($"[ERROR] {message}");
  }

  public static void Exception(string message)
  {
    Console.WriteLine($"[EXCEPTION] {message}");
  }

  public static void Fatal(string message)
  {
    Console.WriteLine($"[FATAL] {message}");
  }
}