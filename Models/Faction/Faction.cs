using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("factions")]
[PrimaryKey("id")]
public partial class Faction
{
  public Faction()
  {
  }

  public int id { get; set; }
  public string name { get; set; }
  public string label { get; set; }

  public List<Faction_Ranks> Ranks { get; set; }
}
