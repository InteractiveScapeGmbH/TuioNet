using System.Collections.Generic;
using OSC.NET;
using TuioNet.Common;

namespace TuioNet.Server
{
    
public class Tuio11Repository
{
    private readonly string _sourceName;

    private readonly string _tuioAddress;
    private readonly IList<ITuioEntity> _entities;

    private uint _frameId = 1;

    public Tuio11Repository(string sourceName, string tuioAddress)
    {
        _sourceName = sourceName;
        _tuioAddress = tuioAddress;
        _entities = new List<ITuioEntity>();
    }

    private OSCMessage SourceMessage
    {
        get
        {
            var message = new OSCMessage(_tuioAddress);
            message.Append("source");
            message.Append(_sourceName);
            return message;
        }
    }

    private OSCMessage AliveMessage
    {
        get
        {
            var message = new OSCMessage(_tuioAddress);
            message.Append("alive");
            foreach (var entity in _entities) message.Append((int)entity.SessionId);

            return message;
        }
    }

    private OSCMessage FseqMessage
    {
        get
        {
            var message = new OSCMessage(_tuioAddress);
            message.Append("fseq");
            message.Append((int)_frameId);
            return message;
        }
    }

    public void Update(uint frameId)
    {
        _frameId = frameId;
    }

    public void Add(ITuioEntity entity)
    {
        _entities.Add(entity);
    }

    public void Remove(ITuioEntity entity)
    {
        _entities.Remove(entity);
    }

    public void Clear()
    {
        _entities.Clear();
    }

    public OSCBundle UpdateBundle(OSCBundle bundle)
    {
        if (!(_entities.Count > 0)) return bundle;
        bundle.Append(SourceMessage);
        bundle.Append(AliveMessage);
        foreach (var entity in _entities) bundle.Append(entity.OscMessage);
        bundle.Append(FseqMessage);
        return bundle;
    }
}
}
