using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace PCC_Pong
{
    public class Ball
    {
        public PictureBox pbGame;
        public Color clr;
        //Size of one side length of the square 'ball' in pixels
        public int Size;
        //Components of the velocity, x stays constant and y is calculated based on the direction
        public double xVelocity;
        public double yVelocity;
        //Position of the top left corner of the rectangle which represents the ball
        public double xPosition;
        public double yPosition;
        //RNG to decide start position and velocity
        public Random StartRandom;

        public Ball(PictureBox picturebox, Color color, int size, int speed)
        {
            pbGame = picturebox;
            clr = color;

            //Ensuring admissible parameters are passed
            if (size <= 0)
                throw new Exception();
            else
                Size = size;
            if (speed <= 0)
                throw new Exception();
            else
                xVelocity = speed;
            //yVelocity to be randomized upon ball start
            yVelocity = 0;
            //Initialized to the middle of the court
            xPosition = (pbGame.Width / 2) - (Size / 2);
            yPosition = (pbGame.Height / 2) - (Size / 2);

            StartRandom = new Random();
        }
        public void ResetCenter()
        {
            //Serve ball from center 

            //xVelocity can either be negative or positive (left or right), will return xVelocity * 1 or xVelocity * -1
            xVelocity = xVelocity * ((StartRandom.Next(0, 2) * 2) - 1);
            //yVelocity is calculated randomly based on 3 starting angles 
            int ran = StartRandom.Next(0, 3);
            if (ran == 0)
                yVelocity = Math.Abs(xVelocity) * Math.Tan(20 * Math.PI / 180);
            else if (ran == 1)
                yVelocity = 0;
            else if (ran == 2)
                yVelocity = Math.Abs(xVelocity) * Math.Tan(-20 * Math.PI / 180);
            //Ball position is calculated randomly along center line
            xPosition = (pbGame.Width / 2) - (Size / 2);
            yPosition = (pbGame.Height / 2) - (Size / 2);
        }
        public void ResetRandom()
        {
            //Serve ball from randomized location on center line

            //xVelocity can either be negative or positive (left or right), will return xVelocity * 1 or xVelocity * -1
            xVelocity = xVelocity * ((StartRandom.Next(0, 2) * 2) - 1);
            //yVelocity is calculated randomly based on 3 starting angles 
            int ran = StartRandom.Next(0, 3);
            if (ran == 0)
                yVelocity = Math.Abs(xVelocity) * Math.Tan(20 * Math.PI / 180);
            else if (ran == 1)
                yVelocity = 0;
            else if (ran == 2)
                yVelocity = Math.Abs(xVelocity) * Math.Tan(-20 * Math.PI / 180);
            //Ball position is calculated randomly along center line
            xPosition = (pbGame.Width / 2) - (Size / 2);
            yPosition = StartRandom.Next(0, pbGame.Height - Size);
        }
        public void Move(TimeSpan time, Paddle p1, Paddle p2)
        {
            if (Math.Sign(yVelocity) == -1 && yPosition <= 0)
            {
                //If ball is travelling upward and is out of bounds, bounce off top edge
                yVelocity = yVelocity * -1;
            }
            else if (Math.Sign(yVelocity) == 1 && yPosition >= pbGame.Height - Size)
            {
                //If ball is travelling downwards and is out of bounds, bounce off bottom edge
                yVelocity = yVelocity * -1;
            }
            else if (Math.Sign(xVelocity) == -1 && xPosition <= p1.xPosition + p1.Width && xPosition >= p1.xPosition - Size)
            {
                //If ball is going left and is crossing the xPosition of the paddle, check if ball will bounce off paddle 1

                //Calculate distance from center of ball to center of paddle
                double DistanceToPaddle = Math.Abs((yPosition + (Size / 2)) - (p1.yPosition + (p1.Height / 2)));
                //Calculate whether the value is positive or negative to decide if ball will bounce up or down
                int Sign = Math.Sign((yPosition + (Size / 2)) - (p1.yPosition + (p1.Height / 2)));
                //Divide paddle into 5 equal segments which will reflect different angles
                double PaddleSegment = p1.Height / 5;

                if (DistanceToPaddle <= (2.5 * PaddleSegment) + (Size / 2) && DistanceToPaddle > 1.5 * PaddleSegment)
                {
                    //Bounce off the edge of the paddle, at an angle of 60 degrees up or down from horizontal
                    xVelocity = xVelocity * -1;
                    yVelocity = Math.Abs(xVelocity) * Math.Tan(60 * Math.PI / 180) * Sign;
                    //Play audio
                    Stream str = Properties.Resources.pong_beep;
                    System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                    snd.Play();
                }
                else if (DistanceToPaddle <= 1.5 * PaddleSegment && DistanceToPaddle > 0.5 * PaddleSegment)
                {
                    //Bounce off the upper middle or lower middle of the paddle, at an angle of 20 degrees up or down from horizontal
                    xVelocity = xVelocity * -1;
                    yVelocity = Math.Abs(xVelocity) * Math.Tan(20 * Math.PI / 180) * Sign;
                    //Play audio
                    Stream str = Properties.Resources.pong_boop;
                    System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                    snd.Play();
                }
                else if (DistanceToPaddle <= 0.5 * PaddleSegment && DistanceToPaddle >= 0)
                {
                    //Bounce off the middle of the paddle, at an angle of 0 degrees from horizontal
                    xVelocity = xVelocity * -1;
                    yVelocity = 0;
                    //Play audio
                    Stream str = Properties.Resources.pong_boop;
                    System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                    snd.Play();
                }
            }
            else if (Math.Sign(xVelocity) == 1 && xPosition >= p2.xPosition - Size && xPosition <= p2.xPosition + p2.Width)
            {
                //If ball is going right and it is crossing the xPosition of the paddle, check if ball will bounce off paddle 2

                //Calculate distance from center of ball to center of paddle
                double DistanceToPaddle = Math.Abs((yPosition + (Size / 2)) - (p2.yPosition + (p2.Height / 2)));
                //Calculate whether the value is positive or negative to decide if ball will bounce up or down
                int Sign = Math.Sign((yPosition + (Size / 2)) - (p2.yPosition + (p2.Height / 2)));
                //Divide paddle into 5 equal segments which will reflect different angles
                double PaddleSegment = p2.Height / 5;

                if (DistanceToPaddle <= (2.5 * PaddleSegment) + (Size / 2) && DistanceToPaddle > 1.5 * PaddleSegment)
                {
                    //Bounce off the edge of the paddle, at an angle of 60 degrees up or down from horizontal
                    xVelocity = xVelocity * -1;
                    yVelocity = Math.Abs(xVelocity) * Math.Tan(60 * Math.PI / 180) * Sign;
                    //Play audio
                    Stream str = Properties.Resources.pong_beep;
                    System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                    snd.Play();
                }
                else if (DistanceToPaddle <= 1.5 * PaddleSegment && DistanceToPaddle > 0.5 * PaddleSegment)
                {
                    //Bounce off the upper middle or lower middle of the paddle, at an angle of 20 degrees up or down from horizontal
                    xVelocity = xVelocity * -1;
                    yVelocity = Math.Abs(xVelocity) * Math.Tan(20 * Math.PI / 180) * Sign;
                    //Play audio
                    Stream str = Properties.Resources.pong_boop;
                    System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                    snd.Play();
                }
                else if (DistanceToPaddle <= 0.5 * PaddleSegment && DistanceToPaddle >= 0)
                {
                    //Bounce off the middle of the paddle, at an angle of 0 degrees from horizontal
                    xVelocity = xVelocity * -1;
                    yVelocity = 0;
                    //Play audio
                    Stream str = Properties.Resources.pong_boop;
                    System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                    snd.Play();
                }
            }
            //Change position based on velocity and time passed
            xPosition += xVelocity * time.TotalSeconds;
            yPosition += yVelocity * time.TotalSeconds;
        }
        public void Draw(Graphics g)
        {
            //Draws ball as a square with position and size
            SolidBrush myBrush = new SolidBrush(clr);
            g.FillRectangle(myBrush, Convert.ToSingle(xPosition), Convert.ToSingle(yPosition), Size, Size);
        }
    }
}
