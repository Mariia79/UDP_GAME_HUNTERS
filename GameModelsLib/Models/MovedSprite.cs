using System.Drawing;

namespace GameModelsLib
{

    public class MovedSprite : Sprite
    {
        public int Step = 8;
        public int ForcedStep = 16;
        public int SpeedForcedLifeTick = 0;
        public bool IsAbroad
        {
            get
            {
                return Left < 0 || Left + Width > Game.GameField.Width
                || Top < 0 || Top + Height > Game.GameField.Height;
            }
        }
        public MovedSprite(Point p) : base(p)
        {

        }

        public virtual void CheckEnvironment()
        {

        }

        public virtual void Run()
        {
            int localStep = Step;
            if (SpeedForcedLifeTick > 0)
            {
                localStep = ForcedStep;
                SpeedForcedLifeTick--;
            }
            switch (Direction)
            {
                case MoveDirection.Left:
                    Left = Left - localStep;
                    break;
                case MoveDirection.Right:
                    Left = Left + localStep;
                    break;
                case MoveDirection.Up:
                    Top = Top - localStep;
                    break;
                case MoveDirection.Down:
                    Top = Top + localStep;
                    break;
                case MoveDirection.None:
                    break;
            }
        }
    }
}
