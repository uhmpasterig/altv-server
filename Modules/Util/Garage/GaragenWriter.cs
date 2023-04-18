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
  private readonly List<xVehicle> invehicles;
  private readonly List<Models.Vehicle> outvehicles;
  public string garage = "Garage";
  public garagenWriter(List<xVehicle> _invehicles, List<Models.Vehicle> _outvehicles, string _garage)
  {
    this.invehicles = _invehicles;
    this.outvehicles = _outvehicles;
    this.garage = _garage;
  }

  public void OnWrite(IMValueWriter writer)
  {
    writer.BeginObject();
    writer.Name("name");
    writer.Value(garage);
    writer.Name("vehicles");
    writer.BeginObject();

    writer.Name("out");
    writer.BeginArray();
    foreach (Models.Vehicle veh in outvehicles.ToList())
    {
      writer.BeginObject();
      writer.Name("id");
      writer.Value(veh.id);
      writer.Name("model");
      writer.Value(veh.model.ToUpper());
      writer.EndObject();
    }
    writer.EndArray();

    writer.Name("in");
    writer.BeginArray();
    foreach (xVehicle veh in invehicles.ToList())
    {
      writer.BeginObject();
      writer.Name("id");
      writer.Value(veh.vehicleId);
      writer.Name("model");
      writer.Value(veh.model.ToUpper());
      writer.EndObject();
    }
    writer.EndArray();

    writer.EndObject();

    writer.EndObject();
  }
}