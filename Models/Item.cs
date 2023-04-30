using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("items")]
[PrimaryKey("id")]
public partial class Item
{
  public Item(Models.Item item)
  {
    this.id = item.id;
    this.name = item.name;
    this.label = item.label;
    this.stackSize = item.stackSize;
    this.weight = item.weight;
    this._data = item._data;
  }

  public Item()
  {
  }

  public int id { get; set; }

  public string name { get; set; }
  public string label { get; set; } = "Kein Label";
  public int stackSize { get; set; }
  public float weight { get; set; }
  public string _data { get; set; }

  [NotMapped]
  public Dictionary<string, object> data
  {
    get { return JsonConvert.DeserializeObject<Dictionary<string, object>>(this._data); }
    set { this._data = JsonConvert.SerializeObject(value); }
  }
}