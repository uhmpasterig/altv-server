using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using server.Core;

namespace server.Models;

[Table("vehicle_shop_vehicles")]
[PrimaryKey("id")]
public partial class Vehicle_Shop_Vehicle
{
  public Vehicle_Shop_Vehicle()
  {
  }
  public int id { get; set; }
  public string name { get; set; }
  public string model { get; set; }
  public int slots { get; set; }
  public float maxWeight { get; set; }
  public int price { get; set; }

  public string _pos { get; set; }
  public string _rot { get; set; }

  [ForeignKey("Vehicle_Shop")]
  public int vehicle_shop_id { get; set; }
  public Vehicle_Shop Vehicle_Shop { get; set; }

  [NotMapped]
  public Position Position
  {
    get { return JsonConvert.DeserializeObject<Position>(_pos); }
    set { _pos = JsonConvert.SerializeObject(value); }
  }

  [NotMapped]
  public Rotation Rotation
  {
    get { return JsonConvert.DeserializeObject<Rotation>(_rot); }
    set { _rot = JsonConvert.SerializeObject(value); }
  }
}