﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OSC.NET;
using TuioNet.Common;

namespace TuioNet.Tuio11
{
    public class Tuio11Client
    {
        private readonly TuioReceiver _tuioReceiver;

        private readonly List<ITuio11Listener> _tuio11Listeners = new List<ITuio11Listener>();
        private readonly List<ITuio11CursorListener> _cursorListeners = new List<ITuio11CursorListener>();
        private readonly List<ITuio11ObjectListener> _objectListeners = new List<ITuio11ObjectListener>();
        private readonly List<ITuio11BlobListener> _blobListeners = new List<ITuio11BlobListener>();
        private readonly List<ITimeRefresher> _timeRefreshers = new List<ITimeRefresher>();

        private readonly Dictionary<uint, Tuio11Object> _tuioObjects = new Dictionary<uint, Tuio11Object>();
        private readonly Dictionary<uint, Tuio11Cursor> _tuioCursors = new Dictionary<uint, Tuio11Cursor>();
        private readonly Dictionary<uint, Tuio11Blob> _tuioBlobs = new Dictionary<uint, Tuio11Blob>();

        private readonly List<OSCMessage> _objectSetMessages = new List<OSCMessage>();
        private readonly List<OSCMessage> _cursorSetMessages = new List<OSCMessage>();
        private readonly List<OSCMessage> _blobSetMessages = new List<OSCMessage>();

        private OSCMessage _objectAliveMessage;
        private OSCMessage _cursorAliveMessage;
        private OSCMessage _blobAliveMessage;

        private readonly List<uint> _freeCursorIds = new List<uint>();
        private readonly List<uint> _freeBlobIds = new List<uint>();

        private uint _currentFrame = 0;
        private TuioTime _currentTime;
        
        /// <summary>
        /// Returns true if the receiver is connected to the TUIO sender.
        /// </summary>
        public bool IsConnected => _tuioReceiver.IsConnected;

        /// <summary>
        /// Create new client for TUIO 1.1.
        /// </summary>
        /// <param name="connectionType">Type of the protocol which gets used to connect to the sender.</param>
        /// <param name="address">The IP address of the TUIO sender.</param>
        /// <param name="port">The port the client listen to for new TUIO messages. Default UDP port is 3333.</param>
        /// <param name="isAutoProcess">If set, the receiver processes incoming messages automatically. Otherwise the ProcessMessages() methods needs to be called manually.</param>
        public Tuio11Client(TuioConnectionType connectionType, string address = "0.0.0.0", int port = 3333, bool isAutoProcess = true)
        {
            _tuioReceiver = TuioReceiver.FromConnectionType(connectionType, address, port, isAutoProcess);
            _tuioReceiver.AddMessageListener("/tuio/2Dobj", On2Dobj);
            _tuioReceiver.AddMessageListener("/tuio/2Dcur", On2Dcur);
            _tuioReceiver.AddMessageListener("/tuio/2Dblb", On2Dblb);
        }
        
        /// <summary>
        /// Establish a connection to the TUIO sender.
        /// </summary>
        public void Connect()
        {
            TuioTime.Init();
            _currentTime = TuioTime.GetCurrentTime();
            _tuioReceiver.Connect();
        }
        
        /// <summary>
        /// Closes the connection to the TUIO sender.
        /// </summary>
        public void Disconnect()
        {
            _tuioReceiver.Disconnect();
        }
        
        /// <summary>
        /// Process the TUIO messages in the message queue and invoke callbacks of the associated message listener. Only needs to be called if isAutoProcess is set to false.
        /// </summary>
        public void ProcessMessages()
        {
            _tuioReceiver.ProcessMessages();
        }

        /// <summary>
        /// Add a general listener to listen for all kinds of TUIO events.
        /// </summary>
        /// <param name="tuio11Listener"></param>
        public void AddTuioListener(ITuio11Listener tuio11Listener)
        {
            _tuio11Listeners.Add(tuio11Listener);
        }

        /// <summary>
        /// Remove a general listener from the list of Tuio11Listeners.
        /// </summary>
        /// <param name="tuio11Listener"></param>
        public void RemoveTuioListener(ITuio11Listener tuio11Listener)
        {
            _tuio11Listeners.Remove(tuio11Listener);
        }
        
