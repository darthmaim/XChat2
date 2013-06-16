using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace XChat2.Client.Controls
{
    class Helper
    {
        public static string[] wrapText(string text, int firstLineWidth, int otherLinesWidth, Font font)
        {
            text = text.Replace("\r\n", "\n");
            text = text.Replace("\r", "\n");

            List<string> lines = new List<string>();

            foreach(string s in text.Split('\n'))
                lines.AddRange(internalWrapText(s, lines.Count == 0 ? firstLineWidth : otherLinesWidth, otherLinesWidth, font));
            
            return lines.ToArray();
        }

        public static string[] internalWrapText(string text, int firstLineWidth, int otherLinesWidth, Font font)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start(); sw.Stop(); sw.Reset();
            sw.Start();

            List<string> lines = new List<string>();

            int totalLength = TextRenderer.MeasureText(text, font).Width;
            if(totalLength < firstLineWidth)
                return text.Split('\n');

            int firstLineCount = (int)(((double)firstLineWidth / (double)totalLength) * text.Length);

            while(TextRenderer.MeasureText(text.Substring(0, firstLineCount), font).Width < firstLineWidth)
            {
                firstLineCount++;
            }
            firstLineCount--;
            while(text[firstLineCount] != ' ' && text.Substring(0, firstLineCount).Contains(' '))
            {
                firstLineCount--;
            }
            lines.Add(text.Substring(0, firstLineCount));

            text = text.Substring(firstLineCount).Trim();

            while(text.Length > 0)
            {
                int remainingLength = TextRenderer.MeasureText(text, font).Width;
                if(remainingLength < otherLinesWidth)
                {
                    lines.Add(text);
                    break;
                }

                int lineCount = (int)(((double)otherLinesWidth / (double)remainingLength) * text.Length);

                while(TextRenderer.MeasureText(text.Substring(0, lineCount), font).Width < otherLinesWidth)
                    lineCount++;
                lineCount--;
                while(text[lineCount] != ' ' && text.Substring(0, lineCount).Contains(' '))
                    lineCount--;
                lines.Add(text.Substring(0, lineCount));
                text = text.Substring(lineCount).Trim();
            }

            sw.Stop();

            Debug.WriteLine(string.Format("Wrap text in {0}ms ({1}ns).", sw.ElapsedMilliseconds, ((double)sw.ElapsedTicks / (double)Stopwatch.Frequency) * 1000000000.0));
            
            return lines.ToArray();
        }

        public static string[] wrapText2(string text, int firstLineWidth, int otherLinesWidth, Font font) {
            Stopwatch sw = new Stopwatch();
            sw.Start(); sw.Stop(); sw.Reset();
            sw.Start();
            
            int totalLength = TextRenderer.MeasureText("(" + text + ")", font).Width - TextRenderer.MeasureText("()", font).Width;
            if(totalLength < firstLineWidth)
                return new string[] { text };

            List<string> lines = new List<string>();

            int length = 0;
            for(int i = 0; i < text.Length; i++)
            {
                int lineLength = (lines.Count == 0 ? firstLineWidth : otherLinesWidth);
                if(GetCharWidth(text[i], font) + length >= lineLength && length < lineLength) {
                    while(text.Substring(0, i - 1).Contains(' ') && /*!char.IsWhiteSpace(text[i])*/ text[i] != ' ') i--;
                    lines.Add(text.Substring(0, i));
                    text = text.Substring(i);
                    i = 0;
                    length = 0;
                }
                length += GetCharWidth(text[i], font);

            }

            lines.Add(text);

            sw.Stop();

            Debug.WriteLine(string.Format("Wrap text in {0}ms ({1}ns).", sw.ElapsedMilliseconds, ((double)sw.ElapsedTicks / (double)Stopwatch.Frequency) * 1000000000.0));

            return lines.ToArray();
        }

        private static Dictionary<Font, Dictionary<char, int>> _fontCharSizesDictionary = new Dictionary<Font, Dictionary<char, int>>();
        private static int GetCharWidth(char c, Font f)
        {
            if(!_fontCharSizesDictionary.ContainsKey(f))
                _fontCharSizesDictionary.Add(f, new Dictionary<char, int>());
            if(!_fontCharSizesDictionary[f].ContainsKey(c))
            {
                _fontCharSizesDictionary[f].Add(c, TextRenderer.MeasureText("(" + c + ")", f).Width - TextRenderer.MeasureText("()", f).Width);
            }
            return _fontCharSizesDictionary[f][c];
        }

        private static int GetTotalWidth(string text, Font f)
        {
            return TextRenderer.MeasureText("(" + text + ")", f).Width - TextRenderer.MeasureText("()", f).Width;
        }

        internal static int GetHoveredChar(string line, int y, Font font)
        {
            int totalWidth = GetTotalWidth(line, font);
            if(y < 0 || y > totalWidth)
                return -1;

            int width = 2;
            for(int i = 0; i < line.Length; i++)
            {
                width += GetCharWidth(line[i], font);
                if(width >= y)
                    return i;
            }

            throw new ArgumentOutOfRangeException();
        }

        public static Rectangle RectangeOfCharAtIndex(string text, int index, Font font)
        {
            if(index < 0 || index >= text.Length)
                return new Rectangle();

            int leftMargin = (int)(((double)TextRenderer.MeasureText(text, font).Width - (double)GetTotalWidth(text, font)) / 2d);

            int start = leftMargin;
            for(int i = 0; i < index; i++)
            {
                start += GetCharWidth(text[i], font);
            }

            int width = GetCharWidth(text[index], font);
            return new Rectangle(start, 0, width + 1, 0);
        }
    }
}
