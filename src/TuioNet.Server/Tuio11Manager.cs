using OSC.NET;
using TuioNet.Tuio11;

namespace TuioNet.Server
{
    
public class Tuio11Manager : ITuioManager
{
    private readonly Tuio11Repository _blobRepository;
    private readonly Tuio11Repository _cursorRepository;
    private readonly Tuio11Repository _objectRepository;

    private OSCBundle _frameBundle;
    private uint _frameId = 1;

    public Tuio11Manager(string sourceName)
    {
        _cursorRepository = new Tuio11Repository(sourceName, "/tuio/2Dcur");
        _objectRepository = new Tuio11Repository(sourceName, "/tuio/2Dobj");
        _blobRepository = new Tuio11Repository(sourceName, "/tuio/2Dblb");
    }

    public uint CurrentSessionId { get; private set; }

    public OSCBundle FrameBundle
    {
        get
        {
            UpdateFrameBundle();
            return _frameBundle;
        }
    }


    public void Update()
    {
        _frameId += 1;
        _cursorRepository.Update(_frameId);
        _objectRepository.Update(_frameId);
        _blobRepository.Update(_frameId);
    }

    public void Quit()
    {
        _cursorRepository.Clear();
        _objectRepository.Clear();
        _blobRepository.Clear();
        Update();
    }

    public void AddCursor(Tuio11Cursor tuioCursor)
    {
        _cursorRepository.Add(tuioCursor);
        CurrentSessionId++;
    }

    public void RemoveCursor(Tuio11Cursor tuio11Cursor)
    {
        _cursorRepository.Remove(tuio11Cursor);
    }

    public void AddObject(Tuio11Object tuioObject)
    {
        _objectRepository.Add(tuioObject);
        CurrentSessionId++;
    }

    public void RemoveObject(Tuio11Object tuioObject)
    {
        _objectRepository.Remove(tuioObject);
    }

    public void AddBlob(Tuio11Blob tuioBlob)
    {
        _blobRepository.Add(tuioBlob);
        CurrentSessionId++;
    }

    public void RemoveBlob(Tuio11Blob tuioBlob)
    {
        _blobRepository.Remove(tuioBlob);
    }

    private void UpdateFrameBundle()
    {
        _frameBundle = new OSCBundle(OscTimeTag.Immediate);
        _cursorRepository.UpdateBundle(_frameBundle);
        _objectRepository.UpdateBundle(_frameBundle);
        _blobRepository.UpdateBundle(_frameBundle);
    }
}
}
