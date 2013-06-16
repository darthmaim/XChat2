using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace XChat2.Client.Update
{
    public static class Updater
    {
        public static UpdateResult Update(string currentVersion, string updateServer, bool forceUpdate)
        {
            UpdateDialog ud = new UpdateDialog(currentVersion, updateServer, forceUpdate);
            ud.ShowDialog();

            return new UpdateResult(ud.Restart, ud.NewVersion, ud.Executable);
        }

        public struct UpdateResult
        {
            private bool _restart;
            public bool Restart
            {
                get { return _restart; }
                internal set { _restart = value; }
            }

            private string _newVersion;
            public string NewVersion
            {
                get { return _newVersion; }
                internal set { _newVersion = value; }
            }

            private string _newExecutable;
            public string Executable
            {
                get { return _newExecutable; }
            }

            internal UpdateResult(bool restart, string newVersion, string executable)
            {
                _restart = restart;
                _newVersion = newVersion;
                _newExecutable = executable;
            }
        }
    }
}
