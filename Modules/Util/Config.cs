using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;
namespace server.Util.Config;

public class Positions
{
  public static Position Spawn = new Position(0, 0, 0);
  public static Position BankSchliessfach = new Position(143.49f, -1041.91f, 29.37f);
  public static Position ExportSchliessfach = new Position(1240.12f, -3239.34f, 5.9f);
  public static Position ImportSchliessfach = new Position(-406.82f, 6149.71f, 31.6f);
}

public class FrakPrices 
{
  public static int main_weapon = 3000;
  public static int meele_weapon = 1000;
}