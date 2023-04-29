using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net;

namespace server.Models;

[Table("cloth_shops")]
[PrimaryKey("id")]
public partial class Cloth_Shop_Cloth
{
  public Cloth_Shop_Cloth()
  {
  }

  public int id { get; set; }
  public string name { get; set; }
  public string category { get; set; }
  public string _cloth_ids { get; set; }

  [ForeignKey("shop_id")]
  public int shop_id { get; set; }  
  public Cloth_Shop shop { get; set; }

  [NotMapped]
  public List<int> cloth_ids
  {
    get { return JsonConvert.DeserializeObject<List<int>>(_cloth_ids); }
    set { _cloth_ids = JsonConvert.SerializeObject(value); }
  }
}