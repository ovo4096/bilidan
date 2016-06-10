using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliDan.Live
{
    public class DanMuStartPacket : DanMuPacketHeader
    {
        new public const int Length = 12;

        public int RoomId { get; set; }
        public int UserId { get; set; }

        public override byte[] ToBytes()
        {
            List<byte> buffer = new List<byte>(Length);
            buffer.AddRange(base.ToBytes());
            buffer.AddRange(BitConverter.GetBytes((short)Length).Reverse());
            buffer.AddRange(BitConverter.GetBytes(RoomId).Reverse());
            buffer.AddRange(BitConverter.GetBytes(UserId).Reverse());
            return buffer.ToArray();
        }

        public DanMuStartPacket(int roomId, int userId)
            : base(DanMuPacketType.Start)
        {
            RoomId = roomId;
            UserId = userId;
        }
    }
}
