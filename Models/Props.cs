using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace server.Models;

[PrimaryKey("permaId")]
public partial class Prop
{
  public Prop() { 
  }

  public int id { get; set; }
  public string prop { get; set; }
  public string _pos { get; set; }
  public string _rot { get; set; }


  [NotMapped]
  public Position Position {
    get {
      return JsonConvert.DeserializeObject<Position>(_pos);
      }
    set {
      _pos = JsonConvert.SerializeObject(value);
    }
  }
  [NotMapped]
  public Rotation Rotation {
    get {
      return JsonConvert.DeserializeObject<Rotation>(_rot);
    }
    set {
      _rot = JsonConvert.SerializeObject(value);
    }
  }
}
