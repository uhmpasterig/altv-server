using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models;

[Table("player_worldoffsets")]
[PrimaryKey("id")]
public partial class Player_WorldOffset
{
  public Player_WorldOffset()
  {
  }

  public int id { get; set; }

  [ForeignKey("player_id")]
  public int player_id { get; set; }
  public Player Player { get; set; } = new();

  public float x { get; set; }
  public float y { get; set; }
  public float z { get; set; }
  public float Roll { get; set; }
  public float Pitch { get; set; }
  public float Yaw { get; set; }

  public async Task SaveAsync(Player_WorldOffset Player_WorldOffset)
  {
    this.x = Player_WorldOffset.x;
    this.y = Player_WorldOffset.y;
    this.z = Player_WorldOffset.z;
    this.Roll = Player_WorldOffset.Roll;
    this.Pitch = Player_WorldOffset.Pitch;
    this.Yaw = Player_WorldOffset.Yaw;
  }
}