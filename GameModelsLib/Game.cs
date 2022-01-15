using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SimpleTCP;


using System.ComponentModel;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace GameModelsLib
{
    public enum GameEndTypes { None, AmmoLost, HunterIsDead, Win };
    public static class Game
    {
        public static SimpleTcpClient client;
        public static GameEndTypes GameEndType = GameEndTypes.None;
        public static int BorderAlarmWidth = 200;
        public static Label lblInfo;
        public static bool IsRun = false;
        public static PictureBox GameField;
        public static Button btnPlay;
        public static int Width = 800;
        public static int Height = 600;

        public static string PlayerName = "";

        public static int BoxGeneratorClientNumber = -1;



        public static System.Windows.Forms.Timer timer;




        private static int huntersCount = 0;
        public static int HuntersCount
        {
            get;
            set;
        }

        static public int PlayersCount = 4;

        static public List<Hunter> Hunters;

        static public Hunter MyHunter;



        public static void GetDropBox(string msg)
        {
            if (msg[0] == '*')
            {
                int typeOfBox = 0;
                int x = 0;
                int y = 0;

                msg = msg.Trim(new char[] { '*' });
                string[] words = msg.Split(new char[] { ' ' });

                typeOfBox = Convert.ToInt32(words[0]);
                x = Convert.ToInt32(words[1]);
                y = Convert.ToInt32(words[2]);
                Point p = new Point(x, y);

                Game.GameField.Invoke((MethodInvoker)delegate ()
                {
                    if (typeOfBox == 1)
                    {
                        Medik randomMedik = new Medik(p);
                        randomMedik.ApplySkin();
                        return;
                    }
                    else
                    if (typeOfBox == 2)
                    {
                        AmmoBox randomAmmo = new AmmoBox(p);
                        randomAmmo.ApplySkin();
                        return;
                    }
                    else
                    if (typeOfBox == 3)
                    {
                        SpeedBox randomSpeed = new SpeedBox(p);
                        randomSpeed.ApplySkin();
                    }

                });

            }

        }


        public static void CheckNewRegisteredHunters(string msg)
        {
            if (msg[0] == '_')
            {
                int num = 0;
                string name = "";

                msg = msg.Trim(new char[] { '_' });
                string[] words = msg.Split(new char[] { ' ' });
                num = Convert.ToInt32(words[0]);

                name = words[1];


                Game.GameField.Invoke((MethodInvoker)delegate ()
                {
                    AddNewConnectedPlayer(num, name);
                });


            }
        }


        public static void CreateClientHunter()
        {
            int num = Convert.ToInt32(PlayerName);
            LoginNewPlayer(num, PlayerName);


            if (MyHunter == null)
            {
                foreach (Hunter h in Hunters)
                {
                    if (h.Name.Contains(PlayerName))
                    {
                        MyHunter = h;
                        break;
                    }
                }

                MyHunter.ShowInfo();
                Game.lblInfo.Invoke((MethodInvoker)delegate ()
                {
                    lblInfo.Text = MyHunter.Name;
                });

                Game.btnPlay.Invoke((MethodInvoker)delegate ()
                {

                    btnPlay.Visible = num == 0;
                });
            }



        }

        public static void CheckHuntersAction(string msg)
        {
            if (msg[0] == '+')
            {
                int num = Convert.ToInt32(msg[1].ToString());
                char act = msg[2];

                if (MyHunter.Number != num)
                {
                    Hunters[num].MoveHunter(act);
                }
            }
        }

        public static void CheckGameState(string msg)
        {

            if (!IsRun)
            {
                CheckNewRegisteredHunters(msg);
                if (msg.Contains("start"))
                {
                    IsRun = true;
                }
            }
            else
            {
                CheckHuntersAction(msg);

                GetDropBox(msg);

                if (msg.Contains("end"))
                {
                    IsRun = false;

                    CheckEndGame();
                    return;
                }
            }
        }


        public static void StartDropBoxes()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 3000;
            timer.Tick += Timer_Tick1;
            timer.Start();
        }

        private static void Timer_Tick1(object sender, EventArgs e)
        {
            string line = "";
            if (IsRun)
            {
                SetBoxGeneratorAsNextActiveClient();

                if (BoxGeneratorClientNumber == MyHunter.Number)
                {
                    Random rnd = new Random();

                    int vypadok = rnd.Next(1, 6);
                    Point p = Game.GetRandomPosition(800, 600);

                    if (vypadok <= 1)
                    {
                        line = "*" + 1.ToString() + " " + p.X.ToString() + " " + p.Y.ToString() + " ";
                        UDPClient.SendMessageInfo(line);
                        GetDropBox(line);
                        return;
                    }
                    if (vypadok <= 3)
                    {
                        line = "*" + 2.ToString() + " " + p.X.ToString() + " " + p.Y.ToString() + " ";
                        UDPClient.SendMessageInfo(line);
                        GetDropBox(line);
                        return;
                    }
                    if (vypadok <= 6)
                    {
                        line = "*" + 3.ToString() + " " + p.X.ToString() + " " + p.Y.ToString() + " ";
                        UDPClient.SendMessageInfo(line);
                        GetDropBox(line);
                        return;
                    }
                }
            }
        }

        public static void SendHunterActionState(Hunter h)
        {
            UDPClient.SendMessageInfo("+" + h.ActionsState);
        }

        public static void SendHunterHealthAmmoState(Hunter h)
        {
            client.WriteLineAndGetReply(h.HealthAmmoState, TimeSpan.FromSeconds(0));
        }

        public static void InitGame(string plName, PictureBox p, Label l, Button btn)
        {
            btnPlay = btn;
            Restart();

            PlayerName = plName;
            huntersCount = 0;
            HuntersCount = 0;
            lblInfo = l;

            GameField = p;
            GameField.Controls.Clear();

            if (Hunters == null)
                Hunters = new List<Hunter>();

            for (int i = 0; i < PlayersCount; i++)
            {
                Hunter h = new Hunter(i.ToString(), Color.Blue, GetHunterPosition(i));
                h.Number = i;
                h.IsConect = false;
                Hunters.Add(h);
                HuntersCount++;

            }

            if (MyHunter == null)
            {
                MyHunter = Hunters[Convert.ToInt32(PlayerName)];
                MyHunter.IsConect = true;
                MyHunter.PreloadSkinImages();
                MyHunter.ApplySkin();

                MyHunter.ShowInfo();
            }

            UDPClient.SendMessageInfo("_" + Game.MyHunter.Number.ToString() + " " + Game.MyHunter.Name);

            StartDropBoxes();
            
        }


        public static void AddNewConnectedPlayer(int num, string name)
        {
            if (!Hunters[num].IsConect)
            {
                Hunters[num].IsConect = true;
                Hunters[num].PreloadSkinImages();
                Hunters[num].ApplySkin();
                UDPClient.SendMessageInfo("_" + Game.MyHunter.Number.ToString() + " " + Game.MyHunter.Name);
            }

        }

        public static Hunter LoginNewPlayer(int num, string name)
        {
            Color color;
            Hunter OtherNewHunter = null;
            if (HuntersCount < 4)
            {

                switch (num)
                {
                    case 0:
                        color = Color.Red;
                        break;
                    case 1:
                        color = Color.Blue;
                        break;
                    case 2:
                        color = Color.DarkGreen;
                        break;
                    default:
                        color = Color.White;
                        break;
                }
                OtherNewHunter = new Hunter(name, color, GetHunterPosition(num));
                OtherNewHunter.Name = name;
                OtherNewHunter.PreloadSkinImages();
                OtherNewHunter.ApplySkin();

                Hunters.Add(OtherNewHunter);
                OtherNewHunter.Number = num;
                HuntersCount++;

            }
            else
            {
                MessageBox.Show("Limit 4 players!");
            }

            return OtherNewHunter;
        }


        public static void CheckEndGame()
        {

            string myMessage = "";
            string WinnerName = "";
            if (Game.IsRun)
            {
                int alive = 0;

                foreach (Hunter h in Hunters)
                {
                    if (h.IsConect && h.LifeScore > 0)
                    {
                        WinnerName = h.Name;
                        alive++;
                    }
                }
                if (alive == 1)
                {
                    Game.IsRun = false;
                    UDPClient.SendMessageInfo("end");
                    lblInfo.Invoke((MethodInvoker)delegate ()
                    {
                        lblInfo.Text = myMessage + ". Game is over! " + WinnerName + " WIN !!!";
                    });
                    

                }
            }
        }


        public static void SetBoxGeneratorAsNextActiveClient()
        {
            BoxGeneratorClientNumber = 0;
            foreach (Hunter h in Hunters)
            {
                if (h.IsConect && h.LifeScore > 0)
                {
                    return;
                    BoxGeneratorClientNumber++;
                }
            }
        }


        public static Point GetRandomPosition()
        {
            int delta = 200;
            Point p = new Point(0, 0);
            Random random = new Random();
            p.X = random.Next(0 + delta, GameField.Width - delta);
            p.Y = random.Next(0 + delta, GameField.Height - delta);
            return p;
        }

        public static Point GetRandomPosition(int w, int h)
        {
            int delta = 150;
            Point p = new Point(0, 0);
            Random random = new Random();
            p.X = random.Next(0 + delta, w - delta);
            p.Y = random.Next(0 + delta, h - delta);
            return p;
        }


        static Point GetHunterPosition(int num)
        {
            Point p = new Point(0, 0);
            switch (num)
            {
                case 0:
                    p.X = 50;
                    p.Y = 50;
                    return p;
                case 1:
                    p.X = GameField.Width - 50;
                    p.Y = GameField.Height - 50;
                    return p;
                case 2:
                    p.X = 50;
                    p.Y = GameField.Height - 50;
                    return p;
                case 3:
                    p.X = GameField.Width - 50;
                    p.Y = 50;
                    return p;
            }

            return p;
        }


        public static void Restart()
        {

            if (Game.GameField != null)
                while (Game.GameField.Controls.Count > 1)
                {
                    Game.GameField.Invoke((MethodInvoker)delegate ()
                    {
                        Game.GameField.Controls.Remove(Game.GameField.Controls[1]);
                    });
                }

        }




    }
}
