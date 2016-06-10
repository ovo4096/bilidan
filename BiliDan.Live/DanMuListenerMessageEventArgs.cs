using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiliDan.Live
{
    public class DanMuListenerMessageEventArgs : EventArgs
    {
        public DateTime SendDateTime { get; private set; }
        public string Message { get; private set; }
        public int UserId { get; private set; }
        public string UserName { get; private set; }

        public DanMuListenerMessageEventArgs(DateTime sendDateTime, string message, int userId, string UserName)
        {
            this.SendDateTime = sendDateTime;
            this.Message = message;
            this.UserId = userId;
            this.UserName = UserName;
        }
    }
}
