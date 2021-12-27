using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using SimpleTCP;
using GameModelsLib;


namespace GameHunterServer
{

    class Hunter
    {
        public int Number;
        public string Name;
    }
    public partial class frmServerMain : Form
    {
        public frmServerMain()
        {
            InitializeComponent();
        }


        bool IsRun = false;

        List<Hunter> Hunters;
        System.Windows.Forms.Timer timer;
        SimpleTcpServer server;
        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13;//enter
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;
            server.ClientConnected += Server_ClientConnected;
            listClients.Items.Clear();

            Hunters = new List<Hunter>();



            timer = new System.Windows.Forms.Timer();
            timer.Interval = 5000;
            timer.Tick += Timer_Tick1;
            timer.Start();
            StartServer();

        }

        private void Timer_Tick1(object sender, EventArgs e)
        {
            if (IsRun) 
            {
                Random rnd = new Random();

                int vypadok = rnd.Next(1, 6);
                Point p = Game.GetRandomPosition(800, 600);
                 
                if (vypadok <= 1)
                {
                    server.BroadcastLine("*" + 1.ToString() + " " + p.X.ToString() + " " + p.Y.ToString() + " ");
                    return;
                }
                if (vypadok <= 3)
                {
                    server.BroadcastLine("*" + 2.ToString() + " " + p.X.ToString() + " " + p.Y.ToString() + " ");
                    return;
                }
                if (vypadok <= 6) 
                {
                    server.BroadcastLine("*" + 3.ToString() + " " + p.X.ToString() + " " + p.Y.ToString() + " ");
                    return;
                }
            }
        }


        private void Server_ClientConnected(object sender, System.Net.Sockets.TcpClient e)
        {
            if (!IsRun)
            {
                listClients.Invoke((MethodInvoker)delegate ()
                {
                    listClients.Items.Add(e.Client.LocalEndPoint.ToString());

                });
            }
            else
            {
                listClients.Items.Add("Cannot connect !!!Game is Run!!!");
            }

        }


        private void RegisterNewHunterAndSendMessagesToAllClients(string msg)
        {


            if (msg[0] == '_') // _Name
            {
                string plName = "";
                Hunter hunter = null;
                Point Point = new Point();

                plName = msg.Trim(new char[] { '_' });
                hunter = new Hunter();

                hunter.Number = Hunters.Count;
                hunter.Name = plName;
                Hunters.Add(hunter);

                string info = "_" + hunter.Number.ToString() + " " + hunter.Name;
                listClients.Invoke((MethodInvoker)delegate ()
                {
                    listClients.Items.Add(info);
                });

                if (Hunters.Count > 0)
                {
                    foreach (Hunter h in Hunters)
                    {
                        server.BroadcastLine("_" + h.Number.ToString() + " " + h.Name);
                        Thread.Sleep(100);
                    }
                }
            }
        }

        private void SendHuntersAction(string msg)  
        {
            if (msg.Length == 3)
            {
                server.BroadcastLine(msg);

            }
        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {

            string msg = e.MessageString;
            if (IsRun)
            {
                SendHuntersAction(msg);

                if (msg.Contains("end"))
                {
                    server.BroadcastLine(msg);
                    IsRun = false;
                    timer.Stop();

                }
            }
            else
            {

                RegisterNewHunterAndSendMessagesToAllClients(msg);
                if (msg.Contains("start"))
                {
                    server.BroadcastLine(msg);
                    IsRun = true;

                }

            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartServer();


        }

        void StartServer()
        {
            txtStatus.Text += "Server starting...";
            System.Net.IPAddress ip = System.Net.IPAddress.Parse(txtHost.Text);
            server.Start(ip, Convert.ToInt32(txtPort.Text));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
                server.Stop();

            listClients.Items.Clear();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            server.BroadcastLine("hello");
        }
    }
}
