using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.ConsoleHelpers
{
    public enum Align
    {
        Left,
        Center,
        Right
    }

    public class ConsoleHelper
    {
        public static void WriteLine(string text)
        {
            Write(text, Align.Left);
            CursorToNextLine();
        }

        private static void CursorToNextLine()
        {
            //Console.CursorLeft = 0;
            //Console.CursorTop++;
            Console.WriteLine();
        }

        /// <summary>
        /// Write colored Text to Console
        /// </summary>
        /// <param name="text">Text ($0 = Gray; $1 = Red; $2 = Green; $3 = Blue; $4 = Yellow; $5 = White; $6 = Black; $7 = Magenta; $8 = Cyan; $9 = Darkgray)</param>
        /// <param name="a">Align</param>
        public static void Write(string text, Align a)
        {
            int textLength = text.Length;
            int number = -1;
            for (int i = 0; i < text.Length; i++)
                if (text[i] == '$' && i < text.Length - 1 && int.TryParse(text[i + 1].ToString(), out number))
                {
                    i++;
                    textLength -= 2;
                }

            if(a == Align.Left)
            {
                if(Console.CursorLeft == 0)
                {
                    WriteColored("$0[$8" + DateTime.Now.ToLongTimeString() + "$0] ");
                }
                WriteColored(text);
            }
            else if(a == Align.Right)
            {
                Console.CursorLeft = Console.BufferWidth - textLength - 1;
                WriteColored(text);
                CursorToNextLine();
            }
            else if(a == Align.Center)
            {
                int left = ((Console.BufferWidth - Console.CursorLeft) / 2 + Console.CursorLeft) - (textLength / 2);
                if(left + textLength >= Console.BufferWidth) left = Console.BufferWidth - textLength;
                if(left < 0) left = 0;

                Console.CursorLeft = left;
                WriteColored(text);
                CursorToNextLine();
            }
        }

        private static void WriteColored(string text)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            for (int i = 0; i < text.Length; i++)
            {
                int number = -1;
                if (text[i] == '$' && i < text.Length - 1 && int.TryParse(text[i + 1].ToString(), out number))
                {
                    i++;
                    switch (number)
                    {
                        case 0:
                        default:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case 1:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case 3:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case 4:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case 5:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case 6:
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case 7:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case 8:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case 9:
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            break;
                    }
                }
                else
                {
                    Console.Write(text[i]);
                }
            }
        }
    }
}
