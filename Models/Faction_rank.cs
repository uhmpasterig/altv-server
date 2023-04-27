using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("faction_ranks")]
[PrimaryKey("id")]
public partial class Faction_rank
{
  public Faction_rank()
  {
  }

  public int id { get; set; }
  public string label { get; set; }
  public int rank_id { get; set; }

  [ForeignKey("faction_id")]
  public int faction_id { get; set; }
  public Faction Faction { get; set; }
}