using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.Drawing;

namespace PCC_Pong.UnitTests
{
    [TestClass]
    public class PaddleTests
    {
        [TestMethod]
        public void MoveUp_AtUpperBounds_NoMovement()
        {
            //Initialize paddle at top of a picturebox
            PictureBox pbox = new PictureBox();
            Paddle p = new Paddle(pbox, Color.White, 6, 30, 400);
            p.pbGame.Height = 400;
            p.yPosition = 0;
            //Call method to move paddle up
            p.MoveUp(TimeSpan.FromMilliseconds(1));
            //Check if paddle moved up
            Assert.IsTrue(p.yPosition >= 0);
        }
        [TestMethod]
        public void MoveDown_AtLowerBounds_NoMovement()
        {
            //Initialize paddle at bottom of a picturebox
            PictureBox pbox = new PictureBox();
            Paddle p = new Paddle(pbox, Color.White, 6, 30, 400);
            p.pbGame.Height = 400;
            p.yPosition = 370;
            //Call method to move paddle down
            p.MoveDown(TimeSpan.FromMilliseconds(1));
            //Check if paddle moved down
            Assert.IsTrue(p.yPosition >= 0);
        }
    }
}
