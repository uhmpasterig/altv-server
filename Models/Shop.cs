using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[PrimaryKey("id")]
public partial class Shop
{
  public Shop()
  {
    items = new List<ShopItems>();
  }

  public int id { get; set; }
  public string name { get; set; }
  public int type { get; set; }
  public string _pos { get; set; }
  public float heading { get; set; }
  public string ped { get; set; }
  public float tax { get; set; }

  [NotMapped]
  public string typeString { get; set; }

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
  public List<ShopItems> items { get; set; }
}