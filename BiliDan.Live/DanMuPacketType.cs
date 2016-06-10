using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliDan.Live
{
    public enum DanMuPacketType : short
    {
        Offline = 0,

        Start = 0x101,
        Heartbeat = 0x102,

        OnlineCount = 1,
        Chat = 4
    }
}
