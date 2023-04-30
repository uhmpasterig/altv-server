using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Elements.Entities;
using server.Core;

namespace server.Models;

[Table("farming_props")]
[PrimaryKey("id")]
public partial class Farming_Props
{
  public Farming_Props()
  {
  }
  public int id { get; set; }
  [ForeignKey("route_id")]
  public int route_id { get; set; }
  public Farming_Collector collector { get; set; }

  public string model { get; set; }
  public string _pos { get; set; }
  public string _rot { get; set; }
  [NotMapped]
  public Position Position
  {
    get { return JsonConvert.DeserializeObject<Position>(_pos);}
    set { _pos = JsonConvert.SerializeObject(value); }
  }

  [NotMapped]
  public Rotation Rotation
  {
    get { return JsonConvert.DeserializeObject<Rotation>(_rot); }
    set { _rot = JsonConvert.SerializeObject(value); }
  }
}