using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using GameModelsLib;
using System.IO;
using GameModelsLib;

namespace UDPGameClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            loginButton.Enabled = true;


        }
  
        private void loginButton_Click(object sender, EventArgs e)
        {
            UDPClient.userName = userNameTextBox.SelectedItem.ToString();
            userNameTextBox.Enabled = false;

            btnPlay.Visible = UDPClient.userName == "0";
            try
            {
                UDPClient.ReadPortsSettings();// вичутеэмо значення портів
                Task receiveTask = new Task(ReceiveMessages);// включаємо приймач повідомленб
                receiveTask.Start();


                loginButton.Enabled = false;
        
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (!String.IsNullOrEmpty(UDPClient.userName))
            {
                Game.InitGame(UDPClient.userName, panelGameField, lblInfo, btnPlay);
                Game.IsRun = false;
            }
        }


        private void ReceiveMessages()
        {
            UDPClient.alive = true;
            try
            {
                while (UDPClient.alive)
                {
                    IPEndPoint remoteIp = null;
                    byte[] data = UDPClient.client.Receive(ref remoteIp);
                    string message = Encoding.Unicode.GetString(data);

                    this.Invoke(new MethodInvoker(() =>
                    {
                        Game.CheckGameState(message);
                        Game.CheckNewRegisteredHunters(message);

                    }));
                }
            }
            catch (ObjectDisposedException)
            {
                if (!UDPClient.alive)
                    return;
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            ExitChat();
        }

        private void ExitChat()
        {
            UDPClient.client.DropMulticastGroup(UDPClient.groupAddress);

            UDPClient.alive = false;
            UDPClient.client.Close();

            loginButton.Enabled = true;
        }
 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UDPClient.alive)
                ExitChat();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Game.MyHunter != null)
                Game.MyHunter.MoveHunterByPressKey(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UDPClient.SendMessageInfo("start");
            Game.IsRun = true;
        }
    }
}