        /// <summary>
        /// Add a listener to listen for TUIO cursor events.
        /// </summary>
        /// <param name="cursorListener"></param>
        public void AddCursorListener(ITuio11CursorListener cursorListener)
        {
            _cursorListeners.Add(cursorListener);
        }
        
        /// <summary>
        /// Remove a listener from the list of CursorListeners.
        /// </summary>
        /// <param name="cursorListener"></param>
        public void RemoveCursorListener(ITuio11CursorListener cursorListener)
        {
            _cursorListeners.Remove(cursorListener);
        }

        /// <summary>
        /// Add a listener to listen for TUIO object events.
        /// </summary>
        /// <param name="objectListener"></param>
        public void AddObjectListener(ITuio11ObjectListener objectListener)
        {
            _objectListeners.Add(objectListener);
        }

        /// <summary>
        ///  Remove a listener from the list of ObjectListeners.
        /// </summary>
        /// <param name="objectListener"></param>
        public void RemoveObjectListener(ITuio11ObjectListener objectListener)
        {
            _objectListeners.Remove(objectListener);
        }

        /// <summary>
        /// Add a listener to listen for TUIO blob events.
        /// </summary>
        /// <param name="blobListener"></param>
        public void AddBlobListener(ITuio11BlobListener blobListener)
        {
            _blobListeners.Add(blobListener);
        }

        /// <summary>
        /// Remove a listener from the list of BlobListeners.
        /// </summary>
        /// <param name="blobListener"></param>
        public void RemoveBlobListener(ITuio11BlobListener blobListener)
        {
            _blobListeners.Remove(blobListener);
        }

        /// <summary>
        /// Add a listener to react on TuioTime updates.
        /// </summary>
        /// <param name="timeRefresher"></param>
        public void AddTimeRefresher(ITimeRefresher timeRefresher)
        {
            _timeRefreshers.Add(timeRefresher);
        }

        /// <summary>
        /// Remove a listener from the list of TimeRefresher.
        /// </summary>
        /// <param name="timeRefresher"></param>
        public void RemoveTimeRefresher(ITimeRefresher timeRefresher)
        {
            if (!_timeRefreshers.Contains(timeRefresher)) return;
            _timeRefreshers.Remove(timeRefresher);
        }

        /// <summary>
        /// Remove all listeners from the client. This includes cursors, objects, blobs and timeRefreshers.
        /// </summary>
        public void RemoveAllTuioListeners()
        {
            _tuio11Listeners.Clear();
            _cursorListeners.Clear();
            _objectListeners.Clear();
            _blobListeners.Clear();
            _timeRefreshers.Clear();
        }
        
        /// <summary>
        /// Returns all active TUIO objects.
        /// </summary>
        /// <returns>A list of all active TUIO objects.</returns>
        public List<Tuio11Object> GetTuioObjects()
        {
            return _tuioObjects.Values.ToList();
        }
        
        /// <summary>
        /// Returns all active TUIO cursors.
        /// </summary>
        /// <returns>A List of all active TUIO cursors.</returns>
        public List<Tuio11Cursor> GetTuioCursors()
        {
            return _tuioCursors.Values.ToList();
        }

        /// <summary>
        /// Returns all active TUIO blobs.
        /// </summary>
        /// <returns>A list of all active TUIO blobs.</returns>
        public List<Tuio11Blob> GetTuioBlobs()
        {
            return _tuioBlobs.Values.ToList();
        }
        
        /// <summary>
        /// Return a specific TUIO object by its session id.
        /// </summary>
        /// <param name="sessionId">The session id of the requested TUIO object.</param>
        /// <returns>A single TUIO object with the given session id.</returns>
        public Tuio11Object GetTuioObject(uint sessionId)
        {
            return _tuioObjects[sessionId];
        }

        /// <summary>
        /// Return a specific TUIO cursor by its session id.
        /// </summary>
        /// <param name="sessionId">The session id of the requested TUIO cursor.</param>
        /// <returns>A single TUIO cursor with the given session id.</returns>
        public Tuio11Cursor GetTuioCursor(uint sessionId)
        {
            return _tuioCursors[sessionId];
        }

        /// <summary>
        /// Return a specific TUIO blob by its session id.
        /// </summary>
        /// <param name="sessionId">The session id of the requested TUIO blob.</param>
        /// <returns>A single TUIO blob with the given session id.</returns>
        public Tuio11Blob GetTuioBlob(uint sessionId)
        {
            return _tuioBlobs[sessionId];
        }

