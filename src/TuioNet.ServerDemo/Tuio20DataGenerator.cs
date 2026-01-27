using System.Numerics;
using Microsoft.Extensions.Logging;
using TuioNet.Common;
using TuioNet.Server;
using TuioNet.Tuio20;

namespace TuioNet.ServerDemo;

public class Tuio20DataGenerator : TuioDataGeneratorBase
{
    private Tuio20Pointer? _pointer;
    private Tuio20Token? _token;
    
    
    private readonly Tuio20Manager _manager;

    private readonly ILogger _logger;
    
    public Tuio20DataGenerator(Tuio20Manager manager, ILogger logger)
    {
        _manager = manager;
        _logger = logger;
    }
    public override void Update(float deltaTime)
    {
        var time = TuioTime.GetSystemTime();
        RandomPointerAction(ref _pointer, time, deltaTime);
        RandomTokenAction(ref _token, time, deltaTime);
    }

    protected override void AddPointer(TuioTime time)
    {
        var pointerContainer = new Tuio20Object(time, _manager.CurrentSessionId);
        _pointer = new Tuio20Pointer(time, pointerContainer, 0, 0, Vector2.Zero, 0f, 0f, 0f, 0f, Vector2.Zero, 0f, 0f,
            0f);
        _manager.AddEntity(_pointer);
        _logger.LogInformation("Add Pointer: {pointer}", _pointer);
    }

    protected override void UpdatePointer(TuioTime time, float deltaTime)
    {
        var pointerPosition = MoveOnCircle(POINTER_ANGULAR_SPEED, deltaTime, ref PointerAngle);
        _pointer?.Update(time, 0, 0, pointerPosition, 0, 0, 0, 0, Vector2.Zero, 0, 0, 0);
        
    }

    protected override void RemovePointer()
    {
        if (_pointer == null) return;
       _manager.RemoveEntity(_pointer);
       _logger.LogInformation("Remove Pointer: {pointer}", _pointer);
       _pointer = null;
    }

    protected override void AddToken(TuioTime time)
    {
        var tokenContainer = new Tuio20Object(time, _manager.CurrentSessionId);
        _token = new Tuio20Token(time, tokenContainer, 0, NextId, Vector2.Zero, 0, Vector2.Zero, 0, 0, 0);
        _manager.AddEntity(_token);
        _logger.LogInformation("Add Token: {token}", _token);
        
    }

    protected override void UpdateToken(TuioTime time, float deltaTime)
    {
        var tokenPosition = MoveOnCircle(TOKEN_ANGULAR_SPEED, deltaTime, ref TokenAngle);
        _token?.Update(time, 0, _token.ComponentId, tokenPosition, TokenAngle, Vector2.Zero, 0, 0, 0);

    }

    protected override void RemoveToken()
    {
        if (_token == null) return;
        _manager.RemoveEntity(_token);
        _logger.LogInformation("Remove Token: {token}", _token);
        _token = null;
    }
}