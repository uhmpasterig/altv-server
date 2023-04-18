using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[PrimaryKey("id")]
public partial class GarageSpawns
{
  public GarageSpawns() {
  }

  public int garage_id { get; set; }
  public string _pos { get; set; }
  public string _rot { get; set; }

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