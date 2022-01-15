using System;
using System.Drawing;
using System.Numerics;

namespace GameModelsLib
{
    public class SpeedBox : AutoMovedSprite
    {
        System.Media.SoundPlayer player;
        protected static Bitmap image = null;
        public int Ammo = 100;

        public SpeedBox(Point p) : base(p)
        {
            Random random = new Random();
            Ammo = random.Next(1, 4) * 5;
            Width = 50;
            Height = 40;
            timer.Start();

        }

        public override void ApplySkin()
        {
            if (image == null)
            {
                image = new Bitmap(Environment.CurrentDirectory + "\\images\\speed\\speed.png");

            }
            BackgroundImage = image;
            Invalidate();
        }

        public override void CheckEnvironment()
        {

            Hunter foundedHunter = null;
            foreach (Hunter hunter in Game.Hunters)
            {

                if (Vector2.Distance(new Vector2(this.Center.X, this.Center.Y), new Vector2(hunter.Center.X, hunter.Center.Y)) < 50)
                {
                    foundedHunter = hunter;
                    break;
                }
            }

            if (foundedHunter != null)
            {
                foundedHunter.SpeedForcedLifeTick += 100;
                foundedHunter.ShowInfo();
                this.Die();

            }
        }


    }
}
