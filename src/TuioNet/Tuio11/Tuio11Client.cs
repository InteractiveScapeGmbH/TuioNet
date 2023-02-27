using System.Collections.Generic;
using System.Linq;
using OSC.NET;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Client
    {
        private TuioReceiver _tuioReceiver;
        private List<Tuio11Listener> _tuioListeners = new List<Tuio11Listener>();

        private Dictionary<uint, Tuio11Object> _tuioObjects = new Dictionary<uint, Tuio11Object>();
        private Dictionary<uint, Tuio11Cursor> _tuioCursors = new Dictionary<uint, Tuio11Cursor>();
        private Dictionary<uint, Tuio11Blob> _tuioBlobs = new Dictionary<uint, Tuio11Blob>();

        private List<OSCMessage> _objectSetMessages = new List<OSCMessage>();
        private List<OSCMessage> _cursorSetMessages = new List<OSCMessage>();
        private List<OSCMessage> _blobSetMessages = new List<OSCMessage>();

        private OSCMessage _objectAliveMessage = null;
        private OSCMessage _cursorAliveMessage = null;
        private OSCMessage _blobAliveMessage = null;

        private object _objectLock = new object();
        private object _cursorLock = new object();
        private object _blobLock = new object();

        private List<uint> _freeCursorIds = new List<uint>();
        private List<uint> _freeBlobIds = new List<uint>();

        private uint _currentFrame = 0;
        private TuioTime _currentTime = null;

        public Tuio11Client(TuioReceiver tuioReceiver)
        {
            _tuioReceiver = tuioReceiver;
            _tuioReceiver.AddMessageListener("/tuio/2Dobj", On2Dobj);
            _tuioReceiver.AddMessageListener("/tuio/2Dcur", On2Dcur);
            _tuioReceiver.AddMessageListener("/tuio/2Dblb", On2Dblb);
        }
        
        public void Connect()
        {
            TuioTime.Init();
            _currentTime = TuioTime.GetCurrentTime();
            _tuioReceiver.Connect();
        }

        public void Disconnect()
        {
            _tuioReceiver.Disconnect();
        }

        public object objectLock => _objectLock;
        public object cursorLock => _cursorLock;
        public object blobLock => _blobLock;

        public bool IsConnected()
        {
            return _tuioReceiver.IsConnected;
        }

        public void AddTuioListener(Tuio11Listener tuio11Listener)
        {
            _tuioListeners.Add(tuio11Listener);
        }

        public void RemoveTuioListener(Tuio11Listener tuio11Listener)
        {
            _tuioListeners.Remove(tuio11Listener);
        }

        public void RemoveAllTuioListeners()
        {
            _tuioListeners.Clear();
        }

        public List<Tuio11Object> GetTuioObjects()
        {
            return _tuioObjects.Values.ToList();
        }

        public List<Tuio11Cursor> GetTuioCursors()
        {
            return _tuioCursors.Values.ToList();
        }

        public List<Tuio11Blob> GetTuioBlobs()
        {
            return _tuioBlobs.Values.ToList();
        }

        public Tuio11Object GetTuioObject(uint sessionId)
        {
            return _tuioObjects[sessionId];
        }

        public Tuio11Cursor GetTuioCursor(uint sessionId)
        {
            return _tuioCursors[sessionId];
        }

        public Tuio11Blob GetTuioBlob(uint sessionId)
        {
            return _tuioBlobs[sessionId];
        }

        private bool _updateFrame(uint fseq)
        {
            var currentTime = TuioTime.GetCurrentTime();
            if (fseq > 0)
            {
                if (fseq > _currentFrame)
                {
                    _currentTime = currentTime;
                }

                if (fseq >= _currentFrame || _currentFrame - fseq > 100)
                {
                    _currentFrame = fseq;
                }
                else
                {
                    return false;
                }
            }
            else if ((currentTime - _currentTime).GetTotalMilliseconds() > 100)
            {
                _currentTime = currentTime;
            }

            return true;
        }

        private void On2Dobj(OSCMessage oscMessage)
        {
            var command = (string)oscMessage.Values[0];
            if (command == "set")
            {
                _objectSetMessages.Add(oscMessage);
            }
            else if (command == "alive")
            {
                _objectAliveMessage = oscMessage;
            }
            else if (command == "fseq")
            {
                var fseq = (uint)(int)oscMessage.Values[1];
                if (_updateFrame(fseq))
                {
                    if (_objectAliveMessage != null)
                    {
                        var currentSIds = new HashSet<uint>(_tuioObjects.Keys);
                        var aliveSIds = new HashSet<uint>();
                        for (int i = 1; i < _objectAliveMessage.Values.Count; i++)
                        {
                            aliveSIds.Add((uint)(int)_objectAliveMessage.Values[i]);
                        }

                        var removedSIds = new HashSet<uint>(currentSIds.Except(aliveSIds));
                        foreach (var sId in removedSIds)
                        {
                            var tuioObject = _tuioObjects[sId];
                            tuioObject.Remove();
                            foreach (var tuioListener in _tuioListeners)
                            {
                                tuioListener.RemoveTuioObject(tuioObject);
                            }

                            _tuioObjects.Remove(sId);
                        }

                        foreach (var setMessage in _objectSetMessages)
                        {
                            var sessionId = (uint)(int)setMessage.Values[1];
                            var classId = (uint)(int)setMessage.Values[2];
                            var xPos = (float)setMessage.Values[3];
                            var yPos = (float)setMessage.Values[4];
                            var angle = (float)setMessage.Values[5];
                            var speedX = (float)setMessage.Values[6];
                            var speedY = (float)setMessage.Values[7];
                            var rotationSpeed = (float)setMessage.Values[8];
                            var motionAcceleration = (float)setMessage.Values[9];
                            var rotationAcceleration = (float)setMessage.Values[10];
                            if (aliveSIds.Contains(sessionId))
                            {
                                if (currentSIds.Contains(sessionId))
                                {
                                    var tuioObject = _tuioObjects[sessionId];
                                    tuioObject.UpdateTime(_currentTime);
                                    if (tuioObject.HasChanged(xPos, yPos, angle, speedX, speedY, rotationSpeed, motionAcceleration, rotationAcceleration))
                                    {
                                        tuioObject.Update(_currentTime, xPos, yPos, angle, speedX, speedY, rotationSpeed, motionAcceleration, rotationAcceleration);
                                        foreach (var tuioListener in _tuioListeners)
                                        {
                                            tuioListener.UpdateTuioObject(tuioObject);
                                        }
                                    }
                                }
                                else
                                {
                                    var tuioObject = new Tuio11Object(_currentTime, sessionId, classId, xPos, yPos, angle, speedX, speedY, rotationSpeed, motionAcceleration, rotationAcceleration);
                                    _tuioObjects[sessionId] = tuioObject;
                                    foreach (var tuioListener in _tuioListeners)
                                    {
                                        tuioListener.AddTuioObject(tuioObject);
                                    }
                                }
                            }
                        }

                        foreach (var tuioListener in _tuioListeners)
                        {
                            tuioListener.Refresh(_currentTime);
                        }
                    }
                }

                _objectSetMessages.Clear();
                _objectAliveMessage = null;
            }
        }

        private void On2Dcur(OSCMessage oscMessage)
        {
            var command = (string)oscMessage.Values[0];
            if (command == "set")
            {
                _cursorSetMessages.Add(oscMessage);
            }
            else if (command == "alive")
            {
                _cursorAliveMessage = oscMessage;
            }
            else if (command == "fseq")
            {
                var fseq = (uint)(int)oscMessage.Values[1];
                if (_updateFrame(fseq))
                {
                    if (_cursorAliveMessage != null)
                    {
                        var currentSIds = new HashSet<uint>(_tuioCursors.Keys);
                        var aliveSIds = new HashSet<uint>();
                        for (int i = 1; i < _cursorAliveMessage.Values.Count; i++)
                        {
                            aliveSIds.Add((uint)(int)_cursorAliveMessage.Values[i]);
                        }

                        var removedSIds = new HashSet<uint>(currentSIds.Except(aliveSIds));

                        foreach (var sId in removedSIds)
                        {
                            var tuioCursor = _tuioCursors[sId];
                            tuioCursor.Remove();
                            foreach (var tuioListener in _tuioListeners)
                            {
                                tuioListener.RemoveTuioCursor(tuioCursor);
                            }

                            _tuioCursors.Remove(sId);
                            _freeCursorIds.Add(tuioCursor.CursorId);
                        }
                        _freeCursorIds.Sort();
                        foreach (var setMessage in _cursorSetMessages)
                        {
                            var ses = (uint)(int)setMessage.Values[1];
                            var posX = (float)setMessage.Values[2];
                            var posY = (float)setMessage.Values[3];
                            var speedX = (float)setMessage.Values[4];
                            var speedY = (float)setMessage.Values[5];
                            var motionAcceleration = (float)setMessage.Values[6];
                            if (aliveSIds.Contains(ses))
                            {
                                if (currentSIds.Contains(ses))
                                {
                                    var tuioCursor = _tuioCursors[ses];
                                    if (tuioCursor.HasChanged(posX, posY, speedX, speedY, motionAcceleration))
                                    {
                                        tuioCursor.Update(_currentTime, posX, posY, speedX, speedY, motionAcceleration);
                                        foreach (var tuioListener in _tuioListeners)
                                        {
                                            tuioListener.UpdateTuioCursor(tuioCursor);
                                        }
                                    }
                                }
                                else
                                {
                                    var cursorId = (uint)_tuioCursors.Count;
                                    if (_freeCursorIds.Count > 0)
                                    {
                                        cursorId = _freeCursorIds[0];
                                        _freeCursorIds.RemoveAt(0);
                                    }

                                    var tuioCursor = new Tuio11Cursor(_currentTime, ses, cursorId, posX, posY, speedX, speedY, motionAcceleration);
                                    _tuioCursors[ses] = tuioCursor;
                                    foreach (var tuioListener in _tuioListeners)
                                    {
                                        tuioListener.AddTuioCursor(tuioCursor);
                                    }
                                }
                            }
                        }

                        foreach (var tuioListener in _tuioListeners)
                        {
                            tuioListener.Refresh(_currentTime);
                        }
                    }
                }

                _cursorSetMessages.Clear();
                _cursorAliveMessage = null;
            }
        }

        private void On2Dblb(OSCMessage oscMessage)
        {
            var command = (string)oscMessage.Values[0];
            if (command == "set")
            {
                _blobSetMessages.Add(oscMessage);
            }
            else if (command == "alive")
            {
                _blobAliveMessage = oscMessage;
            }
            else if (command == "fseq")
            {
                var fseq = (uint)(int)oscMessage.Values[1];
                if (_updateFrame(fseq))
                {
                    if (_blobAliveMessage != null)
                    {
                        var currentSIds = new HashSet<uint>(_tuioBlobs.Keys);
                        var aliveSIds = new HashSet<uint>();
                        for (int i = 1; i < _blobAliveMessage.Values.Count; i++)
                        {
                            aliveSIds.Add((uint)(int)_blobAliveMessage.Values[i]);
                        }

                        var removedSIds = new HashSet<uint>(currentSIds.Except(aliveSIds));

                        foreach (var sId in removedSIds)
                        {
                            var tuioBlob = _tuioBlobs[sId];
                            tuioBlob.Remove();
                            foreach (var tuioListener in _tuioListeners)
                            {
                                tuioListener.RemoveTuioBlob(tuioBlob);
                            }

                            _tuioBlobs.Remove(sId);
                            _freeBlobIds.Add(tuioBlob.BlobId);
                        }
                        _freeBlobIds.Sort();
                        foreach (var setMessage in _blobSetMessages)
                        {
                            var sessionId = (uint)(int)setMessage.Values[1];
                            var posX = (float)setMessage.Values[2];
                            var posY = (float)setMessage.Values[3];
                            var angle = (float)setMessage.Values[4];
                            var width = (float)setMessage.Values[5];
                            var height = (float)setMessage.Values[6];
                            var area = (float)setMessage.Values[7];
                            var speedX = (float)setMessage.Values[8];
                            var speedY = (float)setMessage.Values[9];
                            var rotationSpeed = (float)setMessage.Values[10];
                            var motionAcceleration = (float)setMessage.Values[11];
                            var rotationAcceleration = (float)setMessage.Values[12];
                            if (aliveSIds.Contains(sessionId))
                            {
                                if (currentSIds.Contains(sessionId))
                                {
                                    var tuioBlob = _tuioBlobs[sessionId];
                                    if (tuioBlob.HasChanged(posX, posY, angle, width, height, area, speedX, speedY, rotationSpeed, motionAcceleration, rotationAcceleration))
                                    {
                                        tuioBlob.Update(_currentTime, posX, posY, angle, width, height, area, speedX, speedY, rotationSpeed, motionAcceleration, rotationAcceleration);
                                        foreach (var tuioListener in _tuioListeners)
                                        {
                                            tuioListener.UpdateTuioBlob(tuioBlob);
                                        }
                                    }
                                }
                                else
                                {
                                    var blobId = (uint)_tuioBlobs.Count;
                                    if (_freeBlobIds.Count > 0)
                                    {
                                        blobId = _freeBlobIds[0];
                                        _freeBlobIds.RemoveAt(0);
                                    }

                                    var tuioBlob = new Tuio11Blob(_currentTime, sessionId, blobId, posX, posY, angle, width, height, area, speedX, speedY, rotationSpeed, motionAcceleration,
                                        rotationAcceleration);
                                    _tuioBlobs[sessionId] = tuioBlob;
                                    foreach (var tuioListener in _tuioListeners)
                                    {
                                        tuioListener.AddTuioBlob(tuioBlob);
                                    }
                                }
                            }
                        }

                        foreach (var tuioListener in _tuioListeners)
                        {
                            tuioListener.Refresh(_currentTime);
                        }
                    }
                }

                _blobSetMessages.Clear();
                _blobAliveMessage = null;
            }
        }
    }
}