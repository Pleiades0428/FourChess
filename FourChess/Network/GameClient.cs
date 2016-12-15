using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FourChessCore;

namespace FourChess.Network
{
    class GameClient
    {
        private TcpClient client;
        private TcpListener listener;
        private XmlSerializer stepSerializer;
        private byte[] buffer;

        public static readonly GameClient Instance = new GameClient();
        public Action<StepInfo> OnReceive { get; set; }
        public Action OnConnected { get; set; }
        public Action<string> OnError { get; set; }

        private GameClient()
        {
            client = new TcpClient();
            stepSerializer = new XmlSerializer(typeof(StepInfo));
            buffer = new byte[1000];
        }

        public void ConnectToHost(IPAddress addr)
        {
            client.BeginConnect(addr, GameHost.Port, new AsyncCallback(InnerOnConnected), null);
        }

        private void InnerOnConnected(IAsyncResult ar)
        {
            if (OnConnected != null)
            {
                OnConnected();
            }
            client.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(InnerOnReceive), null);
        }

        private void InnerOnReceive(IAsyncResult ar)
        {
            if (OnReceive != null)
            {
                string msg = Encoding.Default.GetString(buffer);
                StringReader reader = new StringReader(msg);
                StepInfo step;
                try
                {
                    step = stepSerializer.Deserialize(reader) as StepInfo;
                    OnReceive(step);
                }
                catch (Exception ex)
                {
                    if (this.OnError != null)
                    {
                        OnError(ex.Message);
                    }
                }
            }
        }
    }
}