        private const int FRAME_TOLERANCE = 100;
        private const int TIME_TOLERANCE = 100;
        private bool UpdateFrame(uint frameId)
        {
            var currentTime = TuioTime.GetCurrentTime();
            
            // redundant bundles can be marked using the frame id -1.
            var isFrameValid = frameId > 0;
            if (isFrameValid)
            {
                if (frameId > _currentFrame)
                {
                    _currentTime = currentTime;
                }
                
                // if the connection to the tuio sender gets lost after more then FRAME_TOLERANCE frames or the frameId
                // integer flows over the second condition holds true and the current frame is set to the incoming one.
                if (frameId >= _currentFrame || _currentFrame - frameId > FRAME_TOLERANCE)
                {
                    _currentFrame = frameId;
                }
                else
                {
                    return false;
                }
            }
            else if ((currentTime - _currentTime).GetTotalMilliseconds() > TIME_TOLERANCE)
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
                var frameId = (uint)(int)oscMessage.Values[1];
                if (UpdateFrame(frameId))
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
                            foreach (var objectListener in _objectListeners)
                            {
                                objectListener.RemoveObject(tuioObject);
                            }

                            foreach (var tuioListener in _tuio11Listeners)
                            {
                                tuioListener.RemoveTuioObject(tuioObject);
                            }

                            _tuioObjects.Remove(sId);
                        }

                        foreach (var setMessage in _objectSetMessages)
                        {
                            var sessionId = (uint)(int)setMessage.Values[1];
                            var symbolId = (uint)(int)setMessage.Values[2];
                            var posX = (float)setMessage.Values[3];
                            var posY = (float)setMessage.Values[4];
                            var position = new Vector2(posX, posY);
                            var angle = (float)setMessage.Values[5];
                            var speedX = (float)setMessage.Values[6];
                            var speedY = (float)setMessage.Values[7];
                            var velocity = new Vector2(speedX, speedY);
                            var rotationSpeed = (float)setMessage.Values[8];
                            var acceleration = (float)setMessage.Values[9];
                            var rotationAcceleration = (float)setMessage.Values[10];
                            if (aliveSIds.Contains(sessionId))
                            {
                                if (currentSIds.Contains(sessionId))
                                {
                                    var tuioObject = _tuioObjects[sessionId];
                                    tuioObject.UpdateTime(_currentTime);
                                    if (tuioObject.HasChanged(position, angle, velocity, rotationSpeed, acceleration, rotationAcceleration))
                                    {
                                        tuioObject.Update(_currentTime, position, angle, velocity, rotationSpeed, acceleration, rotationAcceleration);
                                        foreach (var objectListener in _objectListeners)
                                        {
                                            objectListener.UpdateObject(tuioObject);
                                        }

                                        foreach (var tuioListener in _tuio11Listeners)
                                        {
                                            tuioListener.UpdateTuioObject(tuioObject);
                                        }
                                    }
                                }
                                else
                                {
                                    var tuioObject = new Tuio11Object(_currentTime, sessionId, symbolId, position, angle, velocity, rotationSpeed, acceleration, rotationAcceleration);
                                    _tuioObjects[sessionId] = tuioObject;
                                    foreach (var objectListener in _objectListeners)
                                    {
                                        objectListener.AddObject(tuioObject);
                                    }

                                    foreach (var tuioListener in _tuio11Listeners)
                                    {
                                        tuioListener.AddTuioObject(tuioObject);
                                    }
                                }
                            }
                        }

                        foreach (var timeRefresher in _timeRefreshers)
                        {
                            timeRefresher.Refresh(_currentTime);
                        }

                        foreach (var tuioListener in _tuio11Listeners)
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
                var frameId = (uint)(int)oscMessage.Values[1];
                if (UpdateFrame(frameId))
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
                            foreach (var cursorListener in _cursorListeners)
                            {
                                cursorListener.RemoveCursor(tuioCursor);
                            }

                            foreach (var tuioListener in _tuio11Listeners)
                            {
                                tuioListener.RemoveTuioCursor(tuioCursor);
                            }

