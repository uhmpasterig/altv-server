using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("storages")]
[PrimaryKey("id")]
public partial class Storage
{
  public Storage() {
  }
  public int id { get; set; }
  public int ownerId { get; set; }

  public string name { get; set; }  
  public float maxWeight { get; set; }
  public float currentWeight { get; set; }
  public int slots { get; set; }
  public string _pos { get; set; }
  public bool usePos { get; set; }
  public string _items { get; set; }

  [NotMapped]
  public Position? Position
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
}