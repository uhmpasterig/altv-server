using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[PrimaryKey("id")]
public partial class Storage
{
  public Storage() {
  }
  public int id { get; set; }

  public string name { get; set; }  
  public float maxWeight { get; set; }
  public float currentWeight { get; set; }
  public int slots { get; set; }
  public string _pos { get; set; }
  public bool usePos { get; set; }
  public string _items { get; set; }
}