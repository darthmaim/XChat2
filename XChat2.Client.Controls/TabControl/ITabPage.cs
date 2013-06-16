using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace XChat2.Client.Controls.TabControl
{
    public interface ITabPage
    {
        Font Font {get;}
        bool CanClose { get; }
        string Text { get; }
        bool IsSelected { get; set; }

        Size Size { get; }
    }
}
