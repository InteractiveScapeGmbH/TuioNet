using System;
using System.Numerics;

namespace TuioNet.Common;

public class Utils
{
    public static uint FromDimension(Vector2 dimension)
    {
        var width = (ushort)MathF.Round(dimension.X);
        var height = (ushort)MathF.Round(dimension.Y);
        return ((uint)width << 16) | height;
    }

    public static Vector2 ToDimension(uint encoded)
    {
        var width = (ushort)(encoded >> 16);
        var height = (ushort)(encoded & 0xFFFF);
        return new Vector2(width, height);
    }
}