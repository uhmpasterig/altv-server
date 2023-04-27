using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace server.Models;

[Table("vehicle_data")]
[PrimaryKey("id")]
public partial class Vehicle_Data
{
  public Vehicle_Data()
  {
  }

  public int id { get; set; }

  [ForeignKey("vehicle_id")]
  public int vehicle_id { get; set; }
  public Models.Vehicle Vehicle { get; set; }

  public int r { get; set; }
  public int g { get; set; }
  public int b { get; set; }
  public int sr { get; set; }
  public int sg { get; set; }
  public int sb { get; set; }

  public string plate { get; set; }
  public double fuel { get; set; }
  public double mileage { get; set; }
  public string _uidata { get; set; }
  public string keys { get; set; }
  [NotMapped]
  public List<int> Keys
  {
    get { return JsonConvert.DeserializeObject<List<int>>(keys); }
    set { keys = JsonConvert.SerializeObject(value); }
  }
  [NotMapped]
  public Dictionary<int, Dictionary<string, object>> UIData
  {
    get { return JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, object>>>(_uidata); }
    set { _uidata = JsonConvert.SerializeObject(value); }
  }
}
