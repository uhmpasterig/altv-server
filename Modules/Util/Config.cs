using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;
namespace server.Util.Config;



public class StorageConfig
{

  public class StorageData
  {
    public string name { get; set; }
    public int slots { get; set; }
    public float maxWeight { get; set; }
    public Position? position { get; set; }

    public StorageData()
    {
    }
  }

  public static List<StorageData> StoragesDieJederHabenSollte = new List<StorageData>()
  {
    new StorageData()
    {
      name = "Bank Schliessfach",
      slots = 20,
      maxWeight = 200,
      position = Positions.BankSchliessfach
    },
    new StorageData()
    {
      name = "Export Schliessfach",
      slots = 30,
      maxWeight = 300,
      position = Positions.ExportSchliessfach
    },
    new StorageData()
    {
      name = "Inventar",
      slots = 20,
      maxWeight = 200,
      position = null
    },
    new StorageData()
    {
      name = "Import Schliessfach",
      slots = 50,
      maxWeight = 1000,
      position = Positions.ImportSchliessfach
    }
  };

}



public class Positions
{
  public static Position Spawn = new Position(0, 0, 0);
  public static Position BankSchliessfach = new Position(143.49f, -1041.91f, 29.37f);
  public static Position ExportSchliessfach = new Position(1240.12f, -3239.34f, 5.9f);
  public static Position ImportSchliessfach = new Position(-406.82f, 6149.71f, 31.6f);
}