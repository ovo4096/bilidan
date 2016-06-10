using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace BiliDan.Live
{
    public class DanMuListener : IDisposable
    {
        public event DanMuListenerRoomSilentOnHandler RoomSlientOn;
        public event DanMuListenerRoomSilentOffHandler RoomSlientOff;
        public event DanMuListenerRoomBlockHandler RoomBlockInto;
        public event DanMuListenerRoomBlockHandler RoomBlockMessage;

        public event DanMuListenerMessageEventHandler Message;
        public event DanMuListenerErrorEventHandler Error;
        public event DanMuListenerOnlineCountUpdateEventHandler OnlineCountUpdate;

        public event EventHandler Stoped;

        public bool IsRun { get; private set; }
        public int RoomId { get; private set; }
        public int UserId { get; private set; }

        private CancellationTokenSource cts;
        private const int HeartbeatSpanTime = 30000;

        public const int MaxRoomId = 10999999;
        public const int MinRoomId = 1;

        public DanMuListener(int roomId, int userId)
        {
            this.RoomId = roomId;
            this.UserId = userId;
        }

        public DanMuListener(int roomId)
            : this(roomId, 0) { }

        public void Stop()
        {
            if (!IsRun) return;
            IsRun = false;

            cts.Cancel();
        }

        protected virtual void OnStoped(EventArgs e)
        {
            if (Stoped != null)
            {
                Stoped(this, e);
            }
        }

        protected virtual void OnRoomSilentOn(DanMuListenerRoomSilentOnEventArgs e)
        {
            if (RoomSlientOn != null) RoomSlientOn(this, e);
        }

        protected virtual void OnRoomSilentOff(EventArgs e)
        {
            if (RoomSlientOff != null) RoomSlientOff(this, e);
        }

        protected virtual void OnRoomBlockInto(DanMuListenerRoomBlockEventArgs e)
        {
            if (RoomBlockInto != null) RoomBlockInto(this, e);
        }

        protected virtual void OnRoomBlockMessage(DanMuListenerRoomBlockEventArgs e)
        {
            if (RoomBlockMessage != null) RoomBlockMessage(this, e);
        }

        protected virtual void OnError(DanMuListenerErrorEventArgs e)
        {
            if (Error != null) Error(this, e);
        }

        protected virtual void OnMessage(DanMuListenerMessageEventArgs e)
        {
            if (Message != null) Message(this, e);
        }

        protected virtual void OnOnlineCountUpdate(DanMuListenerOnlineCountUpdateEventArgs e)
        {
            if (OnlineCountUpdate != null) OnlineCountUpdate(this, e);
        }

        public void Start() { StartAsync(); }

        private async void StartAsync()
        {
            if (IsRun) return;
            IsRun = true;

            cts = new CancellationTokenSource();

            try
            {
                await StartAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                OnStoped(new EventArgs());
            }
            catch (Exception e)
            {
                IsRun = false;
                OnError(new DanMuListenerErrorEventArgs(e));
            }
        }

        private void SendHeartbeat(object state)
        {
            try
            {
                NetworkStream stream = state as NetworkStream;

                DanMuHeartbeatPacket liveHeartbeatPacket = new DanMuHeartbeatPacket();
                byte[] buffer = liveHeartbeatPacket.ToBytes();

                stream.Write(buffer, 0, buffer.Length);
            }
            catch (ObjectDisposedException) { return; }
        }

        private class JMessage
        {
            public JArray info { get; set; }
            public string cmd { get; set; }
            public int roomid { get; set; }
            public int countdown { get; set; }
            public bool is_newbie { get; set; }
            public int uid { get; set; }
            public string uname { get; set; }
        }

        private async Task StartAsync(CancellationToken ct)
        {

            TcpClient client = new TcpClient();
            NetworkStream stream = null;
            Timer heartbeatTimer = null;
            byte[] buffer;
            try
            {
                await client.ConnectAsync("livecmt.bilibili.com", 88);

                ct.ThrowIfCancellationRequested();

                stream = client.GetStream();

                heartbeatTimer = new Timer(SendHeartbeat, stream, Timeout.Infinite, Timeout.Infinite);

                DanMuStartPacket startPacket = new DanMuStartPacket(RoomId, UserId);
                buffer = startPacket.ToBytes();
                await stream.WriteAsync(buffer, 0, buffer.Length, ct);

                heartbeatTimer.Change(HeartbeatSpanTime, HeartbeatSpanTime);

                while (true)
                {
                    ct.ThrowIfCancellationRequested();

                    buffer = new byte[DanMuPacketHeader.Length];
                    await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                    DanMuPacketHeader packetHeader = DanMuPacketHeader.FromData(buffer);

                    switch (packetHeader.PacketType)
                    {
                        case DanMuPacketType.OnlineCount:

                            buffer = new byte[DanMuOnlineCountPacket.Length];
                            await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                            DanMuOnlineCountPacket onlineCountPacket = new DanMuOnlineCountPacket(buffer);
                            OnOnlineCountUpdate(new DanMuListenerOnlineCountUpdateEventArgs(onlineCountPacket.Count));

                            break;

                        case DanMuPacketType.Chat:

                            buffer = new byte[DanMuChatPacket.ChatJsonTextLengtFieldByteLength];
                            await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                            buffer = new byte[DanMuChatPacket.GetChatJsonTextLength(buffer)];
                            await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                            DanMuChatPacket chatPacket = new DanMuChatPacket(buffer);

                            JMessage jMessage = JsonConvert.DeserializeObject<JMessage>(chatPacket.ChatJsonText);

                            switch (jMessage.cmd)
                            {
                                case "DANMU_MSG":

                                    JArray danMuInfo = (JArray)jMessage.info[0];
                                    string danMuMessage = (string)jMessage.info[1];
                                    JArray danMuUserInfo = (JArray)jMessage.info[2];

                                    DateTime danMuTime = new DateTime(1970, 1, 1).AddSeconds((long)danMuInfo[4]).ToLocalTime();

                                    int userId = (int)danMuUserInfo[0];
                                    string userName = (string)danMuUserInfo[1];
                                    bool isAdmin = (bool)danMuUserInfo[2];
                                    OnMessage(new DanMuListenerMessageEventArgs(danMuTime, danMuMessage, userId, userName));

                                    break;

                                case "ROOM_SILENT_ON":
                                    OnRoomSilentOn(new DanMuListenerRoomSilentOnEventArgs(jMessage.countdown, jMessage.is_newbie));
                                    break;

                                case "ROOM_SILENT_OFF":
                                    OnRoomSilentOff(new EventArgs());
                                    break;

                                case "ROOM_BLOCK_MSG":
                                    OnRoomBlockMessage(new DanMuListenerRoomBlockEventArgs(jMessage.uid, jMessage.uname));
                                    break;

                                case "ROOM_BLOCK_INTO":
                                    OnRoomBlockInto(new DanMuListenerRoomBlockEventArgs(jMessage.uid, jMessage.uname));
                                    break;

                                default:
                                    break;
                            }

                            break;

                        default:

                            throw new DanMuDataPacketUnknownException();
                    }
                }
            }
            finally
            {
                if (heartbeatTimer != null) heartbeatTimer.Dispose();
                if (stream != null) stream.Close();
                client.Close();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
