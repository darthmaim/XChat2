using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using XChat2.Common.Configuration;
using XChat2.Server.Clients;
using XChat2.Server.ConsoleHelpers;
using System.IO;
using XChat2.Server.Database.Clients;
using XChat2.Common.Networking.P2P.Server;

namespace XChat2.Server
{
    public class Server
    {
        private static TcpListener _listener;
        private static Thread _listenerThread;

        private static bool _run = true;

        private static Config _config;

        private static List<Connection> _activeConnections;

        private static HolePunchServer _holePunchServer;

        public static void Main(string[] args)
        {
            ConsoleHelpers.ConsoleHelper.Write("=== $4X$0Chat2 Server v1.0.0.0 ===", ConsoleHelpers.Align.Center);

            Init(args);

            _listenerThread.Start();

            string cmd;
            do
            {
                cmd = Console.ReadLine();
            } while (ProcessCmd(cmd));

            _run = false;
            _holePunchServer.Stop();
            _listenerThread.Join(1000);
        }

        private static bool ProcessCmd(string cmd)
        {
            if(string.IsNullOrEmpty(cmd.Trim()))
            {
                //Console.CursorTop--;
            }
            else
            {
                switch(cmd.Split(' ')[0])
                {
                    case "exit":
                        Client.SaveAll();
                        foreach(Connection c in _activeConnections)
                            c.StartDisconnect();
                        return false;
                    case "connections":
                        int id = 0;
                        ConsoleHelper.WriteLine("Active Connections: " + _activeConnections.Count);
                        foreach(Connection c in _activeConnections)
                        {
                            ConsoleHelper.WriteLine(string.Format(" {0,3} | {1}", id++, c.ToString()));
                        }
                        break;
                    case "clients":
                        ConsoleHelper.WriteLine("Clients: " + Client.Clients.Count());
                        foreach (Client c in Client.Clients)
                        {
                            ConsoleHelper.WriteLine(string.Format(" {0,3} | {1} {2}", c.ID, c.Username, c.Online ? "(Online)" : "(Offline)"));
                        }
                        break;
                    case "save":
                        ConsoleHelper.Write("Save all clients", Align.Left);
                        Client.SaveAll();
                        ConsoleHelpers.ConsoleHelper.Write("[ $2OK$0 ]", ConsoleHelpers.Align.Right);
                    break;
                }
            }
            return true;
        }

        private static void Init(string[] args)
        {
            // ==================[ Config ] ======================
            ConsoleHelpers.ConsoleHelper.Write("Load Config", ConsoleHelpers.Align.Left);

            try
            {
                string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "XChat2Server");
                if (!Directory.Exists(configPath))
                    Directory.CreateDirectory(configPath);
                _config = new Config(Path.Combine(configPath, "config.cfg"));
                ConsoleHelpers.ConsoleHelper.Write("[ $2OK$0 ]", ConsoleHelpers.Align.Right);
            }
            catch (Exception x)
            {
                ConsoleHelpers.ConsoleHelper.Write("[ $1ERROR$0 ]", ConsoleHelpers.Align.Right);
                Console.WriteLine(x.GetType().FullName);
                Console.WriteLine(x.ToString());
            }
            DefaultConfig();
            // ===================================================

            // ==================[ Clients ] =====================
            bool loadClientsFromDB = !args.Contains("-debug");
            //loadClientsFromDB = true;
            if(loadClientsFromDB)
                ConsoleHelpers.ConsoleHelper.Write("Load Clients from db", ConsoleHelpers.Align.Left);
            else
                ConsoleHelpers.ConsoleHelper.Write("Load test Clients", ConsoleHelpers.Align.Left);
            try
            {
                if (loadClientsFromDB)
                {
                    Client.Init(_config,
                        new MySQLClientDataSource(_config["mysql"].get<string>("server").Value,
                            _config["mysql"].get<string>("database").Value,
                            _config["mysql"].get<string>("uid").Value,
                            _config["mysql"].get<string>("password").Value),
                        clientOnlineStatusChanged);
                }
                else
                {
                    Client.Init(_config, new FakeClientDataSource(), clientOnlineStatusChanged);
                }
                ConsoleHelpers.ConsoleHelper.Write("[ $2OK$0 ]", ConsoleHelpers.Align.Right);
            }
            catch (Exception x)
            {
                ConsoleHelpers.ConsoleHelper.Write("[ $1ERROR$0 ]", ConsoleHelpers.Align.Right);
                Console.WriteLine(x.GetType().FullName);
                Console.WriteLine(x.ToString());
            }

