using System;
using System.Collections.Generic;
using System.Linq;
using OSC.NET;
using Stride.Core.Mathematics;
using TuioNet.Common;

namespace TuioNet.Tuio20
{
    public class Tuio20Processor
    {
        public Tuio20Processor(TuioClient client)
        {
            client.AddMessageListeners(new List<MessageListener>()
            {
                new MessageListener("/tuio2/frm", OnFrm),
                new MessageListener("/tuio2/alv", OnAlv),
                new MessageListener("/tuio2/tok", OnOther),
                new MessageListener("/tuio2/ptr", OnOther),
                new MessageListener("/tuio2/bnd", OnOther),
                new MessageListener("/tuio2/sym", OnOther),
            });
            
            TuioTime.Init();
            _currentTime = TuioTime.GetCurrentTime();
        }
        
        private readonly Dictionary<uint, Tuio20Object> _tuioObjects = new Dictionary<uint, Tuio20Object>();

        private OSCMessage _frmMessage;
        private readonly List<OSCMessage> _otherMessages = new List<OSCMessage>();
        
        private uint _bundleFrameId = 0;
        private uint _nextFrameId = 0;
        private uint _prevFrameId = 0;
        private TuioTime _currentTime;

        /// <summary>
        /// Event gets triggered when Tuio 2.0 Object is added.
        /// </summary>
        public event EventHandler<Tuio20Object> OnObjectAdded;
        
        /// <summary>
        /// Event gets triggered when Tuio 2.0 Object is updated.
        /// </summary>
        public event EventHandler<Tuio20Object> OnObjectUpdated;
        
        /// <summary>
        /// Event gets triggered when Tuio 2.0 Object is removed.
        /// </summary>
        public event EventHandler<Tuio20Object> OnObjectRemoved;
        
        /// <summary>
        /// This event gets triggered at the end of the current frame after all tuio messages were processed and it
        /// provides the current TuioTime. This event is useful to handle all updates contained in one TUIO frame together.
        /// </summary>
        public event EventHandler<TuioTime> OnRefreshed;

        /// <summary>
        /// The sensor dimension. The first two bytes represent the width. The last two byte represent the height.
        /// </summary>
        public uint SensorDimension { get; private set; } = 0;
        
        /// <summary>
        /// Provide additional information about the TUIO source.
        /// </summary>
        public string Source { get; private set; }
        public object TuioObjectLock { get; } = new object();
        
        /// <summary>
        /// Returns all active TUIO tokens.
        /// </summary>
        /// <returns>A list of all active TUIO tokens.</returns>
        public List<Tuio20Token> GetTuioTokenList()
        {
            lock (TuioObjectLock)
            {
                var tuioTokenList = new List<Tuio20Token>();
                foreach (var entry in _tuioObjects)
                {
                    if (entry.Value.ContainsTuioToken())
                    {
                        tuioTokenList.Add(entry.Value.Token);
                    }
                }

                return tuioTokenList;
            }
        }

        /// <summary>
        /// Returns all active TUIO pointers.
        /// </summary>
        /// <returns>A list of all active TUIO pointers.</returns>
        public List<Tuio20Pointer> GetTuioPointerList()
        {
            lock (TuioObjectLock)
            {
                var tuioPointerList = new List<Tuio20Pointer>();
                foreach (var entry in _tuioObjects)
                {
                    if (entry.Value.ContainsTuioPointer())
                    {
                        tuioPointerList.Add(entry.Value.Pointer);
                    }
                }

                return tuioPointerList;
            }
        }

        /// <summary>
        /// Returns all active TUIO bounds.
        /// </summary>
        /// <returns>A list of all active TUIO bounds.</returns>
        public List<Tuio20Bounds> GetTuioBoundsList()
        {
            lock (TuioObjectLock)
            {
                var tuioBoundsList = new List<Tuio20Bounds>();
                foreach (var entry in _tuioObjects)
                {
                    if (entry.Value.ContainsTuioBounds())
                    {
                        tuioBoundsList.Add(entry.Value.Bounds);
                    }
                }

                return tuioBoundsList;
            }
        }

        /// <summary>
        /// Returns all active TUIO marker symbols.
        /// </summary>
        /// <returns>A list of all active TUIO marker symbols.</returns>
        public List<Tuio20Symbol> GetTuioSymbolList()
        {
            lock (TuioObjectLock)
            {
                var tuioSymbolList = new List<Tuio20Symbol>();
                foreach (var entry in _tuioObjects)
                {
                    if (entry.Value.ContainsTuioSymbol())
                    {
                        tuioSymbolList.Add(entry.Value.Symbol);
                    }
                }

                return tuioSymbolList;
            }
        }
        
        private void OnFrm(OSCMessage oscMessage)
        {
            _bundleFrameId = (uint)(int)oscMessage.Values[0];

            if (_bundleFrameId <= _nextFrameId) return;
            _otherMessages.Clear();
            _nextFrameId = _bundleFrameId;
            _frmMessage = oscMessage;
        }
        
