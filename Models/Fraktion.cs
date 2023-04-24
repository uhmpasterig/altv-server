using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[PrimaryKey("id")]
public partial class Fraktion
{
  public Fraktion() {
  }
  
  public int id { get; set; }
  public int storage_id { get; set; }
  public string name { get; set; }
  public string weapon { get; set; }
  public string droge { get; set; }
  public string _pos { get; set; }
  public string _posLager { get; set; }
  public int money { get; set; }

  public int ug_id { get; set; }
  public string uicolor { get; set; }

  public string motd { get; set; }
  public int warns { get; set; }
  public string funk { get; set; }
  public string fight_funk { get; set; }
  public string ug_funk { get; set; }

  public DateTime creationDate { get; set; }

  [NotMapped]
  public Dictionary<int, Fraktion_rang> raenge { get; set; }

  [NotMapped]
  public Fraktion_ug untergruppe { get; set; }

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
  public Position PositionLager
  {
    get
    {
      return JsonConvert.DeserializeObject<Position>(_posLager);
    }
    set
    {
      _posLager = JsonConvert.SerializeObject(value);
    }
  }
}