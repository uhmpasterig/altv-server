using AltV.Net.Async.Elements.Entities;
using server.Models;
using AltV.Net;
using AltV.Net.Data;

namespace server.Core;

public partial class xVehicle
{
  public Vehicle_WorldOffset WorldOffset
  {
    get
    {
      return new Vehicle_WorldOffset()
      {
        x = this.Position.X,
        y = this.Position.Y,
        z = this.Position.Z,
        Roll = this.Rotation.Roll,
        Pitch = this.Rotation.Pitch,
        Yaw = this.Rotation.Yaw
      };
    }
    set
    {
      this.Position = new Position(value.x, value.y, value.z);
      this.Rotation = new Rotation(value.Roll, value.Pitch, value.Yaw);
    }
  }

  private async Task _loadWorldOffset(Vehicle_WorldOffset WorldOffset)
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
}