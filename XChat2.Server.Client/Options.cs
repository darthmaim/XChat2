using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XChat2.Common.Networking;

namespace XChat2.Server.Clients
{
    internal class Options
    {
        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }
            private set { _data = value; }
        }

        private int _version;
        public int Version
        {
            get { return _version; }
            private set { _version = value; }
        }

        private Options()
        {
            _version = 0;
            _data = new byte[0];
        }

        private Options(Stream s)
        {
            _version = StreamHelper.ReadInt(s);
            int length = StreamHelper.ReadInt(s);

            _data = StreamHelper.ReadBytes(s, length);
        }

        public static Options LoadFromStream(Stream s)
        {
            return new Options(s);
        }

        public static Options LoadFromFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return Options.LoadFromStream(fs);
        }

        public static Options CreateNew()
        {
            return new Options();
        }

        public static bool Exists(string file)
        {
            return File.Exists(file);
        }

        public void Update(int version, byte[] data)
        {
            _version = version;
            _data = data;
        }

        public void SaveToFile(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                SaveToStream(fs);
        }

        public void SaveToStream(Stream s)
        {
            StreamHelper.WriteInt(s, _version);
            StreamHelper.WriteInt(s, _data.Length);
            StreamHelper.WriteBytes(s, _data);
        }
    }
}
