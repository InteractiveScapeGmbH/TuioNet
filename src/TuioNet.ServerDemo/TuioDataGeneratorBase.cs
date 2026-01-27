using System.Numerics;
using TuioNet.Common;

namespace TuioNet.ServerDemo;

public abstract class TuioDataGeneratorBase
{
    private readonly Vector2 _center = new Vector2(0.5f, 0.5f);
    private const float Radius = 0.4f;
    private const int MinTokenId = 1;
    private const int MaxTokenId = 30;
    protected const float POINTER_ANGULAR_SPEED = 0.2f;
    protected const float TOKEN_ANGULAR_SPEED = 0.5f;

    private readonly Random _pointerRand = new ();
    private readonly Random _tokenRand = new ();
    private readonly Random _idRand = new();

    protected uint NextId => (uint)_idRand.Next(MinTokenId, MaxTokenId);
    

    protected float PointerAngle = 0f;
    protected float TokenAngle = 0f;
    protected Vector2 MoveOnCircle(float angularSpeed, float deltaTime, ref float angle)
    {
        angle += angularSpeed * deltaTime;
        return new Vector2(_center.X + MathF.Cos(angle) * Radius, _center.Y + MathF.Sin(angle) * Radius);
    }
    public abstract void Update(float deltaTime);

    protected abstract void AddPointer(TuioTime time);
    protected abstract void UpdatePointer(TuioTime time, float deltaTime);

    protected abstract void RemovePointer();

    protected abstract void AddToken(TuioTime time);
    protected abstract void UpdateToken(TuioTime time, float deltaTime);
    protected abstract void RemoveToken();

    protected void RandomPointerAction<T>(ref T entity, TuioTime time, float deltaTime) where T: ITuioEntity?
    {
        RandomAction(ref entity, time, deltaTime, _pointerRand, AddPointer, UpdatePointer, RemovePointer);
    }

    protected void RandomTokenAction<T>(ref T entity, TuioTime time, float deltaTime)
        where T : ITuioEntity?
    {
        RandomAction(ref entity, time, deltaTime, _tokenRand, AddToken, UpdateToken, RemoveToken);
    }

    private static void RandomAction<T>(ref T entity, TuioTime time, float deltaTime, Random rand, Action<TuioTime> add, Action<TuioTime, float> update, Action remove) where T: ITuioEntity?
    {
        if (entity == null)
        {
            var shouldPlace = rand.Next(0, 2) == 0;
            if (shouldPlace)
            {
                add(time);
            }
        }
        else
        {
            var shouldRemove = rand.Next(0, 2) == 0;
            if (shouldRemove)
            {
                remove();
            }
            else
            {
                update(time, deltaTime);
            }
        }
    }
}