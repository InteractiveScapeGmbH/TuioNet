using System.Numerics;
using Microsoft.Extensions.Logging;
using TuioNet.Common;
using TuioNet.Server;
using TuioNet.Tuio11;

namespace TuioNet.ServerDemo;

public class Tuio11DataGenerator : TuioDataGeneratorBase
{
    private Tuio11Cursor? _cursor;
    private Tuio11Object? _object;
    
    private readonly Tuio11Manager _manager;

    private readonly ILogger _logger;
    
    public Tuio11DataGenerator(Tuio11Manager manager, ILogger logger)
    {
        _manager = manager;
        _logger = logger;
    }

    public override void Update(float deltaTime)
    {
        var time = TuioTime.GetSystemTime();
        RandomPointerAction(ref _cursor, time, deltaTime);
        RandomTokenAction(ref _object, time, deltaTime);
        
    }

    protected override void AddPointer(TuioTime time)
    {
        _cursor = new Tuio11Cursor(time, _manager.CurrentSessionId, 0, Vector2.Zero, Vector2.Zero, 0f);
        _manager.AddCursor(_cursor);
        _logger.LogInformation("Add Cursor: {cursor}", _cursor);
    }

    protected override void UpdatePointer(TuioTime time, float deltaTime)
    {
        var cursorPosition = MoveOnCircle(POINTER_ANGULAR_SPEED, deltaTime, ref PointerAngle);
        _cursor?.Update(time, cursorPosition);
    }

    protected override void RemovePointer()
    {
        if (_cursor == null) return;
        _manager.RemoveCursor(_cursor);
        _logger.LogInformation("Remove Cursor: {cursor}", _cursor);
        _cursor = null;
    }

    protected override void AddToken(TuioTime time)
    {
        _object = new Tuio11Object(time, _manager.CurrentSessionId, NextId, Vector2.Zero, 0,
            Vector2.Zero, 0, 0, 0);
        _manager.AddObject(_object);
        _logger.LogInformation("Add Object: {object}", _object);
    }

    protected override void UpdateToken(TuioTime time, float deltaTime)
    {
        var objectPosition = MoveOnCircle( TOKEN_ANGULAR_SPEED, deltaTime, ref TokenAngle);
        _object?.Update(time, objectPosition, TokenAngle);
    }

    protected override void RemoveToken()
    {
        if (_object == null) return;
        _manager.RemoveObject(_object);
        _logger.LogInformation("Remove Object: {object}", _object);
        _object = null;
    }
    
}