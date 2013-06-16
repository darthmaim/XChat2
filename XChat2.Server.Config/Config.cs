using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.Configuration
{
    public class Config
    {
        private Config() { }

        private static Config _instance = null;

        public static Config GetConfig()
        {
            if (_instance == null)
                throw new ConfigException("Configuration not Loaded");

            return _instance;
        }

        public static bool LoadConfig()
        {
            _instance = new Config();

            return true;
        }

        private string _ipBinding = "0.0.0.0";
        public string IPBinding { get { return _ipBinding; } set { _ipBinding = value; } }

        private int _port = 2443;
        public int Port { get { return _port; } set { _port = value; } }
    }

    [Serializable]
    public class ConfigException : Exception
    {
        public ConfigException() { }
        public ConfigException(string message) : base(message) { }
        public ConfigException(string message, Exception inner) : base(message, inner) { }
        protected ConfigException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
