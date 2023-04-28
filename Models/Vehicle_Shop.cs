using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using server.Core;

namespace server.Models;

[Table("vehicle_shops")]
[PrimaryKey("id")]
public partial class Vehicle_Shop
{
  public Vehicle_Shop()
  {
  }
  public int id { get; set; }
  public string name { get; set; }
  public int type { get; set; }
  public string _pos { get; set; }
  public float heading { get; set; }
  public string ped { get; set; }

  public List<Vehicle_Shop_Vehicle> Vehicles { get; set; }
  
  [NotMapped]
  public List<xVehicle> xVehicles { get; set; }
  
  [NotMapped]
  public Position Position
  {
    get { return JsonConvert.DeserializeObject<Position>(_pos); }
    set { _pos = JsonConvert.SerializeObject(value); }
  }
}