            // ===================================================

            _activeConnections = new List<Connection>();

            _listener = new TcpListener(IPAddress.Parse(_config["general"].get<string>("bind").Value), _config["general"].get<int>("port").Value);
            _listenerThread = new Thread(new ThreadStart(Run));
            _listenerThread.IsBackground = true;

            _holePunchServer = new HolePunchServer(IPAddress.Parse(_config["general"].get<string>("bind").Value), _config["general"].get<int>("port").Value);
        }

        private static void DefaultConfig()
        {
            if (!_config.ContainsSection("general"))
                _config.AddSection("general");
            if (!_config["general"].ContainsEntry("bind"))
                _config["general"].get<string>("bind").Value = "0.0.0.0";
            if (!_config["general"].ContainsEntry("port"))
                _config["general"].get<int>("port").Value = 2443;

            if (!_config.ContainsSection("mysql"))
                _config.AddSection("mysql");
            if (!_config["mysql"].ContainsEntry("server"))
                _config["mysql"].get<string>("server").Value = "127.0.0.1";
            if (!_config["mysql"].ContainsEntry("database"))
                _config["mysql"].get<string>("database").Value = "xchat2";
            if (!_config["mysql"].ContainsEntry("uid"))
                _config["mysql"].get<string>("uid").Value = "xchat2";
            if (!_config["mysql"].ContainsEntry("password"))
                _config["mysql"].get<string>("password").Value = "";

            if (!_config.ContainsSection("clients"))
                _config.AddSection("clients");
            if (!_config["clients"].ContainsEntry("UserOptionPath"))
                _config["clients"].get<string>("UserOptionPath").Value = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine("XChat2Server","UserOptions"));

            if (!Directory.Exists(_config["clients"].get<string>("UserOptionPath").Value))
                Directory.CreateDirectory(_config["clients"].get<string>("UserOptionPath").Value);

            _config.Save();
        }

        private static void Run()
        {
            _listener.Start();

            ConsoleHelpers.ConsoleHelper.WriteLine("Listener started (" + _listener.LocalEndpoint.ToString() + ")");

            while (_run)
            {
                try
                {
                    if(_listener.Pending())
                        AcceptClient(_listener.AcceptTcpClient());

                    else
                        Thread.Sleep(100);
                }
                catch(Exception x)
                {
                    Console.WriteLine(x.ToString());
                }
            }
            _listener.Stop();
            ConsoleHelpers.ConsoleHelper.WriteLine("Listener stopped");
        }

        private static void AcceptClient(TcpClient tcpClient)
        {
            ConsoleHelper.WriteLine("New connection etablished (" + tcpClient.Client.RemoteEndPoint.ToString() + ")");
            Connection c = new Connection(tcpClient);
            c.Disconnect += new Connection.DisconnectHandler(connectionDisconnect);
            _activeConnections.Add(c);
        }

        static void connectionDisconnect(Connection connection, string reason)
        {
            ConsoleHelper.WriteLine(connection + " quit (" + reason + ")");
            _activeConnections.Remove(connection);
            if(connection.Client != null)
                connection.Client.RemoveConnection(connection);
        }

        static void clientOnlineStatusChanged(Client client, bool online)
        {
            if(online)
                ConsoleHelper.WriteLine(client.ToString() + " is now online");
            else
                ConsoleHelper.WriteLine(client.ToString() + " is now offline");
        }
    }
}
