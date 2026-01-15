using System.Numerics;
using TuioNet.Common;
using TuioNet.Server;
using TuioNet.Tuio11;

namespace TuioNet.ServerDemo;

public class Tuio11DataGenerator : ITuioDataGenerator
{
    private Tuio11Manager _manager;
    private readonly Tuio11Cursor _cursor;
    private readonly Tuio11Object _object;
    private Vector2 _center = new Vector2(0.5f, 0.5f);
    private float _touchRadius = 0.4f;
    private float _objectRadius = 0.2f;
    private float _touchAngle = 0f;
    private float _objectAngle = 0f;
    
    public Tuio11DataGenerator(Tuio11Manager manager)
    {
        _manager = manager;
        _cursor = new Tuio11Cursor(TuioTime.GetSystemTime(), _manager.CurrentSessionId, 0, Vector2.Zero, Vector2.Zero, 0f);
        _manager.AddCursor(_cursor);
    }

    public void Update(float deltaTime)
    {
        var time = TuioTime.GetSystemTime();
        var position = MoveOnCircle(_touchRadius, 0.2f, deltaTime, ref _touchAngle);
        _cursor.Update(time, position, Vector2.Zero, 0);
    }

    private Vector2 MoveOnCircle(float radius, float angularSpeed, float deltaTime, ref float angle)
    {
        angle += angularSpeed * deltaTime;
        return new Vector2(_center.X + MathF.Cos(angle) * radius, _center.Y + MathF.Sin(angle) * radius);
    }
}