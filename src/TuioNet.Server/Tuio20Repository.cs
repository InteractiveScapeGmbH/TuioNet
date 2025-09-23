using System;
using System.Collections.Generic;
using System.Numerics;
using OSC.NET;
using TuioNet.Common;

namespace TuioNet.Server
{
    
public class Tuio20Repository
{
    private readonly List<ITuioEntity> _entities;
    private readonly string _sourceName;
    private uint _frameId = 1;
    private readonly uint _screenResolution;
    private TuioTime _time;

    public Tuio20Repository(string sourceName, Vector2 screenResolution)
    {
        _sourceName = sourceName;
        _screenResolution = Utils.FromDimension(screenResolution);
        _entities = new List<ITuioEntity>();
    }

    private OSCMessage AliveMessage
    {
        get
        {
            var message = new OSCMessage("/tuio2/alv");
            foreach (var entity in _entities) message.Append((int)entity.SessionId);
            return message;
        }
    }

    private OSCMessage FrameMessage
    {
        get
        {
            var message = new OSCMessage("/tuio2/frm");
            message.Append((int)_frameId);
            message.Append(new OscTimeTag(DateTime.Now));
            message.Append((int)_screenResolution);
            message.Append(_sourceName);
            return message;
        }
    }

    public void Update(uint frameId)
    {
        _frameId = frameId;
        _time = TuioTime.GetSystemTime();
    }

    public void AddEntity(ITuioEntity entity)
    {
        _entities.Add(entity);
    }

    public void RemoveEntity(ITuioEntity entity)
    {
        _entities.Remove(entity);
    }

    public OSCBundle UpdateBundle(OSCBundle bundle)
    {
        bundle.Append(FrameMessage);
        foreach (var entity in _entities) bundle.Append(entity.OscMessage);
        bundle.Append(AliveMessage);
        return bundle;
    }

    public void Clear()
    {
        _entities.Clear();
    }
}
}
