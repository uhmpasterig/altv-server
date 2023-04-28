using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using server.Modules.Weapons;

namespace server.Models;

[Table("players")]
[PrimaryKey("id")]
public partial class Player
{
  public Player()
  {
  }

  public int id { get; set; }
  public string name { get; set; }
  public string ped { get; set; }

  public int cash { get; set; }
  public int bank { get; set; }
  public ushort health { get; set; }
  public ushort armor { get; set; }
  public ushort max_armor { get; set; }

  public string phone { get; set; }
  public string _boundStorages { get; set; }
  public string _weapons { get; set; }

  public string _pos { get; set; }
  public string _rot { get; set; }

  public DateTime lastLogin { get; set; }
  public DateTime creationDate { get; set; }
  public bool isOnline { get; set; }

  public string _dataCache { get; set; }

  public Player_Skin player_skin { get; set; }
  public Player_Cloth player_cloth { get; set; }
  
  public List<Vehicle_Key> vehicle_keys { get; set; }
  public Player_Society player_society { get; set; }
  public Player_Factory player_factory { get; set; }

  [NotMapped]
  public Dictionary<string, int> boundStorages
  {
    get { return JsonConvert.DeserializeObject<Dictionary<string, int>>(_boundStorages); }
    set { _boundStorages = JsonConvert.SerializeObject(value); }
  }

  [NotMapped]
  public List<xWeapon> weapons
  {
    get { return JsonConvert.DeserializeObject<List<xWeapon>>(_weapons); }
    set { _weapons = JsonConvert.SerializeObject(value); }
  }

  [NotMapped]
  public Dictionary<string, object> dataCache
  {
    get { return JsonConvert.DeserializeObject<Dictionary<string, object>>(_dataCache); }
    set { _dataCache = JsonConvert.SerializeObject(value); }
  }

  [NotMapped]
  public Position Position
  {
    get { return JsonConvert.DeserializeObject<Position>(_pos); } 
    set { _pos = JsonConvert.SerializeObject(value); }
  }

  [NotMapped]
  public Rotation Rotation
  {
    get { return JsonConvert.DeserializeObject<Rotation>(_rot); }
    set { _rot = JsonConvert.SerializeObject(value); }
  }
}
