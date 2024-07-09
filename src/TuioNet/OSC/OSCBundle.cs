#region licence/info
// OSC.NET - Open Sound Control for .NET
// http://luvtechno.net/
//
// Copyright (c) 2006, Yoshinori Kawasaki 
// All rights reserved.
//
// Changes and improvements:
// Copyright (c) 2006-2014 Martin Kaltenbrunner <martin@tuio.org>
// As included with http://reactivision.sourceforge.net/
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// * Neither the name of "luvtechno.net" nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY 
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion licence/info
	
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// VVVV OSC Utilities 
/// </summary>
namespace OSC.NET
{
	/// <summary>
	/// OSCBundle
	/// </summary>
	public class OSCBundle : OSCPacket
	{
		protected const string BUNDLE = "#bundle";
		private OscTimeTag timestamp = new OscTimeTag();

        public OSCBundle(OscTimeTag ts)
        {
			this.address = BUNDLE;
			this.timestamp = ts;
		}

		public OSCBundle(long ts)
		{
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            timestamp = new OscTimeTag(start.AddMilliseconds(ts).ToLocalTime());		    
		}


		public OSCBundle()
		{
			this.address = BUNDLE;
			this.timestamp = new OscTimeTag(DateTime.Now);
		}

		override protected void pack()
		{
			var data = new List<byte>();

			addBytes(data, packString(this.Address));
			padNull(data);
			addBytes(data, packTimeTag(timestamp));  // fixed point, 8 bytes
			
			foreach(OSCPacket oscPacket in this.Values)
			{
				if (oscPacket != null)
				{
					byte[] bs = oscPacket.BinaryData;
					addBytes(data, packInt(bs.Length));
					addBytes(data, bs);
				}
				else 
				{
					// TODO
				}
			}
			
			this.binaryData = data.ToArray();
		}

		public static new OSCBundle Unpack(byte[] bytes, ref int start, int end)
		{

			string address = unpackString(bytes, ref start);
			//Console.WriteLine("bundle: " + address);
			if(!address.Equals(BUNDLE)) return null; // TODO

			OscTimeTag timestamp = unpackTimeTag(bytes, ref start);
            OSCBundle bundle = new OSCBundle(timestamp);
			
			while(start < end)
			{
				int length = unpackInt(bytes, ref start);
				int sub_end = start + length;
				bundle.Append(OSCPacket.Unpack(bytes, ref start, sub_end));
			}

			return bundle;
		}

		public OscTimeTag getTimeStamp() {
			return timestamp;
		}

		override public void Append(object value)
		{
			if( value is OSCPacket) 
			{
				values.Add(value);
			}
			else 
			{
				// TODO: exception
			}
		}

		override public bool IsBundle() { return true; }
	}
}

