using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net;

namespace server.Models;

public class Cloth_Shop_Category
{
  public string name { get; set; }
  public string label { get; set; }
  public List<Cloth_Shop_Cloth> items { get; set; } = new List<Cloth_Shop_Cloth>();
}

[Table("cloth_shops")]
[PrimaryKey("id")]
public partial class Cloth_Shop
{
  public Cloth_Shop()
  {
  }

  public int id { get; set; }
  public string name { get; set; }
  public string _pos { get; set; }
  public string _categorys { get; set; }

  public List<Cloth_Shop_Cloth> cloth_items { get; set; } = new List<Cloth_Shop_Cloth>();
  [NotMapped]
  public List<Cloth_Shop_Category> Categorys
  {
    get
    {
      List<Cloth_Shop_Category>? categorys = JsonConvert.DeserializeObject<List<Cloth_Shop_Category>>(_categorys);

      foreach (Cloth_Shop_Cloth item in cloth_items)
      {
        if (categorys == null) continue;
        Cloth_Shop_Category? category = categorys.Find(c => c.name == item.category);
        if (category == null) continue;
        category.items.Add(item);
      }

      return categorys;

    }
  }

  [NotMapped]
  public Position Position
  {
    get { return JsonConvert.DeserializeObject<Position>(_pos); }
    set { _pos = JsonConvert.SerializeObject(value); }
  }
}