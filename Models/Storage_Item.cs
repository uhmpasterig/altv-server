using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using server.Core;

namespace server.Models;

[Table("storage_items")]
[PrimaryKey("id")]
public partial class Storage_Item
{
  public Storage_Item(Item item, int count)
  {
    this.Item_Data = item;
    this.count = count;
  }

  public Storage_Item(Item item, int count, int slot)
  {
    this.Item_Data = item;
    this.count = count;
    this.slot = slot;
  }

  public Storage_Item() { }

  public int id { get; set; }

  [ForeignKey("storage_id")]
  public int storage_id { get; set; }
  public Storage Storage { get; set; }

  [ForeignKey("item_id")]
  public int item_id { get; set; }
  public Item Item_Data { get; set; }

  public int slot { get; set; }
  public int count { get; set; }
  public string _data { get; set; }
  public DateTime created_at { get; set; }

  [NotMapped]
  public Dictionary<string, object> data
  {
    get { return JsonConvert.DeserializeObject<Dictionary<string, object>>(this._data); }
    set { this._data = JsonConvert.SerializeObject(value); }
  }
}