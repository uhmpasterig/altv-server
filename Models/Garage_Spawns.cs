using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("garage_spawns")]
[PrimaryKey("id")]
public partial class GarageSpawn
{
  public GarageSpawn() {
  }

  public int id { get; set; }
  public string _pos { get; set; }
  public string _rot { get; set; }

  [ForeignKey("garage_id")]
  public int garage_id { get; set; }
  public Garage Garage { get; set; }

  [NotMapped]
  public Position Position {
    get {
      return JsonConvert.DeserializeObject<Position>(_pos);
      }
    set {
      _pos = JsonConvert.SerializeObject(value);
    }
  }

  [NotMapped]
  public Rotation Rotation {
    get {
      return JsonConvert.DeserializeObject<Rotation>(_rot);
    }
    set {
      _rot = JsonConvert.SerializeObject(value);
    }
  }
}