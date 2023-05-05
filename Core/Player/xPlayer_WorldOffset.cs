using AltV.Net.Async.Elements.Entities;
using server.Models;
using AltV.Net;
using AltV.Net.Data;

namespace server.Core;

public partial class xPlayer
{
  private Position? cachePosition { get; set; }
  private Rotation? cacheRotation { get; set; }

  public Player_WorldOffset WorldOffset
  {
    get
    {
      Position pos = cachePosition ?? this.Position;
      Rotation rot = cacheRotation ?? this.Rotation;

      return new Player_WorldOffset()
      {
        x = pos.X,
        y = pos.Y,
        z = pos.Z,
        Roll = rot.Roll,
        Pitch = rot.Pitch,
        Yaw = rot.Yaw
      };
    }
    set
    {
      this.Position = new Position(value.x, value.y, value.z);
      this.Rotation = new Rotation(value.Roll, value.Pitch, value.Yaw);
    }
  }

  private async Task _loadWorldOffset(Player_WorldOffset WorldOffset)
  {
    this.WorldOffset = WorldOffset;
  }

  public async Task<Position> GetPosition()
  {
    return new Position(this.Position.X, this.Position.Y, this.Position.Z);
  }

  public async Task<Rotation> GetRotation()
  {
    return new Rotation(this.Rotation.Roll, this.Rotation.Pitch, this.Rotation.Yaw);
  }

  public async Task SetDimension(int dimension)
  {
    if (dimension != 1)
    {
      this.cachePosition = this.Position;
      this.cacheRotation = this.Rotation;
    }
    else
    {
      this.cachePosition = null;
      this.cacheRotation = null;
    }

    this.Dimension = dimension;
  }
}