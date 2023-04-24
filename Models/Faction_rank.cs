using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[PrimaryKey("id")]
public partial class Faction_rank
{
  public Faction_rank()
  {
  }

  public int id { get; set; }
  public string label { get; set; }
  public int rank_id { get; set; }
  public int fraktions_id { get; set; }
}