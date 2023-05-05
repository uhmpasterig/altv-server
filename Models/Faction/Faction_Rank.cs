using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace server.Models;
[Table("faction_ranks")]
[PrimaryKey("id")]
public partial class Faction_Ranks
{
  public Faction_Ranks()
  {
  }

  public int id { get; set; }
  public int grade { get; set; }
  public string name { get; set; }
  public string label { get; set; }

  [ForeignKey("faction_id")]
  public int faction_id { get; set; }
  public Faction Faction { get; set; } = new();
}