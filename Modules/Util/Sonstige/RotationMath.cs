namespace server.Core;

class RotationMath
{

  // jaw is -pi to pi and heading should be 0.0 to 360.0
  public float YawToHeading(float yaw)
{
    float heading = (yaw + (MathF.PI * 2)) % (MathF.PI * 2);
    heading *= 180.0f / MathF.PI;
    return heading;
}
}