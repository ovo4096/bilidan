using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliDan.Live
{
    public class DanMuPacketHeader
    {
        public const int Length = 2;

        public DanMuPacketType PacketType { get; private set; }

        public virtual byte[] ToBytes()
        {
            byte[] buffer = BitConverter.GetBytes((short)PacketType);
            return buffer.Reverse().ToArray();
        }

        public DanMuPacketHeader(DanMuPacketType type)
        {
            PacketType = type;
        }

        public static DanMuPacketHeader FromData(byte[] data)
        {
            data = data.Reverse().ToArray();
            return new DanMuPacketHeader((DanMuPacketType)BitConverter.ToInt16(data, 0));
        }
    }
}
