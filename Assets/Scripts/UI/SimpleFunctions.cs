using UnityEngine;
public class SimpleFunctions
{
    public static float Linear(float f)
    {
        return f;
    }
    public static float Square(float f)
    {
        return f * f;
    }
    public static float Cubic(float f)
    {
        return f * f * f;
    }
    public static float SmoothStep(float f)
    {
        return f * f * (3 - 2 * f);
    }
    public static float SquareRoot(float f)
    {
        return Mathf.Sqrt(f);
    }
    public static float SquareNegativePlusOneXY(float f)
    {
        return f * (2 - f);
    }
}
