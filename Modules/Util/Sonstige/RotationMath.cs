namespace server.Core;

class RotationMath
{

  // jaw is -pi to pi and heading should be 0.0 to 360.0
  public static float JawToHeading(float jaw)
  {
    jaw = jaw + (float)Math.PI;
    float heading = (float)(jaw * (180.0 / Math.PI));
    return heading;
  }
}