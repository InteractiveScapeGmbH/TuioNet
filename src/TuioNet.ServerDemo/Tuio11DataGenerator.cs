using System.Numerics;
using TuioNet.Common;
using TuioNet.Server;
using TuioNet.Tuio11;

namespace TuioNet.ServerDemo;

public class Tuio11DataGenerator : ITuioDataGenerator
{
    private readonly Tuio11Cursor _cursor;
    private readonly Tuio11Object _object;
    private readonly Vector2 _center = new Vector2(0.5f, 0.5f);
    private const float TouchRadius = 0.4f;
    private const float ObjectRadius = 0.2f;
    private float _touchAngle = 0f;
    private float _objectAngle = 0f;
    
    public Tuio11DataGenerator(Tuio11Manager manager)
    {
        _cursor = new Tuio11Cursor(TuioTime.GetSystemTime(), manager.CurrentSessionId, 0, Vector2.Zero, Vector2.Zero, 0f);
        manager.AddCursor(_cursor);
        _object = new Tuio11Object(TuioTime.GetSystemTime(), manager.CurrentSessionId, 5, Vector2.Zero, 0,
            Vector2.Zero, 0, 0, 0);
        manager.AddObject(_object);
    }

    public void Update(float deltaTime)
    {
        var time = TuioTime.GetSystemTime();
        var cursorPosition = MoveOnCircle(TouchRadius, 0.2f, deltaTime, ref _touchAngle);
        var objectPosition = MoveOnCircle(ObjectRadius, 0.5f, deltaTime, ref _objectAngle);
        _cursor.Update(time, cursorPosition);
        _object.Update(time, objectPosition, _objectAngle);
    }

    private Vector2 MoveOnCircle(float radius, float angularSpeed, float deltaTime, ref float angle)
    {
        angle += angularSpeed * deltaTime;
        return new Vector2(_center.X + MathF.Cos(angle) * radius, _center.Y + MathF.Sin(angle) * radius);
    }
}