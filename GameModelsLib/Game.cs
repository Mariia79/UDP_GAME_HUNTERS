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

using SimpleTCP;

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


        public static System.Windows.Forms.Timer timer;

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Random rnd = new Random();

            int vypadok = rnd.Next(1, 6);
            if (vypadok <= 1)
            {
                Medik randomMedik = new Medik(GetRandomPosition(800, 600));
                randomMedik.ApplySkin();
                return;
            }
            if (vypadok <= 2)
            {
                AmmoBox randomAmmo = new AmmoBox(GetRandomPosition(800, 600));
                randomAmmo.ApplySkin();
                return;
            }
            if (vypadok <= 6)
            {
                SpeedBox randomSpeed = new SpeedBox(GetRandomPosition(800, 600));
                randomSpeed.ApplySkin();
            }
        }


        private static int huntersCount = 0;
        public static int HuntersCount
        {
            get;
            set;
        }

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

                bool find = false;
                foreach (Hunter h in Hunters)
                {
                    if (h.Number == num)
                        find = true;
                }
                if (!find)
                {
                    Game.GameField.Invoke((MethodInvoker)delegate ()
                    {
                        LoginNewPlayer(num, name);
                    });
                }

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
        }

        public static void CheckHuntersAction(string msg)
        {
            if (msg.Length == 3)
            {
                int num = Convert.ToInt32(msg[0].ToString());
                char act = msg[1];

                if (MyHunter.Number != num)
                {
                    Hunters[num].MoveHunter(act);
                }


            }
        }

        public static void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            string msg = e.MessageString.ToString();


            if (!IsRun)
            {
                CheckNewRegisteredHunters(msg); 
                if (msg.Contains("start"))
                {
                    IsRun = true;

                    if (MyHunter != null)
                    {

                        OnChangeGame();
                    }
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
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public static void SendHunterActionState(Hunter h)
        {
            client.WriteLineAndGetReply(h.ActionsState, TimeSpan.FromSeconds(0));
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

            if (client == null)
            {
                client = new SimpleTcpClient();
                client.StringEncoder = Encoding.UTF8;
                client.DataReceived += Client_DataReceived;
            }

            client.Connect("127.0.0.1", 8910);
            huntersCount = 0;


            lblInfo = l;

            GameField = p;
            GameField.Controls.Clear();

            if (Hunters == null)
                Hunters = new List<Hunter>();
            client.WriteLineAndGetReply("_" + plName, TimeSpan.FromSeconds(0));



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


        static bool CheckEndGame()
        {
            bool IsEndGame = false;

            lblInfo.Invoke((MethodInvoker)delegate ()
            {
                string myMessage = "";


               

                if (!IsEndGame)
                {
                    int alive = 0;
                    string WinnerName = "";
                    foreach (Hunter h in Hunters)
                    {
                        if (h.LifeScore > 0)
                        {
                            WinnerName = h.Name;
                            alive++;
                        }
                    }
                    if (alive == 1)
                    {
                        lblInfo.Text = myMessage + ". Game is over! " + WinnerName + " WIN !!!";
                        IsEndGame = true;
                        Game.client.WriteLineAndGetReply("end", TimeSpan.FromSeconds(1));
                    }
                }

                if (MyHunter == null)
                {

                    if (timer != null)
                    {
                        timer.Stop();
                        timer = null;
                    }

                    GameEndType = GameEndTypes.HunterIsDead;
                    //lblInfo.Text = myMessage + ". Game is over! " + PlayerName + " is dead!";
                    IsEndGame = true;
                }

            });
            return IsEndGame;
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

        static Point GetHunterPosition()
        {

            Point p = new Point(0, 0);

            p.X = GameField.Width / 2;
            p.Y = GameField.Height - 100;
            return p;
        }


        static Point GetHunterPosition(int num)
        {
            Point p = new Point(0, 0);
            switch (num)
            {
                case 0:
                    p.X = 100;
                    p.Y = 100;
                    return p;
                case 1:
                    p.X = GameField.Width - 100;
                    p.Y = GameField.Height - 100;
                    return p;
                case 2:
                    p.X = 100;
                    p.Y = GameField.Height - 100;
                    return p;
                case 3:
                    p.X = GameField.Width - 100;
                    p.Y = 100;
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

        public static void OnChangeGame()
        {
            IsRun = !CheckEndGame();

            //if (!IsRun)
            //{
            //    Restart();
            //}

        }


    }
}
