using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Elements.Entities;
using server.Core;

namespace server.Models;

[Table("player_factories")]
[PrimaryKey("id")]
public partial class Player_Factory
{
  public Player_Factory()
  {
  }
  public int id { get; set; }
  public int selected_process { get; set; }

  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Models.Player Player { get; set; }

  [NotMapped]
  public int ticksDone { get; set; } = 0;
}