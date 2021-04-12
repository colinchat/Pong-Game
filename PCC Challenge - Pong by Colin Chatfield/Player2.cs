using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace PCC_Pong
{
    public class Player2 : Paddle
    {
        public Player2(PictureBox picturebox, Color color, int width, int height, int speed) : base(picturebox, color, width, height, speed)
        {
            //Position set to right side of picturebox
            yPosition = (pbGame.Height / 2) - (height / 2);
            xPosition = (pbGame.Width * 7 / 8) - width;
            //Draws score 3/4 accross court (minus the approximate width of the text)
            ScorePosition = (pbGame.Width * 3 / 4) - 60;
        }
        public void DrawScore(Graphics g)
        {
            SolidBrush myBrush = new SolidBrush(Color.White);
            //Create a rectangle in which to draw the scores, player 2's will be to the right (far)
            Rectangle textRect = new Rectangle(pbGame.Width / 4, pbGame.Height / 16, pbGame.Width / 2, pbGame.Height / 2);
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Far;

            using (Font myFont = new Font("Lucida Console", pbGame.Height / 10))
            {
                g.DrawString(Score.ToString(), myFont, myBrush, textRect, strFormat);
            }
        }
    }
}
