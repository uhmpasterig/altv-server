using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Elements.Entities;
using server.Core;

namespace server.Models;

public partial class sammler_farming_data
{
  public sammler_farming_data() {
  }
  
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
  public Position[] PropPositions
  {
    get
    {
      return JsonConvert.DeserializeObject<Position[]>(_propPositions)!;
    }
    set
    {
      _propPositions = JsonConvert.SerializeObject(value);
    }
  }

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