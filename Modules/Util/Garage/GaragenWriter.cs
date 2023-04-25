using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.Core;
using server.Models;
using server.Handlers.Storage;
using server.ModulesGoofy.Items;

namespace server.Util.Garage;
public class garagenWriter : IWritable
{
  private readonly List<Models.Vehicle> vehicles;
  private readonly xPlayer player;
  public string garage = "Garage";
  public garagenWriter(List<Models.Vehicle> _vehicles, string _garage, xPlayer _player)
  {
    this.vehicles = _vehicles;
    this.garage = _garage;
    this.player = _player;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("name");
    writer.Value(garage);
    writer.Name("vehicles");

    writer.BeginArray();
    foreach (Models.Vehicle veh in vehicles.ToList())
    {
      string vehname = "Unbekannt";
      string keyword = "";
      bool fav = false;

      Dictionary<string, object>? playerUidata = veh.UIData.Where(x => x.Key == player.id).Select(x => x.Value).FirstOrDefault();
      if (playerUidata != null)
      {
        vehname = playerUidata?["name"]?.ToString() ?? "";
        keyword = playerUidata?["keyword"]?.ToString() ?? "";
        fav = (bool?)playerUidata?["important"] ?? false;
      };

      writer.BeginObject();
      writer.Name("id");
      writer.Value(veh.id);
      writer.Name("model");
      writer.Value(veh.model.ToUpper());
      writer.Name("name");
      writer.Value(vehname);
      writer.Name("keyword");
      writer.Value(keyword);
      writer.Name("fav");
      writer.Value((bool)fav);
      writer.EndObject();
    }
    writer.EndArray();

    writer.EndObject();
  }
}