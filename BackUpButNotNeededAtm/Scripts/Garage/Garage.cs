using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using AltV.Net.Data;
using Newtonsoft.Json;

using server.Handlers.Sql;
using MySql.Data.MySqlClient;
using server.Entitys;

using server.Data;

namespace server.Scripts
{
  public class Garage
  {
    internal class GarageData
    {
      public string ped;
      public string name;
      public string type;
      public Position coords;
      public float rotation;
    }
    internal class ParkoutData
    {
      public string coords;
      public string rotation;
    }
    static List<GarageData> Garages = new List<GarageData>();

    public static async void _init()
    {
      MySqlCommand command = Datenbank.Connection.CreateCommand();
      command.CommandText = "SELECT * FROM garages";
      MySqlDataReader reader = command.ExecuteReader();
      while (reader.Read())
      {
        GarageData garageData = new GarageData();
        garageData.name = reader.GetString("name");
        garageData.type = reader.GetString("type");
        garageData.coords = JsonConvert.DeserializeObject<Position>(reader.GetString("coords"));
        garageData.rotation = reader.GetFloat("rotation");
        garageData.ped = reader.GetString("ped");

        string text = $"Um die Garage {garageData.name} zu öffnen.";
        xEntityData entityData = new xEntityData(text, ENTITY_TYPES.HELPTEXT, garageData.coords, 0, 0, 3);
        xEntity entityText = new xEntity(entityData);
        entityText.Add();

        garageData.coords.Z = garageData.coords.Z - 1.0f;
        xEntityData entityData2 = new xEntityData(garageData.ped, ENTITY_TYPES.PED, garageData.coords, 0, 0, 100);
        xEntity entityPed = new xEntity(entityData2);
        entityPed.Add();
        if (garageData.type == "car")
        {
          ScriptLoader.Blips.Add(new ScriptLoader.BlipData
          {
            position = garageData.coords,
            sprite = 290,
            color = 38,
            name = "PKW Garage"
          });
        }
        else if (garageData.type == "lkw")
        {
          ScriptLoader.Blips.Add(new ScriptLoader.BlipData
          {
            position = garageData.coords,
            sprite = 67,
            color = 38,
            name = "LKW Garage"
          });
        }

        Garages.Add(garageData);
      }
      reader.Close();
      Alt.Log("Garage.cs loaded");
    }
  }
}