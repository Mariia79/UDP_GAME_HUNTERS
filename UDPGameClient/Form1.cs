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
            logoutButton.Enabled = false;
            sendButton.Enabled = false; 
            chatTextBox.ReadOnly = true; 

        }
  
        private void loginButton_Click(object sender, EventArgs e)
        {
            UDPClient.userName = userNameTextBox.SelectedItem.ToString();
            userNameTextBox.Enabled = false;

            try
            {
                UDPClient.ReadPortsSettings();// вичутеэмо значення портів
                Task receiveTask = new Task(ReceiveMessages);// включаємо приймач повідомленб
                receiveTask.Start();


                loginButton.Enabled = false;
                logoutButton.Enabled = true;
                sendButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (!String.IsNullOrEmpty(UDPClient.userName))
            {
                Game.InitGame(UDPClient.userName, panelGameField, lblInfo, btnPlay);
                Game.IsRun = true;
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
                        //string time = DateTime.Now.ToShortTimeString();
                        //chatTextBox.Text = message + "\r\n" + chatTextBox.Text;
                        Game.CheckHuntersAction(message);
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

        private void sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                string message = String.Format("{0}: {1}", UDPClient.userName, messageTextBox.Text);
                UDPClient.SendMessageInfo(message);
                messageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // обработчик нажатия кнопки logoutButton
        private void logoutButton_Click(object sender, EventArgs e)
        {
            ExitChat();
        }
        // выход из чата
        private void ExitChat()
        {
            string message = UDPClient.userName + " покидает чат";
            UDPClient.SendMessageInfo(message);

            UDPClient.client.DropMulticastGroup(UDPClient.groupAddress);

            UDPClient.alive = false;
            UDPClient.client.Close();

            loginButton.Enabled = true;
            logoutButton.Enabled = false;
            sendButton.Enabled = false;
        }
        // обработчик события закрытия формы
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

        }
    }
}