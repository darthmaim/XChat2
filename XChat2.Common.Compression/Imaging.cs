using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace XChat2.Common.Compression
{
    public class Imaging
    {
        public static byte[] BitmapToByteArray(Bitmap b)
        {
            //string t = System.Threading.Thread.CurrentThread.Name;

            using(MemoryStream ms = new MemoryStream())
            {
                b.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public static Bitmap ByteArrayToBitmap(byte[] buffer)
        {
            using(MemoryStream ms = new MemoryStream(buffer))
            {
                Bitmap b = (Bitmap)Bitmap.FromStream(ms);
                return b;
            }
        }
    }
}
