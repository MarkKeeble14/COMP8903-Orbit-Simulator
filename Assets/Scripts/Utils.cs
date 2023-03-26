using System;
using System.Globalization;
using UnityEngine;

public class Utils
{
    public static string ConvVector3ToString(Vector3 v)
    {
        return "<" + v.x + ", " + v.y + ", " + v.z + ">";
    }

    public static string ConvVector3ToString(Vector3 v, int roundTo)
    {
        return "<" + RoundTo(v.x, roundTo) + ", " + RoundTo(v.y, roundTo) + ", " + RoundTo(v.z, roundTo) + ">";
    }

    public static string ConvVector3ToString(Vector3 v, int roundTo, string format)
    {
        return string.Format(format, RoundTo(v.x, roundTo), RoundTo(v.y, roundTo), RoundTo(v.z, roundTo));
    }

    public static float RoundTo(float v, int numDigits)
    {
        return (float)System.Math.Round(v, numDigits);
    }

    public static bool ParseFloat(string s, out float v)
    {
        try
        {
            v = float.Parse(s, CultureInfo.InvariantCulture);
            return true;
        }
        catch
        {
            //
            Debug.LogWarning("Attempted an Invalid Parse");
            v = 0;
            return false;
        }
    }

    public static string GetRepeatingString(string s, int repeat)
    {
        string r = "";
        for (int i = 0; i < repeat; i++)
            r += s;
        return r;
    }

    public static int GetNumDigits(int v)
    {
        return v == 0 ? 1 : (int)Math.Floor(Math.Log10(Math.Abs(v)) + 1);
    }

    public static int GetMaxDigits(Vector3 v3)
    {
        int max = 1;
        int x = GetNumDigits((int)Math.Truncate(v3.x));
        if (x > max)
            max = x;
        int y = GetNumDigits((int)Math.Truncate(v3.y));
        if (y > max)
            max = y;
        int z = GetNumDigits((int)Math.Truncate(v3.z));
        if (z > max)
            max = z;
        return max;
    }
}
