using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliDan.Live
{
    public class DanMuHeartbeatPacket : DanMuPacketHeader
    {
        new public const int Length = 4;

        public override byte[] ToBytes()
        {
            List<byte> buffer = new List<byte>(Length);
            buffer.AddRange(base.ToBytes());
            buffer.Add(0);
            buffer.Add(4);
            return buffer.ToArray();
        }

        public DanMuHeartbeatPacket()
            : base(DanMuPacketType.Heartbeat) { }
    }
}
