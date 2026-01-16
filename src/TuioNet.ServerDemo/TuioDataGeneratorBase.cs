using System.Numerics;

namespace TuioNet.ServerDemo;

public abstract class TuioDataGeneratorBase
{
    private readonly Vector2 _center = new Vector2(0.5f, 0.5f);
    private const float Radius = 0.4f;
    
    public abstract void Update(float deltaTime);
    protected Vector2 MoveOnCircle(float angularSpeed, float deltaTime, ref float angle)
    {
        angle += angularSpeed * deltaTime;
        return new Vector2(_center.X + MathF.Cos(angle) * Radius, _center.Y + MathF.Sin(angle) * Radius);
    }
}