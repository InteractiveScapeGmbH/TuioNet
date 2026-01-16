using System.Numerics;
using TuioNet.Common;
using TuioNet.Server;
using TuioNet.Tuio11;

namespace TuioNet.ServerDemo;

public class Tuio11DataGenerator : TuioDataGeneratorBase
{
    private readonly Tuio11Cursor _cursor;
    private readonly Tuio11Object _object;
    
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

    public override void Update(float deltaTime)
    {
        var time = TuioTime.GetSystemTime();
        var cursorPosition = MoveOnCircle(0.2f, deltaTime, ref _touchAngle);
        var objectPosition = MoveOnCircle( 0.5f, deltaTime, ref _objectAngle);
        _cursor.Update(time, cursorPosition);
        _object.Update(time, objectPosition, _objectAngle);
    }

    
}