﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TuioNet.Server;

public class UdpServer : ITuioServer
{
    private UdpClient _server;
    public void Start(IPAddress address, int port)
    {
        var endpoint = new IPEndPoint(address, port);
        _server = new UdpClient(endpoint);
    }

    public void Stop()
    {
        _server.Close();
        _server.Dispose();
    }

    public void Send(string data)
    {
        var dataBytes = Encoding.ASCII.GetBytes(data);
        Send(dataBytes);
    }

    public void Send(byte[] data)
    {
        _server.Send(data, data.Length);
    }
}