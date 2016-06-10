using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiliDan.Live
{
    public class DanMuListenerRoomSilentOnEventArgs : EventArgs
    {
        public int Countdown { get; private set; }
        public bool IsNewbie { get; private set; }

        public DanMuListenerRoomSilentOnEventArgs(int countdown, bool isNewbie)
        {
            this.Countdown = countdown;
            this.IsNewbie = isNewbie;
        }
    }
}
