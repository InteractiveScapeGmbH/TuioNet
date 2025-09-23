
using System.Net;

namespace TuioNet.Server
{
    public interface ITuioServer
    {
        public void Start(IPAddress address, int port);
        public void Stop();
        public void Send(string data);
        public void Send(byte[] data);
    }
}

