using System.Numerics;

namespace TuioNet.Common;

public interface ITouchDrawer
{
    public Vector2 Position { get; }
    public string DebugText { get; }
}