using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace XChat2.Client.Controls.ChatBox
{
    public interface IChatBoxEntry
    {
        int Index { get; set; }

        string Text { get; }
        string[] Lines { get; }
        int Height { get; }
        void RecalculateSize(int width);
        void Draw(Graphics g, int top);

        int LeftMarginInLine(int line);

        bool IsSelected { get; set; }
        int SelectionStart { get; set; }
        int SelectionEnd { get; set; }
        string SelectedText { get; }
    }
}
