using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiliDan.Live
{
    public class DanMuListenerOnlineCountUpdateEventArgs : EventArgs
    {
        public int Count { get; private set; }
        public DanMuListenerOnlineCountUpdateEventArgs(int count) { Count = count; }
    }
}
