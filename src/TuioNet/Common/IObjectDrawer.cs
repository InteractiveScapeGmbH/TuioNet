using Stride.Core.Mathematics;

namespace TuioNet.Common;

public interface IObjectDrawer
{
    public Vector2 Position { get; }
    public float Angle { get; }
    public string DebugText { get; }
}