        private void OnOther(OSCMessage oscMessage)
        {
            if (_bundleFrameId != _nextFrameId)
            {
                return;
            }

            _otherMessages.Add(oscMessage);
        }

        private void OnAlv(OSCMessage oscMessage)
        {
            if (_frmMessage == null || _bundleFrameId != _nextFrameId)
            {
                return;
            }

            var frameId = (uint)(int)_frmMessage.Values[0];
            var frameTime = (OscTimeTag)_frmMessage.Values[1];
            var sensorDimension = (uint)(int)_frmMessage.Values[2];
            var source = (string)_frmMessage.Values[3];
            TuioTime currentFrameTime = TuioTime.FromOscTime(frameTime);
            if (frameId >= _prevFrameId || frameId == 0 || 
                (currentFrameTime - _currentTime).GetTotalMilliseconds() >= 1000)
            {
                SensorDimension = sensorDimension;
                Source = source;
                HashSet<uint> currentSIds = new HashSet<uint>(_tuioObjects.Keys);
                HashSet<uint> aliveSIds = new HashSet<uint>();
                foreach (var sId in oscMessage.Values)
                {
                    aliveSIds.Add((uint)(int) sId);
                }
                HashSet<uint> newSIds = new HashSet<uint>(aliveSIds.Except(currentSIds));
                HashSet<uint> removedSIds = new HashSet<uint>(currentSIds.Except(aliveSIds));
                HashSet<Tuio20Object> addedTuioObjects = new HashSet<Tuio20Object>();
                HashSet<Tuio20Object> updatedTuioObjects = new HashSet<Tuio20Object>();
                HashSet<Tuio20Object> removedTuioObjects = new HashSet<Tuio20Object>();
                lock (TuioObjectLock)
                {
                    foreach (var sId in newSIds)
                    {
                        var tuioObject = new Tuio20Object(_currentTime, sId);
                        _tuioObjects[sId] = tuioObject;
                    }
                    foreach (var sId in removedSIds)
                    {
                        var tuioObject = _tuioObjects[sId];
                        removedTuioObjects.Add(tuioObject);
                        tuioObject.Remove(currentFrameTime);
                        _tuioObjects.Remove(sId);
                    }

                    foreach (var otherOscMessage in _otherMessages)
                    {
                        if (otherOscMessage.Address == "/tuio2/tok")
                        {
                            var sId = (uint)(int)otherOscMessage.Values[0];
                            if (!aliveSIds.Contains(sId)) continue;
                            var typeUserId = (uint)(int)otherOscMessage.Values[1];
                            var componentId = (uint)(int)otherOscMessage.Values[2];
                            var posX = (float)otherOscMessage.Values[3];
                            var posY = (float)otherOscMessage.Values[4];
                            var position = new Vector2(posX, posY);
                            var angle = (float)otherOscMessage.Values[5];
                            float angleVelocity = 0, acceleration = 0, rotationAcceleration = 0;
                            var velocity = Vector2.Zero;
                            if (otherOscMessage.Values.Count > 6)
                            {
                                velocity.X = (float)otherOscMessage.Values[6];
                                velocity.Y = (float)otherOscMessage.Values[7];
                                    
                                angleVelocity = (float)otherOscMessage.Values[8];
                                acceleration = (float)otherOscMessage.Values[9];
                                rotationAcceleration = (float)otherOscMessage.Values[10];
                            }

                            var tuioObject = _tuioObjects[sId];
                            if (tuioObject.Token == null)
                            {
                                addedTuioObjects.Add(tuioObject);
                                tuioObject.SetTuioToken(new Tuio20Token(_currentTime, tuioObject, typeUserId, componentId, position, angle, velocity, angleVelocity, acceleration, rotationAcceleration));
                            }
                            else
                            {
                                if (!tuioObject.Token.HasChanged(typeUserId, componentId, position, angle, velocity,
                                        angleVelocity, acceleration, rotationAcceleration)) continue;
                                updatedTuioObjects.Add(tuioObject);
                                tuioObject.Token.Update(_currentTime, typeUserId, componentId, position, angle, velocity, angleVelocity, acceleration, rotationAcceleration);
                            }
                        }
                        else if (otherOscMessage.Address == "/tuio2/ptr")
                        {
                            var sId = (uint)(int)otherOscMessage.Values[0];
                            if (!aliveSIds.Contains(sId)) continue;
                            var typeId = (uint)(int)otherOscMessage.Values[1];
                            var componentId = (uint)(int)otherOscMessage.Values[2];
                            var posX = (float)otherOscMessage.Values[3];
                            var posY = (float)otherOscMessage.Values[4];
                            var position = new Vector2(posX, posY);
                            var angle = (float)otherOscMessage.Values[5];
                            var shear = (float)otherOscMessage.Values[6];
                            var radius = (float)otherOscMessage.Values[7];
                            var press = (float)otherOscMessage.Values[8];
                            float  pressureVelocity = 0, acceleration = 0, pressureAcceleration = 0;
                            var velocity = Vector2.Zero;
                            if (otherOscMessage.Values.Count > 9)
                            {
                                velocity.X = (float)otherOscMessage.Values[6];
                                velocity.Y = (float)otherOscMessage.Values[7];
                                pressureVelocity = (float)otherOscMessage.Values[8];
                                acceleration = (float)otherOscMessage.Values[9];
                                pressureAcceleration = (float)otherOscMessage.Values[10];
                            }

                            var tuioObject = _tuioObjects[sId];
                            if (tuioObject.Pointer == null)
                            {
                                addedTuioObjects.Add(tuioObject);
                                tuioObject.SetTuioPointer(new Tuio20Pointer(_currentTime, tuioObject, typeId, componentId, position, angle, shear, radius, press, velocity, pressureVelocity, acceleration, pressureAcceleration));
                            }
                            else
                            {
                                if (!tuioObject.Pointer.HasChanged(typeId, componentId, position, angle, shear, radius,
                                        press, velocity, pressureVelocity, acceleration, pressureAcceleration))
                                    continue;
                                updatedTuioObjects.Add(tuioObject);
                                tuioObject.Pointer.Update(_currentTime, typeId, componentId, position, angle, shear, radius, press, velocity, pressureVelocity, acceleration, pressureAcceleration);
                            }
                        }
                        else if (otherOscMessage.Address == "/tuio2/bnd")
                        {
                            var sId = (uint)(int)otherOscMessage.Values[0];
                            if (!aliveSIds.Contains(sId)) continue;
                            var posX = (float)otherOscMessage.Values[1];
                            var posY = (float)otherOscMessage.Values[2];
                            var position = new Vector2(posX, posY);
                            var angle = (float)otherOscMessage.Values[3];
                            var width = (float)otherOscMessage.Values[4];
                            var height = (float)otherOscMessage.Values[5];
                            var size = new Vector2(width, height);
                            var area = (float)otherOscMessage.Values[6];
                            float angleVelocity = 0, acceleration = 0, rotationAcceleration = 0;
                            var velocity = Vector2.Zero;
                            if (otherOscMessage.Values.Count > 7)
                            {
                                velocity.X = (float)otherOscMessage.Values[7];
                                velocity.Y = (float)otherOscMessage.Values[8];
                                angleVelocity = (float)otherOscMessage.Values[9];
                                acceleration = (float)otherOscMessage.Values[10];
                                rotationAcceleration = (float)otherOscMessage.Values[11];
                            }

                            var tuioObject = _tuioObjects[sId];
                            if (tuioObject.Bounds == null)
                            {
                                addedTuioObjects.Add(tuioObject);
                                tuioObject.SetTuioBounds(new Tuio20Bounds(_currentTime, tuioObject, position, angle, size, area, velocity, angleVelocity, acceleration, rotationAcceleration));
                            }
                            else
                            {
                                if (!tuioObject.Bounds.HasChanged(position, angle, size, area, velocity, angleVelocity, acceleration, rotationAcceleration)) continue;
                                updatedTuioObjects.Add(tuioObject);
                                tuioObject.Bounds.Update(_currentTime, position, angle, size, area, velocity, angleVelocity, acceleration, rotationAcceleration);
                            }
                        }
                        else if (otherOscMessage.Address == "/tuio2/sym")
                        {
                            var sessionId = (uint)(int)otherOscMessage.Values[0];
                            if (!aliveSIds.Contains(sessionId)) continue;
                            var typeUserId = (uint)(int)otherOscMessage.Values[1];
                            var componentId = (uint)(int)otherOscMessage.Values[2];
                            string group = (string)otherOscMessage.Values[3];
                            string data = (string)otherOscMessage.Values[4];

                            var tuioObject = _tuioObjects[sessionId];
                            if (tuioObject.Symbol == null)
                            {
                                addedTuioObjects.Add(tuioObject);
                                tuioObject.SetTuioSymbol(new Tuio20Symbol(_currentTime, tuioObject, typeUserId, componentId, 
                                    group, data));
                            }
                            else
                            {
                                if (!tuioObject.Symbol.HasChanged(typeUserId, componentId, group, data)) continue;
                                updatedTuioObjects.Add(tuioObject);
                                tuioObject.Symbol.Update(_currentTime, typeUserId, componentId, group,
                                    data);
                            }
                        }
                    }
                }

                foreach (var tuioObject in addedTuioObjects)
                {
                    OnObjectAdded?.Invoke(this, tuioObject);
                }

                foreach (var tuioObject in updatedTuioObjects)
                {
                    OnObjectUpdated?.Invoke(this, tuioObject);
                }

                foreach (var tuioObject in removedTuioObjects)
                {
                    OnObjectRemoved?.Invoke(this, tuioObject);
                }

                OnRefreshed?.Invoke(this, _currentTime);
            }
            _currentTime = currentFrameTime;
            _prevFrameId = frameId;
            _frmMessage = null;
        }
    }
}