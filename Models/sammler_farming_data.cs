using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Elements.Entities;
using server.Core;

namespace server.Models;

public class propData
{
  public Rotation rotation { get; set; }
  public Position position { get; set; }
  public string model { get; set; }

  public propData(Rotation rotation, Position position, string model)
  {
    this.rotation = rotation;
    this.position = position;
    this.model = model;
  }
}

[PrimaryKey("id")]
public partial class sammler_farming_data
{
  public sammler_farming_data()
  {
  }
  public int id { get; set; }
  public string name { get; set; }
  public string tool { get; set; }
  public int timeS { get; set; }
  public string item { get; set; }
  public int amountmin { get; set; }
  public int amountmax { get; set; }
  public string prop { get; set; }
  public string _pos { get; set; }
  public string _propPositions { get; set; }
  public string sideProducts { get; set; }

  [NotMapped]
  public Position Position
  {
    get
    {
      return JsonConvert.DeserializeObject<Position>(_pos);
    }
    set
    {
      _pos = JsonConvert.SerializeObject(value);
    }
  }



  [NotMapped]
  public List<propData> PropPositions { get; set; } = new List<propData>();

  [NotMapped]
  public List<string> SideProducts
  {
    get
    {
      return JsonConvert.DeserializeObject<List<string>>(sideProducts)!;
    }
    set
    {
      sideProducts = JsonConvert.SerializeObject(value);
    }
  }

  [NotMapped]
  public List<xEntity> Entities { get; set; } = new List<xEntity>();

  public override string ToString()
  {
    return $"Name: {name}, Tool: {tool}, Time: {timeS}, Item: {item}, Prop: {prop}, Position: {Position}, PropPositions: {PropPositions}, SideProducts: {SideProducts}";
  }
}