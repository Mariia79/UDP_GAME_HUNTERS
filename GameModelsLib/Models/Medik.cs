using System;
using System.Drawing;


namespace GameModelsLib
{
    public class Medik : AutoMovedSprite
    {
        System.Media.SoundPlayer player;
        protected static Bitmap image = null;
        public int LifeScore = 100;

        public Medik(Point p) : base(p)
        {
            Random random = new Random();
            LifeScore = random.Next(1, 4) * 10;
            Width = 40;
            Height = 30;


        }

        public override void ApplySkin()
        {
            if (image == null)
            {
                image = new Bitmap(Environment.CurrentDirectory + "\\images\\medik\\medik.png");

            }
            BackgroundImage = image;
            Invalidate();
        }

        public override void CheckEnvironment()
        {

            Hunter foundedHunter = null;
            foreach (Hunter hunter in Game.Hunters)
            {

                if (this.IsIntersection(hunter))
                {
                    foundedHunter = hunter;
                    break;
                }
            }

            if (foundedHunter != null)
            {
                foundedHunter.LifeScore += this.LifeScore;
                foundedHunter.ShowInfo();
                this.Die();

            }
        }


    }
}
