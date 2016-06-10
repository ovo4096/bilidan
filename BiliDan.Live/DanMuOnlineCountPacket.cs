using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliDan.Live
{
    public class DanMuOnlineCountPacket
    {
        public const int Length = 4;
        public int Count { get; private set; }

        public DanMuOnlineCountPacket(byte[] data)
        {
            Count = BitConverter.ToInt32(data.Reverse().ToArray(), 0);
        }
    }
}
