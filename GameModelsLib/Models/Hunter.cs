using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameModelsLib
{

    public class Hunter : AutoMovedSprite
    {
        public int Number = 0;
        System.Media.SoundPlayer player;
        public int Ammo = 60;
        public int HitCount = 0;
        public int LifeScore = 100;

        public string HunterName;

        public Label LabelName;
        public bool IsCanMove = false;
        private Point point;

        
        public string ActionsState
        {
            get;
            set;
        }

        public string HealthAmmoState
        {
            get;
            set;
        }

        public string StateStartPos
        {
            get { return "40,40,1"; }
        }

        public Hunter(string name, Color color, Point p) : base(p)
        {
            Width = 50;
            Height = 50;
            Name = name;
            ImageFolder = "hunter";
            Direction = MoveDirection.Up;
            LabelName = new Label();


            LabelName.Top = this.Top + this.Height;
            LabelName.Left = this.Left + 5;

            LabelName.Width = this.Width;
            LabelName.Height = (int)LabelName.Font.Size * 6;
            LabelName.ForeColor = color;
            Game.GameField.Controls.Add(LabelName);

           

            player = new System.Media.SoundPlayer(Environment.CurrentDirectory + "\\sounds\\shoot.wav");
            ShowInfo();
        }


        public void ShowInfo()
        {
            LabelName.Invoke((MethodInvoker)delegate ()
            {
                LabelName.Text = Name + " " + LifeScore.ToString() + " " + Ammo.ToString();
            });
        }

        public override void Run()
        {
            if (LifeScore <= 0)
            {
                Die();
                return;
            }

            if (IsAbroad)
                ChangeDirection();

            if (IsCanMove)
            {
                ApplySkin();
                base.Run();
                LabelName.Top = this.Top + this.Height;
                LabelName.Left = this.Left;
            }

        }



        public override void Die()
        {
            if (LifeScore > 0)
            {
                LifeScore -= 20;
                ShowInfo();
            }
            else
            {
                Game.GameField.Controls.Remove(this);
                Game.GameField.Controls.Remove(LabelName);

                base.Die();
                Game.OnChangeGame();
            }
        }


        public virtual void ChangeDirection()
        {
            switch (Direction)
            {
                case MoveDirection.Down:
                    Direction = MoveDirection.Up;
                    Top = Game.GameField.Height - Height - 1;
                    break;
                case MoveDirection.Up:
                    Direction = MoveDirection.Down;
                    Top = 1;
                    break;

                case MoveDirection.Left:
                    Direction = MoveDirection.Right;
                    Left = 1;
                    break;
                case MoveDirection.Right:
                    Direction = MoveDirection.Left;
                    Top = Game.GameField.Width - Width - 1;
                    break;
            }

            ApplySkin();
        }

        public void MoveHunter(char KeyChar)
        {
            if (KeyChar == 'w')
            {
                Direction = MoveDirection.Up;
                IsCanMove = true;

            }
            if (KeyChar == 'a')
            {
                Direction = MoveDirection.Left;
                IsCanMove = true;

            }
            if (KeyChar == 'd')
            {
                Direction = MoveDirection.Right;
                IsCanMove = true;

            }
            if (KeyChar == 's')
            {
                Direction = MoveDirection.Down;
                IsCanMove = true;

            }
            if (KeyChar == 'b')
            {
                Shoot();
            }

            if (KeyChar == 'x')
            {
                IsCanMove = false;
            }
            

        }


        public void MoveHunterByPressKey(KeyPressEventArgs e)
        {
            if (Game.IsRun)
            {
                ActionsState = Number.ToString() + e.KeyChar.ToString();
                //Game.SendHunterActionState(this);
                MoveHunter(e.KeyChar);
            }
        }

        public void Shoot()
        {
            Game.GameField.Invoke((MethodInvoker)delegate ()
            {

                if (Ammo > 0)
                {
                    Ammo--;

                    //HealthAmmoState = Number.ToString() + e.KeyChar.ToString();
                    //Game.SendHunterHealthAmmoState(this);

                    ShowInfo();

                    player.Play();

                    Point p = new Point();
                    p.X = Left + Width / 2;
                    p.Y = Top + Height / 2;
                    Bullet Bullet = new Bullet(this, p);
                    Bullet.ApplySkin();
                    Bullet.Direction = this.Direction;
                    Bullet.Run();
                }
            });
        }

    }
}
