using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TuioNet.Server
{
    
    public class UdpServer : ITuioServer
    {
        private UdpClient _server;
        private IPEndPoint _endpoint;
        
        public void Start(IPAddress address, int port)
        {
            if (_server != null) Stop();
            _server = new UdpClient();
            _endpoint = new IPEndPoint(address, port);
        }

        public void Stop()
        {
            _server.Close();
            _server = null;
        }

        public void Send(string data)
        {
            var dataBytes = Encoding.ASCII.GetBytes(data);
            Send(dataBytes);
        }

        public void Send(byte[] data)
        {
            _server.Send(data, data.Length, _endpoint);
        }
    }
}
