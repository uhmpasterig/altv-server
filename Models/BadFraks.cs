using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[PrimaryKey("id")]
public partial class BadFrak
{
  public BadFrak() {
  }
  
  public int id { get; set; }
  public string name { get; set; }
  public string weapon { get; set; }
  public string logo { get; set; }
  public string _pos { get; set; }
  public string _posLager { get; set; }
  public int money { get; set; }

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