using System.Net;
using System.Numerics;
using CommandLine;
using Microsoft.Extensions.Logging;
using TuioNet.Common;
using TuioNet.Server;
using TuioNet.ServerDemo;

class Program
{
    public class Options
    {
        [Option('i', "ip", HelpText = "Set the ip address of the tuio sender.", Default = "127.0.0.1")]
        public string IpAddress { get; set; }
        [Option('p', "port", HelpText = "Set the port.", Default = 3333)]
        public int Port { get; set; }
        [Option('l', "logLevel", HelpText = "Set the minimum log level [Trace, Debug, Information, Warning, Error, Critical, None].", Default = LogLevel.Information)]
        public LogLevel LogLevel { get; set; }
        [Option('v', "tuioVersion", HelpText = "Set the tuio version [Tuio11, Tuio20].", Default = TuioVersion.Tuio11)]
        public TuioVersion TuioVersion { get; set; }
        [Option('c', "connection", HelpText = "Set the connection type [UDP, Websocket].", Default = TuioConnectionType.UDP)]
        public TuioConnectionType ConnectionType { get; set; }
        [Option('s', "sourceName", HelpText = "Set the source name for the Tuio Sender.", Default = "TuioServerDemo")]
        public string SourceName { get; set; }
        [Option('r', "resolution", HelpText = "Set the screen resolution as [x,y]. Only valid for Tuio 2.", Default = "1280,720")]
        public string Resolution { get; set; }
    }
    
    
    
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(option =>
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            var resolutionParts = option.Resolution.Split(',');
            var resolution = new Vector2(float.Parse(resolutionParts[0].Trim()),
                float.Parse(resolutionParts[1].Trim()));

            ITuioServer server = option.ConnectionType switch
            {
                TuioConnectionType.Websocket => new WebsocketServer(logger),
                TuioConnectionType.UDP => new UdpServer(),
                _ => throw new ArgumentOutOfRangeException()
            };

            ITuioManager tuioManager = option.TuioVersion switch
            {
                TuioVersion.Tuio11 => new Tuio11Manager(option.SourceName),
                TuioVersion.Tuio20 => new Tuio20Manager(option.SourceName, resolution),
                _ => throw new ArgumentOutOfRangeException()
            };

            var transmitter = new TuioTransmitter(server, tuioManager, logger);
            transmitter.Open(IPAddress.Parse(option.IpAddress), option.Port);
            transmitter.Send();
            transmitter.Close();
            
        });

    }
}