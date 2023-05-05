using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using AltV.Net.Data;

namespace server.Models;

[Table("vehicle_worldoffsets")]
[PrimaryKey("id")]
public partial class Vehicle_WorldOffset
{
  public Vehicle_WorldOffset()
  {
  }

  public int id { get; set; }

  [ForeignKey("vehicle_id")]
  public int vehicle_id { get; set; }
  public Vehicle Vehicle { get; set; } = new();

  public float x { get; set; }
  public float y { get; set; }
  public float z { get; set; }
  public float Roll { get; set; }
  public float Pitch { get; set; }
  public float Yaw { get; set; }

  public async Task SaveAsync(Vehicle_WorldOffset Vehicle_WorldOffset)
  {
    this.x = Vehicle_WorldOffset.x;
    this.y = Vehicle_WorldOffset.y;
    this.z = Vehicle_WorldOffset.z;
    this.Roll = Vehicle_WorldOffset.Roll;
    this.Pitch = Vehicle_WorldOffset.Pitch;
    this.Yaw = Vehicle_WorldOffset.Yaw;
  }

  [NotMapped]
  public Position Position
  {
    get { return new Position(this.x, this.y, this.z); }
    set { this.x = value.X; this.y = value.Y; this.z = value.Z; }
  }

  [NotMapped]
  public Rotation Rotation
  {
    get { return new Rotation(this.Roll, this.Pitch, this.Yaw); }
    set { this.Roll = value.Roll; this.Pitch = value.Pitch; this.Yaw = value.Yaw; }
  }
}