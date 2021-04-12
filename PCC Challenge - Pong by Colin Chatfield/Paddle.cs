using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace PCC_Pong
{
    public class Paddle
    {
        public PictureBox pbGame;
        public Color clr;
        //Score displayed at top of screen, 1/4 away from the edge of the picturebox
        public int Score;
        public int ScorePosition;
        //Speed of paddle in pixels per second
        public int Speed;
        //Width of paddle in pixels
        public int Width;
        //Height of paddle in pixels
        public int Height;
        //Coordinates of the top left of the paddle
        public double xPosition;
        public double yPosition;

        public Paddle(PictureBox picturebox, Color color, int width, int height, int speed)
        {
            pbGame = picturebox;
            clr = color;
            Score = 0;
            xPosition = 0;
            yPosition = 0;

            //Ensuring admissible parameters are passed
            if (speed <= 0)
                throw new Exception();
            else
                Speed = speed;

            if (width <= 0)
                throw new Exception();
            else
                Width = width;

            if (height <= 0)
                throw new Exception();
            else
                Height = height;
        }
        public void Reset()
        {
            yPosition = (pbGame.Height / 2) - (Height / 2);
        }
        public void MoveUp(TimeSpan time)
        {
            //Moves paddle up based on it's speed and the time since last update
            yPosition -= Speed * time.TotalSeconds;
            //Limits paddle to the upper boundary of the picturebox
            if (yPosition < 0)
                yPosition = 0;
        }
        public void MoveDown(TimeSpan time)
        {
            //Moves paddle down based on it's speed and the time since last update
            yPosition += Speed * time.TotalSeconds;
            //Limits paddle to the lower boundary of the picturebox
            if (yPosition > pbGame.Height - Height)
                yPosition = pbGame.Height - Height;
        }
        public void Draw(Graphics g)
        {
            //Draws paddle from position and size
            SolidBrush myBrush = new SolidBrush(Color.White);
            g.FillRectangle(myBrush, Convert.ToSingle(xPosition), Convert.ToSingle(yPosition), Width, Height);
        }
    }
}
