using AltV.Net.Data;

namespace server.Core;

class RotationMath
{
  public static float YawToHeading(float yaw)
  {
    float heading = (yaw + (MathF.PI * 2)) % (MathF.PI * 2);
    heading *= 180.0f / MathF.PI;
    return heading;
  }

  public static Position GetPositionInFrontOf(Position pos, float heading, float distance)
  {
    float x = pos.X + (distance * MathF.Cos(heading));
    float y = pos.Y + (distance * MathF.Sin(heading));
    return new Position(x, y, pos.Z);
  }
}