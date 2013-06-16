using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace XChat2.Common.Networking
{
    public static class StreamHelper
    {
        public static int ReadInt(Stream s)
        {
            return IPAddress.HostToNetworkOrder((int)Read(s, 4));
        }

        public static uint ReadUInt(Stream s)
        {
            byte[] a = new BinaryReader(s).ReadBytes(4);
            if(BitConverter.IsLittleEndian)
                Array.Reverse(a);
            return BitConverter.ToUInt32(a, 0);
        }

        public static short ReadShort(Stream s)
        {
            return IPAddress.HostToNetworkOrder((short)Read(s, 2));
        }

        public static long ReadLong(Stream s)
        {
            return IPAddress.HostToNetworkOrder((long)Read(s, 8));
        }

        public static double ReadDouble(Stream s)
        {
            byte[] b = new BinaryReader(s).ReadBytes(8);
            if(BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return BitConverter.ToDouble(b, 0);
        }

        public static float ReadFloat(Stream s)
        {
            byte[] b = new BinaryReader(s).ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return BitConverter.ToSingle(b, 0);
        }

        public static Boolean ReadBoolean(Stream s)
        {
            return new BinaryReader(s).ReadBoolean();
        }

        public static byte ReadByte(Stream s)
        {
            return new BinaryReader(s).ReadByte();
        }
        
        public static byte[] ReadBytes(Stream s, int count)
        {
            if (count == 0)
                return new byte[0];
            return new BinaryReader(s).ReadBytes(count);
        }

        public static byte[] ReadByteArray(Stream s)
        {
            int count = StreamHelper.ReadInt(s);
            return ReadBytes(s, count);
        }

        public static string ReadString(Stream s)
        {
            return ReadString(s, Encoding.BigEndianUnicode);
        }

        public static string ReadString(Stream s, Encoding encoding)
        {
            int length = ReadInt(s);
            byte[] buffer = ReadBytes(s, length);
            return encoding.GetString(buffer);
        }

        /// <summary>
        /// Writes a string to a stram using UTF-16 (Big Endian Unicode).
        /// </summary>
        /// <param name="s">Stream to write the string to</param>
        /// <param name="msg">The string to write to the stream</param>
        public static void WriteString(Stream s, string msg)
        {
            WriteString(s, msg, Encoding.BigEndianUnicode);
        }

        /// <summary>
        /// Writes a string to a stram using a specific encoding.
        /// </summary>
        /// <param name="s">Stream to write the string to</param>
        /// <param name="msg">The string to write to the stream</param>
        /// <param name="encoding">The encoding to use</param>
        public static void WriteString(Stream s, String msg, Encoding encoding)
        {
            byte[] buffer = encoding.GetBytes(msg);
            WriteInt(s, buffer.Length);
            s.Write(buffer, 0, buffer.Length);
        }

        public static void WriteInt(Stream s, int i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        public static void WriteUInt(Stream s, uint i)
        {
            byte[] a = BitConverter.GetBytes(i);
            if(BitConverter.IsLittleEndian)
                Array.Reverse(a);
            new BinaryWriter(s).Write(a, 0, a.Length);
        }

        public static void WriteLong(Stream s, long i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        public static void WriteShort(Stream s, short i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        public static void WriteDouble(Stream s, double d)
        {
            byte[] b = BitConverter.GetBytes(d);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            new BinaryWriter(s).Write(b, 0, b.Length);
        }

        public static void WriteFloat(Stream s, float f)
        {
            byte[] b = BitConverter.GetBytes(f);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            new BinaryWriter(s).Write(b, 0, b.Length);
        }

        public static void WriteBoolean(Stream s, Boolean b)
        {
            new BinaryWriter(s).Write(b);
        }

        public static void WriteByte(Stream s, byte b)
        {
            new BinaryWriter(s).Write(b);
        }

        public static void WriteBytes(Stream s, byte[] b)
        {
            new BinaryWriter(s).Write(b);
        }

        public static void WriteByteArray(Stream s, byte[] b)
        {
            if (b.LongLength > Int32.MaxValue)
                throw new ArgumentException("The length of the bytearray is limited to Int32");
            StreamHelper.WriteInt(s, b.Length);
        }

        public static Object Read(Stream s, int num)
        {
            byte[] b = new byte[num];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)s.ReadByte();
            }
            switch (num)
            {
                case 4:
                    return BitConverter.ToInt32(b, 0);
                case 8:
                    return BitConverter.ToInt64(b, 0);
                case 2:
                    return BitConverter.ToInt16(b, 0);
                default:
                    return 0;
            }
        }
    }
}
