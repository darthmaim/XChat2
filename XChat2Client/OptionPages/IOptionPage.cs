using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XChat2Client.OptionPages
{
    public interface IOptionPage
    {
        void Save();

        UserControl PageControl { get; }

        string[] Keywords { get; }
    }
}
