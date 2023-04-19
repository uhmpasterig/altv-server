namespace server.Core;

class RotationMath
{
  public static float JawToHeading(float jaw)
  {
    jaw = jaw * 180 / 3.14159265358979323846f;
    jaw = jaw + 90;
    if (jaw < 0)
    {
      jaw = jaw + 360;
    }
    jaw = jaw % 360;
    jaw = jaw * 3.14159265358979323846f / 180;
    return jaw;
  }
}