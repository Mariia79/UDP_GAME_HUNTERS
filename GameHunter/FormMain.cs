using GameModelsLib;
using System;
using System.Windows.Forms;
using GameServerKillers;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

//using System.Windows.Media;

namespace GameHunter
{
    public partial class FormGameField : Form
    {
       

        public FormGameField()
        {

            InitializeComponent();
       
        }

        private void FormGameField_Load(object sender, EventArgs e)
        {

           
        }


        private void FormGameField_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Game.MyHunter != null)
                Game.MyHunter.MoveHunterByPressKey(e);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("wrong Index");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            FormLogin login = new FormLogin();
            login.ShowDialog();

            if (!String.IsNullOrEmpty(login.PlayerName))
            {
                Game.InitGame(login.PlayerName, panelGameField, lblInfo, btnPlay);
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
           // Game.client.WriteLineAndGetReply("start", TimeSpan.FromSeconds(1));
        }

        private void lblInfo_Click(object sender, EventArgs e)
        {

        }

        private void lblInfo_TextChanged(object sender, EventArgs e)
        {
           
        }
    }
}
