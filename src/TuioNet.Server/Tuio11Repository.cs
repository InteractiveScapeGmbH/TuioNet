using System.Collections.Generic;
using TuioNet.Common;
using TuioNet.OSC;

namespace TuioNet.Server
{
    
public class Tuio11Repository
{
    private readonly string _sourceName;

    private readonly string _tuioAddress;
    private readonly List<ITuioEntity> _entities;

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

    public OSCBundle Bundle()
    {
        var bundle = new OSCBundle(OscTimeTag.Immediate);
        bundle.Append(SourceMessage);
        bundle.Append(AliveMessage);
        foreach (var entity in _entities) bundle.Append(entity.OscMessage);
        bundle.Append(FseqMessage);
        return bundle;
    }
}
}
