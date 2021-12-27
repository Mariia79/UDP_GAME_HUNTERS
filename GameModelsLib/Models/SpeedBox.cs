﻿using System;
using System.Drawing;


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

                if (this.IsIntersection(hunter))
                {
                    foundedHunter = hunter;
                    break;
                }
            }

            if (foundedHunter != null)
            {
                foundedHunter.SpeedForcedLifeTick += 690;
                foundedHunter.ShowInfo();
                this.Die();

            }
        }


    }
}