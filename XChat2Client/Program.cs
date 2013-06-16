using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XChat2.Common.Configuration;
using System.IO;
using XChat2.Client.Update;
using XChat2.Client.Communication;
using System.Diagnostics;

namespace XChat2Client
{
    static class Program
    {
#if(!DEBUG)
        public const string VERSION = "1.0.2.8";
#else
        public const string VERSION = "DEBUG";
#endif

        private static Config _config;
        public static Config Config
        {
            get { return _config; }
        }

        private static string _server;
        public static string Server
        {
            get { return _server; }
        }
        
        private static int _port;
        public static int Port
        {
            get { return _port; }
        }

        private static bool _forceUpdate;
        public static bool ForceUpdate
        {
            get { return _forceUpdate;}
        }

        private static string _lastUser;
        public static string LastUser
        {
            get { return _lastUser; }
        }


        [STAThread]
        static void Main(string[] arguments)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (sender, args) => UnhandledException(args.Exception);
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, args) => UnhandledException((Exception) args.ExceptionObject);

            LoadConfig();

            ParseArguments(arguments);

            DeleteUpdateBackup();

#if(!DEBUG)
            if (!arguments.Contains("-debug") && !arguments.Contains("-skipupdates"))
            {
                if (Update())
                    return;
            }
#endif

            Run();

            _config.Save();
        }

        private static void UnhandledException(Exception x)
        {
            try
            {
                string path = Path.Combine(configPath, "error." + DateTime.Now.ToBinary() + ".log");

                string exceptionString = x.ToString();
                Exception xr = x.InnerException;
                while (xr != null)
                {
                    exceptionString += Environment.NewLine + Environment.NewLine + xr.ToString();
                    xr = xr.InnerException;
                }

                File.WriteAllText(path, exceptionString);
                if (
                    MessageBox.Show(
                        "Unhandled Exception: " + x.GetType().FullName + Environment.NewLine + Environment.NewLine +
                        "XChat2Client has to be closed, do you want to see the exception details?",
                        "XChat2Client - Unhandled Exception", MessageBoxButtons.YesNo, MessageBoxIcon.Error) ==
                    DialogResult.Yes)
                {
                    Process.Start(path);
                }

                Environment.Exit(666);
            } catch {}
        }

        private static void Run()
        {
            LoginForm lf = new LoginForm(_server, _port, _lastUser);
            lf.FormClosed += (o, e) => Application.Exit();
            lf.LoginSuccessful += new Action<LoginForm, ClientConnection>(lf_LoginSuccessful);
            lf.Show();
            Application.Run();
        }

        static void lf_LoginSuccessful(LoginForm sender, ClientConnection connection)
        {
            _config["general"].get<string>("lastuser").Value = connection.Username;
            _lastUser = connection.Username;

            sender.Hide();
            ContactListForm clf = new ContactListForm(connection);
            clf.FormClosed += (o, e) =>
            {
                if (clf.Exit)
                    sender.Close();
                else
                    sender.Show();
            };
            clf.Show();
        }

        private static bool Update()
        {
            try
            {
                Updater.UpdateResult ur = Updater.Update(VERSION, _config["update"].get<string>("server").Value, _forceUpdate);
                if (ur.Restart)
                {
                    string executable = Path.Combine(Application.StartupPath, ur.Executable);
                    Process.Start(executable, Environment.CommandLine.Replace("-forceupdate", " ") + " -waitFor " + Process.GetCurrentProcess().Id);
                    return true;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
                return false;
            }
            return false;
        }

        private static void DeleteUpdateBackup()
        {
            if (Directory.Exists("old"))
                Directory.Delete("old", true);
        }

        private static void ParseArguments(string[] args)
        {
            if (args.Length >= 2)
                for (int i = 0; i < args.Length - 1; i++)
                {
                    if (args[i] == "-server" || args[i] == "-s")
                        _server = args[++i];
                    else if (args[i] == "-port" || args[i] == "-p")
                    {
                        int oldPort = _port;
                        if (!int.TryParse(args[i], out _port))
                            _port = oldPort;
                    }
                    else if (args[i] == "-waitFor")
                    {
                        int waitForProcess = int.Parse(args[++i]);
                        try
                        {
                            Process.GetProcessById(waitForProcess).WaitForExit(5000);
                        }
                        catch (ArgumentException) { }
                    }
                }

            _forceUpdate = args.Contains("-forceupdate");
        }

        static string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "XChat2");
            
        private static void LoadConfig()
        {
            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);
            _config = new XChat2.Common.Configuration.Config(Path.Combine(configPath, "main.cfg"));
            SetDefaultConfig();

            _server = _config["general"].get<string>("server").Value;
            _port = _config["general"].get<int>("port").Value;
            _lastUser = _config["general"].get<string>("lastuser").Value;
        }

        private static void SetDefaultConfig()
        { 
            if(!_config.ContainsSection("general"))
                _config.AddSection("general");
            if(!_config["general"].ContainsEntry("server"))
                _config["general"].get<string>("server").Value = "xchat.darthmaim.de";
            if(!_config["general"].ContainsEntry("port"))
                _config["general"].get<int>("port").Value = 2443;
            if(!_config["general"].ContainsEntry("lastuser"))
                _config["general"].get<string>("lastuser").Value = "";

            if(!_config.ContainsSection("update"))
                _config.AddSection("update");
            if(!_config["update"].ContainsEntry("server"))
                _config["update"].get<string>("server").Value = "update.xchat.darthmaim.de";

            if (!_config.ContainsSection("P2P"))
                _config.AddSection("P2P");
            if (!_config["P2P"].ContainsEntry("server"))
                 _config["P2P"].get<string>("server").Value = "p2p.xchat.darthmaim.de";
            if (!_config["P2P"].ContainsEntry("port"))
                 _config["P2P"].get<int>("port").Value = 2443;
      
            _config.Save();
        }
    }
}
