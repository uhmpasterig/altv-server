using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace server.Models;

[PrimaryKey("permaId")]
public partial class Player
{
  public Player() { 
  }

  public bool isOnline { get; set; }
  public int permaId { get; set; }
  public string name { get; set; }

  public ushort health { get; set; }
  public ushort armor { get; set; }

  public string _playerInventorys { get; set; }

  public string _weapons { get; set; }

  public string job { get; set; }
  public int job_rank { get; set; }
  public string job_perm { get; set; }

  public string _pos { get; set; }
  public string _rot { get; set; }

  public DateTime lastLogin { get; set; }
  public DateTime creationDate { get; set; }


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

  [NotMapped]
  public Dictionary<string, int> playerInventorys {
    get {
      return JsonConvert.DeserializeObject<Dictionary<string, int>>(_playerInventorys)!;
    }
    set {
      _playerInventorys = JsonConvert.SerializeObject(value);
    }
  }
}
