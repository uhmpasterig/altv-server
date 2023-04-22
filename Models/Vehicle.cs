using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using server.Core;

namespace server.Models;

[PrimaryKey("id")]
public partial class Vehicle
{
  public Vehicle()
  {
  }

  public int id { get; set; }
  public int ownerId { get; set; }
  public int garageId { get; set; }

  public string model { get; set; }
  public string _uidata { get; set; }

  public int storageIdTrunk { get; set; }
  public int storageIdGloveBox { get; set; }

  public string _pos { get; set; }
  public string _rot { get; set; }

  public int color { get; set; }
  public int color2 { get; set; }
  public string plate { get; set; }

  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }

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
  public Rotation Rotation
  {
    get
    {
      return JsonConvert.DeserializeObject<Rotation>(_rot);
    }
    set
    {
      _rot = JsonConvert.SerializeObject(value);
    }
  }

  [NotMapped]
  public Dictionary<int, Dictionary<string, object>> UIData
  {
    get
    {
      return JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, object>>>(_uidata);
    }
    set
    {
      _uidata = JsonConvert.SerializeObject(value);
      Console.WriteLine(_uidata);
    }
  }
}