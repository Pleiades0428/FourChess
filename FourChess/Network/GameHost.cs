using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    class GameHost
    {
        private TcpClient client;
        private TcpListener server;
        private XmlSerializer stepSerializer;
        private byte[] buffer;

        public static int Port = 8804;
        public static readonly GameHost Instance = new GameHost();
        public Action<StepInfo> OnReceive { get; set; }
        public Action OnAcceptClient { get; set; }
        public Action<string> OnError { get; set; }

        private GameHost()
        {
            server = new TcpListener(new IPEndPoint(0, GameHost.Port));
            //gameSerializer = new XmlSerializer(typeof(FourChessGame));
            stepSerializer = new XmlSerializer(typeof(StepInfo));
            buffer = new byte[1000];
        }

        public void StartHost()
        {
            Debug.Assert(this.OnReceive != null);
            server.Start(100);
            server.BeginAcceptTcpClient(new AsyncCallback(Acceptor), null);
        }

        private void Acceptor(IAsyncResult ar)
        {
            if (OnAcceptClient != null)
            {
                OnAcceptClient();
            }
            client = server.EndAcceptTcpClient(ar);
            client.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(InnerOnReceive), buffer);
        }

        private void InnerOnReceive(IAsyncResult ar)
        {
            if (this.OnReceive != null)
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

        public void StopHost()
        {
            client.Close();
            server.Stop();
        }

        public void Send(FourChessGame game)
        {
            NetworkStream stream = client.GetStream();
            stepSerializer.Serialize(stream, game);
        }
    }
}
