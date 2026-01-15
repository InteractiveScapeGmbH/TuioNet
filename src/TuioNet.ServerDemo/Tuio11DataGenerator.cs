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
    
    public Tuio11DataGenerator(Tuio11Manager manager)
    {
        _manager = manager;
        _cursor = new Tuio11Cursor(TuioTime.GetSystemTime(), _manager.CurrentSessionId, 0, Vector2.Zero, Vector2.Zero, 0f);
        _manager.AddCursor(_cursor);
    }

    public void Update()
    {
        var time = TuioTime.GetSystemTime();
        var position = _cursor.Position + new Vector2(0.1f, 0.1f);
        _cursor.Update(time, position, Vector2.Zero, 0);
    }
    
}