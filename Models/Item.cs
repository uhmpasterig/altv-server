using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[PrimaryKey("id")]
public partial class Item
{
  public int id { get; set; }

  public string name { get; set; }  
  public int stackSize { get; set; }
  public float weight { get; set; }
  public string job { get; set; }
  public string data { get; set; }
}