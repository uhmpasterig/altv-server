using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;
namespace server.Config;

public class Positions
{
  public static Position Spawn = new Position(0, 0, 0);
  public static Position BankSchliessfach = new Position(143.49f, -1041.91f, 29.37f);
  public static Position ExportSchliessfach = new Position(1240.12f, -3239.34f, 5.9f);
  public static Position ImportSchliessfach = new Position(-406.82f, 6149.71f, 31.6f);
  public static Position WorkstationInput = new Position(902.9275f, -2491.345f, 29.566284f);
  public static Position WorkstationOutput = new Position(895.34503f, -2491.0417f, 29.566284f);
}