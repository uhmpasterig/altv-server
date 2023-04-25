using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace server.Models;

[Table("player_skin")]
[PrimaryKey("id")]
public partial class Player_Skin
{
  public Player_Skin()
  {
  }

  public int id { get; set; }

  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Models.Player Player { get; set; }

  public uint shape1 { get; set; }
  public uint shape2 { get; set; }
  public uint skin1 { get; set; }
  public uint skin2 { get; set; }
  public Single shapeMix { get; set; }
  public Single skinMix { get; set; }
}
