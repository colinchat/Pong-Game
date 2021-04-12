using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace PCC_Pong
{
    public class Player1 : Paddle
    {
        Random rMove;
        public Player1(PictureBox picturebox, Color color, int width, int height, int speed) : base(picturebox, color, width, height, speed)
        {
            //Position set to left side of picturebox
            yPosition = (pbGame.Height / 2) - (height / 2);
            xPosition = pbGame.Width / 8;
            //Draws score 1/4 accross screen
            ScorePosition = pbGame.Width / 4;
            rMove = new Random();
        }
        public void DrawScore(Graphics g)
        {
            SolidBrush myBrush = new SolidBrush(Color.White);
            //Create a rectangle in which to draw the scores, player 1's will be to the left (near)
            Rectangle textRect = new Rectangle(pbGame.Width / 4, pbGame.Height / 16, pbGame.Width / 2, pbGame.Height / 2);
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Near;

            using (Font myFont = new Font("Lucida Console", pbGame.Height / 10))
            {
                g.DrawString(Score.ToString(), myFont, myBrush, textRect, strFormat);
            }
        }
        public void AutoMove(Ball b1, TimeSpan time)
        {
            if (Math.Sign(b1.xVelocity) == -1 && b1.xPosition < pbGame.Width / 2 && b1.xPosition > xPosition)
            {
                //When ball is travelling towards player 1 and is on the side of player 1, move up or down based on its position
                //Paddle always attempts to hit the ball 1/3 of the way from the top of the paddle
                if (yPosition + (Height / 3) > b1.yPosition + (b1.Size / 2))
                {
                    if (Math.Sign(b1.yVelocity) == -1)
                    {
                        //If ball is travelling away from paddle, go faster
                        Speed = 300;
                    }
                    MoveUp(time);
                    Speed = 200;
                }
                else
                {
                    if (Math.Sign(b1.yVelocity) == 1)
                    {
                        //If ball is travelling away from paddle, go faster
                        Speed = 300;
                    }
                    MoveDown(time);
                    Speed = 200;
                }
            }
        }
    }
}
