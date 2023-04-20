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
  public string garage = "Garage";
  public garagenWriter(List<Models.Vehicle> _vehicles, string _garage)
  {
    this.vehicles = _vehicles;
    this.garage = _garage;
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
      writer.BeginObject();
      writer.Name("id");
      writer.Value(veh.id);
      writer.Name("model");
      writer.Value(veh.model.ToUpper());
      writer.Name("name");
      writer.Value("Unbenannt");
      writer.Name("keyword");
      writer.Value("Unbenannt");
      writer.EndObject();
    }
    writer.EndArray();

    writer.EndObject();
  }
}