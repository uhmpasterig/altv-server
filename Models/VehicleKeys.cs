using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using server.Core;

namespace server.Models;

[Table("vehicle_keys")]
[PrimaryKey("id")]
public partial class Vehicle_Key
{
  public Vehicle_Key()
  {
  }
  public int id { get; set; }
  
  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Models.Player Player { get; set; }

  [ForeignKey("vehicle_id")]
  public int vehicle_id { get; set; }
  public Models.Vehicle Vehicle { get; set; }
  
  public string name { get; set; }
  public DateTime creationDate { get; set; }
}