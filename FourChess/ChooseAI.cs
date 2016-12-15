using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FourChessCore;

namespace FourChess
{
    public partial class ChooseAI : Form
    {
        public ChooseAI()
        {
            InitializeComponent();
        }

        public GamePlayer Player
        {
            get
            {
                return lbAIList.SelectedItem as GamePlayer;
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void ChooseAI_Load(object sender, EventArgs e)
        {
            AIPlayer player1 = new AIPlayer("普通青年", AIStyle.PreferRandom);
            AIPlayer player2 = new AIPlayer("飘逸小青年", AIStyle.PreferFar);
            AIPlayer player3 = new AIPlayer("稳重的棋手", AIStyle.PreferShrink);
            AIPlayer player4 = new AIPlayer("职业玩家", AIStyle.PreferShrink | AIStyle.Professional);
            AIPlayerNew player5 = new AIPlayerNew("职业玩家2");

            lbAIList.Items.Add(player1);
            lbAIList.Items.Add(player2);
            lbAIList.Items.Add(player3);
            lbAIList.Items.Add(player4);
            lbAIList.Items.Add(player5);
        }
    }
}
