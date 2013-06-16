using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace XChat2.Common.Helper
{
    public static class Misc
    {
        public static bool WaitFor<T>(T obj, Predicate<T> predicate, int timeout, int waitTime)
        {
            for (int i = 0; i < timeout; i += waitTime)
            {
                if (predicate(obj))
                    return true;
                Thread.Sleep(waitTime);
            }
            return false;
        }

        public static void Raise<T>(this EventHandler<T> @event, object sender, T eventArgs) where T: EventArgs
        {
            if (@event != null)
                @event(sender, eventArgs);
        }
    }
}
