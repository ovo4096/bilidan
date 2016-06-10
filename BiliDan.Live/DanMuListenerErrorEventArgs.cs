using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiliDan.Live
{
    public class DanMuListenerErrorEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public DanMuListenerErrorEventArgs(Exception exception) { Exception = exception; }
    }
}
