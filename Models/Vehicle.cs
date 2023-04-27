using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using server.Core;

namespace server.Models;

[Table("vehicles")]
[PrimaryKey("id")]
public partial class Vehicle
{
  public Vehicle()
  {
  }
  public int id { get; set; }
  public int owner_id { get; set; }
  public int owner_type { get; set; }
  public int type { get; set; }
  public int garage_id { get; set; }
  public string model { get; set; }

  public int storage_id_trunk { get; set; }
  public int storage_id_glovebox { get; set; }

  public string _pos { get; set; }
  public string _rot { get; set; }

  public DateTime lastAction { get; set; }
  public DateTime creationDate { get; set; }

  public Vehicle_Data vehicle_data { get; set; }

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
}