using AltV.Net.Elements.Entities;
using AltV.Net;
using AltV.Net.Resources.Chat.Api;
using server.Player;
using server.Entitys;
using MySql.Data.MySqlClient;
using server.Handlers.Sql;
using Newtonsoft.Json;
using AltV.Net.Data;

namespace server.Commands
{
  public class Dev : IScript
  {
    [Command("id")]
    public void ID(IPlayer iplayer)
    {
      xPlayer player = Players.getPlayer(iplayer);
      if (player == null)
      {
        iplayer.SendChatMessage("Du bist nicht registriert!");
        return;
      }
      iplayer.SendChatMessage("Deine ID: " + player.id);
    }
    [Command("car")]
    public void Car(IPlayer iplayer, string model)
    {
      xPlayer player = Players.getPlayer(iplayer);
      if (player == null)
      {
        iplayer.SendChatMessage("Du bist nicht registriert!");
        return;
      }
      IVehicle vehicle = Alt.CreateVehicle(Alt.Hash(model), iplayer.Position, iplayer.Rotation);
    }

    [Command("goto")]
    public void Goto(IPlayer iplayer, int id)
    {
      xPlayer player = Players.getPlayer(id);
      if (player == null)
      {
        iplayer.SendChatMessage("Dieser Spieler Exsistiert nicht!");
        return;
      }
      iplayer.Position = player.iPlayer.Position;
    }

    [Command("r")]
    public void R(IPlayer iplayer, int id = -1)
    {
      if (id == -1) {
        xPlayer player = Players.getPlayer(iplayer);
        player.Revive();
      } else {
        xPlayer player = Players.getPlayer(id);
        player.Revive();
      }


    }

    [Command("bring")]

    public void Bring(IPlayer iplayer, int id)
    {
      xPlayer player = Players.getPlayer(id);
      if (player == null)
      {
        iplayer.SendChatMessage("Dieser Spieler Exsistiert nicht!");
        return;
      }
      player.iPlayer.Position = iplayer.Position;
    }

    [Command("haveperm")]
    public void HavePerm(IPlayer iplayer, string perm)
    {
      xPlayer player = Players.getPlayer(iplayer);
      if (player.HasPerm(perm))
      {
        iplayer.SendChatMessage("Du hast die Permission " + perm);
        return;
      }
      iplayer.SendChatMessage("Du hast die Permission nicht " + perm);
    }

    [Command("entityc")]
    public void EntityTest(IPlayer iplayer, string prop)
    {
      xEntityData entityData = new xEntityData(prop, ENTITY_TYPES.PROP, iplayer.Position, 0, iplayer.Dimension, 100);
      xEntity entity = new xEntity(entityData);
      entity.Add();
      iplayer.SendChatMessage("Entity ID: " + entity.id);
    }

    [Command("textt")]
    public void EntityTest2(IPlayer iplayer, string text)
    {
      xEntityData entityData = new xEntityData(text, ENTITY_TYPES.HELPTEXT, iplayer.Position, 0, iplayer.Dimension, 10);
      xEntity entity = new xEntity(entityData);
      entity.Add();
      iplayer.SendChatMessage("Text ID: " + entity.id);
    }

    [Command("entityh")]
    public void EntityR2(IPlayer iplayer, int id)
    {
      xEntity entity = xEntity.allEntitys.Get(id);
      if (entity == null)
      {
        iplayer.SendChatMessage("Entity nicht gefunden!");
        return;
      }
      entity.Remove();
    }
    [Command("entitys")]
    public void Entitys(IPlayer iplayer, int id)
    {
      xEntity entity = xEntity.allEntitys.Get(id);
      if (entity == null)
      {
        iplayer.SendChatMessage("Entity nicht gefunden!");
        return;
      }
      entity.Add();
    }

    [Command("givi")]
    public void Givi(IPlayer iplayer, string item, int count)
    {
      xPlayer player = Players.getPlayer(iplayer);
      player.GiveItem(item, count);
      Alt.Log("Givi done: " + item + " " + count);
    }

    internal class GarageData
    {
      public string ped;
      public string name;
      public string type;
      public Position coords;
      public float rotation;
    }
    [Command("createGarage")]
    public void CreateGarage(IPlayer iplayer, string name, string type)
    {
      GarageData vehicleData = new GarageData();
      vehicleData.name = name;
      vehicleData.type = type;
      vehicleData.coords = iplayer.Position;
      vehicleData.rotation = 20.0f;
      vehicleData.ped = "s_m_y_dealer_01";

      MySqlCommand command = Datenbank.Connection.CreateCommand();
      command.CommandText = "INSERT into garages (name, type, coords, heading, ped) VALUES (@name, @type, @coords, @heading, @ped)";
      command.Parameters.AddWithValue("@name", vehicleData.name);
      command.Parameters.AddWithValue("@type", vehicleData.type);
      command.Parameters.AddWithValue("@coords", JsonConvert.SerializeObject(vehicleData.coords));
      command.Parameters.AddWithValue("@heading", JsonConvert.SerializeObject(vehicleData.rotation));
      command.Parameters.AddWithValue("@ped", vehicleData.ped);
      command.ExecuteNonQuery();
    }

    internal class ParkoutData
    {
      public string coords;
      public string rotation;
    }
    [Command("addParkout")]
    public void AddParkout(IPlayer iplayer, string name)
    {
      List<ParkoutData> Parkouts = new List<ParkoutData>();
      MySqlCommand command = Datenbank.Connection.CreateCommand();
      command.CommandText = "SELECT * FROM garages WHERE name = @name";
      command.Parameters.AddWithValue("@name", name);
      MySqlDataReader reader = command.ExecuteReader();
      if (reader.Read())
      {
        Parkouts = JsonConvert.DeserializeObject<List<ParkoutData>>(reader.GetString("parkouts"));        
      } 
      reader.Close();
      
      ParkoutData parkoutData2 = new ParkoutData();
      parkoutData2.coords = JsonConvert.SerializeObject(iplayer.Position);
      parkoutData2.rotation = JsonConvert.SerializeObject(iplayer.Rotation);
      Parkouts.Add(parkoutData2);

      MySqlCommand command2 = Datenbank.Connection.CreateCommand();
      command2.CommandText = "UPDATE garages SET parkouts = @parkouts WHERE name = @name";
      command2.Parameters.AddWithValue("@name", name);
      command2.Parameters.AddWithValue("@parkouts", JsonConvert.SerializeObject(Parkouts));
      command2.ExecuteNonQuery();
    }
  }
}