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
}
