using System;
using System.Drawing;

using System.Windows.Forms;

namespace GameModelsLib
{
    public class Bullet : AutoMovedSprite
    {
        int LifeSteps = 35;

        protected static Bitmap image = null;

        Hunter Creator;
        public Bullet(Hunter creator, Point p) : base(p)
        {
            Game.GameField.Invoke((MethodInvoker)delegate ()
            {
                Creator = creator;
                timer.Interval = 20;
                Width = 20;
                Height = Width;
                ImageFolder = "bullet";
                Step = 20;
            });

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Run();
        }

        public override void ApplySkin()
        {
            if (image == null)
            {
                image = new Bitmap(Environment.CurrentDirectory + "\\images\\bullet\\bullet.png");

            }
            BackgroundImage = image;
            Invalidate();
        }

        public override void Run()
        {
            LifeSteps--;
            if (IsAbroad || LifeSteps <= 0)
            {
                Die();
                return;
            }
            base.Run();

        }

        public override void CheckEnvironment()
        {

            Hunter killedHunter = null;
            foreach (Hunter hunter in Game.Hunters)
            {
                if (hunter != Creator)
                {
                    if (this.IsIntersection(hunter))
                    {
                        killedHunter = hunter;
                        break;
                    }
                }
            }

            if (killedHunter != null)
            {
                killedHunter.Die();
                if (Creator != null)
                    Creator.HitCount++;

                this.Die();
            }
        }


    }
}
