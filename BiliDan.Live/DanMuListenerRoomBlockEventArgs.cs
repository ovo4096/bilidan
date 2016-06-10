using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiliDan.Live
{
    public class DanMuListenerRoomBlockEventArgs : EventArgs
    {
        public int UserId { get; private set; }
        public string UserName { get; private set; }

        public DanMuListenerRoomBlockEventArgs(int userId, string userName)
        {
            this.UserId = userId;
            this.UserName = userName;
        }
    }
}
