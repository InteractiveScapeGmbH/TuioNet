using System.Numerics;
using TuioNet.Common;
using TuioNet.Server;
using TuioNet.Tuio20;

namespace TuioNet.ServerDemo;

public class Tuio20DataGenerator : TuioDataGeneratorBase
{
    private readonly Tuio20Pointer _pointer;
    private readonly Tuio20Token _token;
    
    private float _touchAngle = 0f;
    private float _tokenAngle = 0f;
    
    public Tuio20DataGenerator(Tuio20Manager manager)
    {
        var time = TuioTime.GetSystemTime();
        var pointerContainer = new Tuio20Object(time, manager.CurrentSessionId);
        _pointer = new Tuio20Pointer(time, pointerContainer, 0, 0, Vector2.Zero, 0f, 0f, 0f, 0f, Vector2.Zero, 0f, 0f,
            0f);
        manager.AddEntity(_pointer);

        var tokenContainer = new Tuio20Object(time, manager.CurrentSessionId);
        _token = new Tuio20Token(time, tokenContainer, 0, 5, Vector2.Zero, 0, Vector2.Zero, 0, 0, 0);
        manager.AddEntity(_token);
    }
    public override void Update(float deltaTime)
    {
        var time = TuioTime.GetSystemTime();
        var pointerPosition = MoveOnCircle(0.2f, deltaTime, ref _touchAngle);
        var tokenPosition = MoveOnCircle(0.5f, deltaTime, ref _tokenAngle);
        _pointer.Update(time, 0, 0, pointerPosition, 0, 0, 0, 0, Vector2.Zero, 0, 0, 0);
        _token.Update(time, 0, 5, tokenPosition, _tokenAngle, Vector2.Zero, 0, 0, 0);
    }
}