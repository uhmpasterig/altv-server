using AltV.Net;

namespace server.Logger;

public class Logger
{
  
  public static void Log(string message)
  {
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"[LOG] {message}");
  }

  public static void Startup(string message)
  {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"[STARTUP] {message}");
  }

  public static void Debug(string message)
  {
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"[DEBUG] {message}");
  }

  public static void Info(string message)
  {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[INFO] {message}");
  }

  public static void Warning(string message)
  {
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"[WARN] {message}");
  }

  public static void Error(string message)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[ERROR] {message}");
  }

  public static void Exception(string message)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[EXCEPTION] {message}");
  }

  public static void Fatal(string message)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[FATAL] {message}");
  }
}