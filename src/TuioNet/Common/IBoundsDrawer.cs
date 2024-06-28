using System.Numerics;

namespace TuioNet.Common;

public interface IBoundsDrawer
{
    public Vector2 Position { get; }
    public Vector2 Size { get; }
    public float Angle { get; }
    public string DebugText { get; }
}