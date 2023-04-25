using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("shop_items")]
[PrimaryKey("id")]
public partial class ShopItems
{
  public ShopItems()
  {
  }

  public int id { get; set; }
  public string item { get; set; }
  public int type { get; set; }
  public int price { get; set; }

  [NotMapped]
  public int count { get; set; } = 0;

  [NotMapped]
  public string label { get; set; } = "";
}