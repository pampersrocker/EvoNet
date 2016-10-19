using System;

namespace EvoNet
{
  public class Mathf
  {
    public const float PI = 3.1415926535f;
    public const float DegreeToRad = PI / 180.0f;
    public const float RadToDegree = 180.0f / PI;

    public static float Sin(float Value)
    {
      return (float)Math.Sin(Value);
    }

    public static float Cos(float Value)
    {
      return (float)Math.Cos(Value);
    }

    public static float Tan(float Value)
    {
      return (float)Math.Tan(Value);
    }

    public static float Asin(float Value)
    {
      return (float)Math.Asin(Value);
    }

    public static float Acos(float Value)
    {
      return (float)Math.Acos(Value);
    }

    public static float Atan(float Value)
    {
      return (float)Math.Atan(Value);
    }
  }
}
