using UnityEngine;
using System;

public static class MathHelper
{
    
    public static double Lerp(double a, double b, double t)
    {
        return a * (1 - t) + b * t;
    }

    public static float Lerp(float a, float b, float t)
    {
        return a * (1 - t) + b * t;
    }

    public static float Fract(float x)
    {
        return x - Mathf.Floor(x);
    }

    public static double Fract(double x)
    {
        return x - Math.Floor(x);
    }

    public static double Clamp(double value, double min, double max)
    {
        if (value > max)
            return max;
        if (value < min)
            return min;

        return value;
    }

    public static float Clamp(float value, float min, float max)
    {
        if (value > max)
            return max;
        if (value < min)
            return min;

        return value;
    }

    public static float Clamp01(float value)
    {
        if (value > 1)
            return 1;
        if (value < 0)
            return 0;

        return value;
    }


    public static double Smoothstep(double min, double max, double value)
    {
        double t = Clamp((value - min) / (max - min), 0.0, 1.0);
        return t * t * (3.0 - 2.0 * t);
    }

}
