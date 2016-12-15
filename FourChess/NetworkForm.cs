using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FourChess.Network;
using FourChessCore;

namespace FourChess
{
    public partial class NetworkForm : Form
    {
        public NetworkForm()
        {
            InitializeComponent();
        }


        #region Host
        private void btnStartHost_Click(object sender, EventArgs e)
        {
            GameHost.Instance.OnAcceptClient = OnAcceptClient;
            GameHost.Instance.OnReceive = OnServerReceive;
            GameHost.Instance.StartHost();
            WriteLog("等待连接...");
        }

        private void SetNetWorkMode(bool isNetwork, bool isHost)
        {
            Player disablePlayer = isHost ? Player.Counter : Player.One;
            foreach (Control ctrl in this.Controls)
            {
                PieceInfo piece = ctrl.Tag as PieceInfo;
                if (ctrl is Button && piece != null && piece.Player == disablePlayer)
                {
                    ctrl.Enabled = false;
                }
            }
        }

        private void OnAcceptClient()
        {
            SetNetWorkMode(true, true);
            WriteLog("玩家加入");
        }

        private void OnServerReceive(StepInfo step)
        {
            WriteLog(step.ToString());
        }
        #endregion

        #region Client
        private void btnConnect_Click(object sender, EventArgs e)
        {
            GameClient.Instance.OnConnected = OnConnect;
            GameClient.Instance.OnReceive = OnClientReceive;
            GameClient.Instance.ConnectToHost(IPAddress.Parse(tbIP.Text));
            WriteLog("开始连接...");
        }

        private void OnConnect()
        {
            SetNetWorkMode(true, false);
            WriteLog("已连接");
        }

        private void OnClientReceive(StepInfo game)
        {
            WriteLog(game.ToString());
        }
        #endregion


        #region Helper
        private void WriteLog(string msg)
        {
            if (tbLog.InvokeRequired)
            {
                tbLog.Invoke(new Action(() =>
                {
                    tbLog.AppendText(msg + "\r\n");
                }));
            }
            else
            {
                tbLog.AppendText(msg + "\r\n");
            }
        }
        #endregion
    }
}
