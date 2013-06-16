using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XChat2.Common.Networking;

namespace XChat2.Client.Data
{
    public class Options
    {
        private static Options _instance;

        public const int Version = 1;

        private Options()
        {
            //set defaults:
            TimeFormat = "%T";
            ShowTime = true;
        }

        public void SetData(int version, byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                switch (version)
                {
                    //empty options
                    case 0:
                        return;
                    case 1:
                        LoadVersion1(ms);
                        break;
                }
            }
        }

        public byte[] getData()
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                StreamHelper.WriteString(ms, _timeFormat);
                StreamHelper.WriteBoolean(ms, _showTime);

                //========
                ms.Flush();
                data = ms.ToArray();
            }
            return data;
        }

        private void LoadVersion1(Stream s)
        {
            _timeFormat.Value = StreamHelper.ReadString(s);
            _showTime.Value = StreamHelper.ReadBoolean(s);
        }
        
        public static Options Instance
        {
            get { return _instance ?? (_instance = new Options()); }
        }

        private Option<string> _timeFormat;
        public Option<string> TimeFormat
        {
            get { return _timeFormat; }
            private set { _timeFormat = value; }
        }

        private Option<bool> _showTime;
        public Option<bool> ShowTime
        {
            get { return _showTime; }
            private set { _showTime = value; }
        }
    }
}
