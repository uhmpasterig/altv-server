using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using server.Core;
using server.Models;
using server.Handlers.Storage;
using server.Modules.Items;

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
      string? vehname = veh.UIData.Where(x => x.Key == player).Select(x => x.Value["name"]).FirstOrDefault().ToString();
      if (vehname == null) vehname = "";
      string? keyword = veh.UIData.Where(x => x.Key == player).Select(x => x.Value["keyword"]).FirstOrDefault().ToString();
      if (keyword == null) keyword = "";
      bool? fav = (bool?)veh.UIData.Where(x => x.Key == player).Select(x => x.Value["fav"]).FirstOrDefault();
      if (fav == null) fav = false;

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