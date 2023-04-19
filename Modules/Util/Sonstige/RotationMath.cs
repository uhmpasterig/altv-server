namespace server.Core;

class RotationMath
{
  public static float YawToHeading(float yaw)
  {
    float heading = (yaw + (MathF.PI * 2)) % (MathF.PI * 2);
    heading *= 180.0f / MathF.PI;
    return heading;
  }
}