                            _tuioCursors.Remove(sId);
                            _freeCursorIds.Add(tuioCursor.CursorId);
                        }
                        _freeCursorIds.Sort();
                        foreach (var setMessage in _cursorSetMessages)
                        {
                            var sessionId = (uint)(int)setMessage.Values[1];
                            var posX = (float)setMessage.Values[2];
                            var posY = (float)setMessage.Values[3];
                            var position = new Vector2(posX, posY);
                            var speedX = (float)setMessage.Values[4];
                            var speedY = (float)setMessage.Values[5];
                            var velocity = new Vector2(speedX, speedY);
                            var acceleration = (float)setMessage.Values[6];
                            if (aliveSIds.Contains(sessionId))
                            {
                                if (currentSIds.Contains(sessionId))
                                {
                                    var tuioCursor = _tuioCursors[sessionId];
                                    if (tuioCursor.HasChanged(position, velocity, acceleration))
                                    {
                                        tuioCursor.Update(_currentTime, position, velocity, acceleration);
                                        foreach (var cursorListener in _cursorListeners)
                                        {
                                            cursorListener.UpdateCursor(tuioCursor);
                                        }

                                        foreach (var tuioListener in _tuio11Listeners)
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

                                    var tuioCursor = new Tuio11Cursor(_currentTime, sessionId, cursorId, position, velocity, acceleration);
                                    _tuioCursors[sessionId] = tuioCursor;
                                    foreach (var cursorListener in _cursorListeners)
                                    {
                                        cursorListener.AddCursor(tuioCursor);
                                    }

                                    foreach (var tuioListener in _tuio11Listeners)
                                    {
                                        tuioListener.AddTuioCursor(tuioCursor);
                                    }
                                }
                            }
                        }

                        foreach (var timeRefresher in _timeRefreshers)
                        {
                            timeRefresher.Refresh(_currentTime);
                        }

                        foreach (var tuioListener in _tuio11Listeners)
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
                var frameId = (uint)(int)oscMessage.Values[1];
                if (UpdateFrame(frameId))
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
                            foreach (var blobListener in _blobListeners)
                            {
                                blobListener.RemoveBlob(tuioBlob);
                            }

                            foreach (var tuioListener in _tuio11Listeners)
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
                            var position = new Vector2(posX, posY);
                            var angle = (float)setMessage.Values[4];
                            var width = (float)setMessage.Values[5];
                            var height = (float)setMessage.Values[6];
                            var size = new Vector2(width, height);
                            var area = (float)setMessage.Values[7];
                            var speedX = (float)setMessage.Values[8];
                            var speedY = (float)setMessage.Values[9];
                            var velocity = new Vector2(speedX, speedY);
                            var rotationSpeed = (float)setMessage.Values[10];
                            var acceleration = (float)setMessage.Values[11];
                            var rotationAcceleration = (float)setMessage.Values[12];
                            if (aliveSIds.Contains(sessionId))
                            {
                                if (currentSIds.Contains(sessionId))
                                {
                                    var tuioBlob = _tuioBlobs[sessionId];
                                    if (tuioBlob.HasChanged(position, angle, size, area, velocity, rotationSpeed, acceleration, rotationAcceleration))
                                    {
                                        tuioBlob.Update(_currentTime, position, angle, size, area, velocity, rotationSpeed, acceleration, rotationAcceleration);
                                        foreach (var blobListener in _blobListeners)
                                        {
                                            blobListener.UpdateBlob(tuioBlob);
                                        }

                                        foreach (var tuioListener in _tuio11Listeners)
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

                                    var tuioBlob = new Tuio11Blob(_currentTime, sessionId, blobId, position, angle, size, area, velocity, rotationSpeed, acceleration,
                                        rotationAcceleration);
                                    _tuioBlobs[sessionId] = tuioBlob;
                                    foreach (var blobListener in _blobListeners)
                                    {
                                        blobListener.AddBlob(tuioBlob);
                                    }

                                    foreach (var tuioListener in _tuio11Listeners)
                                    {
                                        tuioListener.AddTuioBlob(tuioBlob);
                                    }
                                }
                            }
                        }

                        foreach (var timeRefresher in _timeRefreshers)
                        {
                            timeRefresher.Refresh(_currentTime);
                        }

                        foreach (var tuioListener in _tuio11Listeners)
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