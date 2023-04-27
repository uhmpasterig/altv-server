using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace server.Models;

[Table("player_society")]
[PrimaryKey("id")]
public partial class Player_Society
{
  public Player_Society()
  {
  }

  public int id { get; set; }

  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Models.Player Player { get; set; }

  [ForeignKey("faction_id")]
  public int faction_id { get; set; }
  public Faction Faction { get; set; }

  [ForeignKey("business_id")]
  public int business_id { get; set; }
  public Business Business { get; set; }

  public string faction_perms { get; set; }
  public string business_perms { get; set; }

  public int grade { get; set; }
  public int playtime { get; set; }

  [NotMapped]
  public List<string> FactionPerms
  {
    get { return JsonConvert.DeserializeObject<List<string>>(faction_perms); }
    set { faction_perms = JsonConvert.SerializeObject(value); }
  }
  [NotMapped]
  public List<string> BusinessPerms
  {
    get { return JsonConvert.DeserializeObject<List<string>>(business_perms); }
    set { business_perms = JsonConvert.SerializeObject(value); }
  }
}
