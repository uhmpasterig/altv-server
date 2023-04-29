using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;
namespace server.Config;



public class StorageConfig
{

  public class StorageData
  {
    public string name { get; set; }
    public int slots { get; set; }
    public float maxWeight { get; set; }
    public Position? position { get; set; }
    public bool loadOnConnect { get; set; } = true;

    public StorageData()
    {
    }
  }

  public static List<StorageData> StoragesDieJederHabenSollte = new List<StorageData>()
  {
    new StorageData()
    {
      name = "Inventar",
      loadOnConnect = true,
      slots = 10,
      maxWeight = 150,
      position = null
    },
    new StorageData()
    {
      name = "Bank Schliessfach",
      loadOnConnect = true,
      slots = 20,
      maxWeight = 200,
      position = Positions.BankSchliessfach
    },
    new StorageData()
    {
      name = "Export Schliessfach",
      loadOnConnect = true,
      slots = 30,
      maxWeight = 300,
      position = Positions.ExportSchliessfach
    },
    new StorageData()
    {
      name = "Import Schliessfach",
      loadOnConnect = true,
      slots = 50,
      maxWeight = 1000,
      position = Positions.ImportSchliessfach
    },
    new StorageData()
    {
      name = "Fraktions Tresor",
      loadOnConnect = false,
      slots = 10,
      maxWeight = 100,
      position = null
    },
    new StorageData()
    {
      name = "Haus Garage",
      loadOnConnect = false,
      slots = 15,
      maxWeight = 150,
      position = null
    },
    new StorageData()
    {
      name = "Warenannahme Fabrik",
      loadOnConnect = true,
      slots = 32, 
      maxWeight = 320,
      position = Positions.WorkstationInput
    },
    new StorageData()
    {
      name = "Warenausgabe Fabrik",
      loadOnConnect = true,
      slots = 32,
      maxWeight = 320,
      position = Positions.WorkstationOutput
    },
  };
}