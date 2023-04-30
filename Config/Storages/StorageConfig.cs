using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;

namespace server.Config;
public enum STORAGES : int
{
  INVENTORY = 1,
  BANK = 2,
  EXPORT = 3,
  IMPORT = 4,
  FACTION = 5,
  HOUSE = 6,
  FACTORY_IN = 7,
  FACTORY_OUT = 8,
}
public class StorageConfig
{

  public class StorageData
  {
    public int local_id { get; set; }
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
      local_id = 1,
      name = "Inventar",
      loadOnConnect = true,
      slots = 10,
      maxWeight = 150,
      position = null
    },
    new StorageData()
    {
      local_id = 2,
      name = "Bank Schliessfach",
      loadOnConnect = true,
      slots = 20,
      maxWeight = 200,
      position = Positions.BankSchliessfach
    },
    new StorageData()
    {
      local_id = 3,
      name = "Export Schliessfach",
      loadOnConnect = true,
      slots = 30,
      maxWeight = 300,
      position = Positions.ExportSchliessfach
    },
    new StorageData()
    {
      local_id = 4,
      name = "Import Schliessfach",
      loadOnConnect = true,
      slots = 50,
      maxWeight = 1000,
      position = Positions.ImportSchliessfach
    },
    new StorageData()
    {
      local_id = 5,
      name = "Fraktions Tresor",
      loadOnConnect = false,
      slots = 10,
      maxWeight = 100,
      position = null
    },
    new StorageData()
    {
      local_id = 6,
      name = "Haus Garage",
      loadOnConnect = false,
      slots = 15,
      maxWeight = 150,
      position = null
    },
    new StorageData()
    {
      local_id = 7,
      name = "Warenannahme Fabrik",
      loadOnConnect = true,
      slots = 32,
      maxWeight = 320,
      position = Positions.WorkstationInput
    },
    new StorageData()
    {
      local_id = 8,
      name = "Warenausgabe Fabrik",
      loadOnConnect = true,
      slots = 32,
      maxWeight = 320,
      position = Positions.WorkstationOutput
    },
  };
}