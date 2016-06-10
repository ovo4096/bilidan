using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliDan.Live
{
    public class DanMuChatPacket
    {
        public const int ChatJsonTextLengtFieldByteLength = 2;

        public string ChatJsonText;

        public static short GetChatJsonTextLength(byte[] data)
        {
            data = data.Reverse().ToArray();
            return (short)(BitConverter.ToInt16(data, 0) - 4);
        }

        public DanMuChatPacket(byte[] data)
        {
            ChatJsonText = Encoding.UTF8.GetString(data);
        }
    }
}
