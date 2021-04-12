using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace PCC_Pong
{
    public partial class GameForm : Form
    {
        Player1 p1;
        Player2 p2;
        Ball b1;
        Stopwatch UpdateTimer;
        TimeSpan TimeSinceLastUpdate, TotalRuntime, Cooldown, WinCooldown;
        bool W, S, Up, Down, GameRunning, isCooldown, isWinCooldown1, isWinCooldown2, SinglePlayer;

        public GameForm()
        {
            InitializeComponent();
            //Initializing classes
            p1 = new Player1(pbGame, Color.White, 6, 30, 400);
            p2 = new Player2(pbGame, Color.White, 6, 30, 400);
            b1 = new Ball(pbGame, Color.White, 12, 200);
            UpdateTimer = new Stopwatch();
            TimeSinceLastUpdate = new TimeSpan();
            TotalRuntime = new TimeSpan();
            //Initializing game to an idle state
            GameRunning = false;
            //Draws all objects
            pbGame.Invalidate();
        }
        private void PbGame_Paint(object sender, PaintEventArgs e)
        {
            //This event handler is used as the game loop, calling itself at the bottom

            //If the game was previously running, get time passed since last cycle to calculate movement
            if (UpdateTimer.IsRunning == true)
            {
                TimeSinceLastUpdate = UpdateTimer.Elapsed;
                TotalRuntime += UpdateTimer.Elapsed;
                UpdateTimer.Reset();
            }

            //Start timer for next cycle
            UpdateTimer.Start();

            //Calls method to update all objects
            UpdatePosition(TimeSinceLastUpdate);

            //Draws gameboard, game objects, and win condition graphics
            Graphics g = e.Graphics;
            //Pen for dashed center line
            Pen myPen = new Pen(Color.White, 4);
            float[] dash = { 2, 2, 2, 2 };
            myPen.DashPattern = dash;
            myPen.DashOffset = 10;
            //Brush and rectangle for score display
            SolidBrush myBrush = new SolidBrush(Color.White);
            Font myFont = new Font("Lucida Console", pbGame.Height / 15);
            Rectangle textRect = new Rectangle(0, pbGame.Height / 2 - (myFont.Height / 2), pbGame.Width, pbGame.Height / 4);
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            //If game is not running then only draw title
            if (GameRunning == false)
            {
                DrawTitle(g);
            }
            //If a player has enough points, reset score and start win cooldown
            else if (p1.Score == 11)
            {
                p1.Score = 0;
                p2.Score = 0;
                isWinCooldown1 = true;
                WinCooldown = TotalRuntime + TimeSpan.FromSeconds(3);
            }
            else if (p2.Score == 11)
            {
                p1.Score = 0;
                p2.Score = 0;
                isWinCooldown2 = true;
                WinCooldown = TotalRuntime + TimeSpan.FromSeconds(3);
            }
            //If the game running and is not currently on win cooldown, draw objects
            if (GameRunning == true && isWinCooldown1 == false && isWinCooldown2 == false)
            {
                g.DrawLine(myPen, pbGame.Width / 2, 0, pbGame.Width / 2, pbGame.Height);
                p1.Draw(g);
                p1.DrawScore(g);
                p2.Draw(g);
                p2.DrawScore(g);
                b1.Draw(g);
            }
            //If win cooldown was active and if cooldown has passed, end cooldown, stop game, and return to title screen
            else if (TotalRuntime >= WinCooldown && (isWinCooldown1 == true || isWinCooldown2 == true))
            {
                isWinCooldown1 = false;
                isWinCooldown2 = false;
                GameRunning = false;
                p1.Reset();
                p2.Reset();
                b1.ResetCenter();
                DrawTitle(g);
            }
            //If currently on win cooldown, draw win message
            else if (isWinCooldown1 == true)
            {
                if (SinglePlayer == true)
                    g.DrawString("Computer Wins!", myFont, myBrush, textRect, strFormat);
                else
                    g.DrawString("Player 1 Wins!", myFont, myBrush, textRect, strFormat);
            }
            else if (isWinCooldown2 == true)
            {
                if (SinglePlayer == true)
                    g.DrawString("You Win!", myFont, myBrush, textRect, strFormat);
                else
                    g.DrawString("Player 2 Wins!", myFont, myBrush, textRect, strFormat);
            }

            //While game is running, call this event handler again
            if (GameRunning == true)
                pbGame.Invalidate();
            else
                UpdateTimer.Stop();
        }
        public void DrawTitle(Graphics g)
        {
            //Draws all title text in different rectangles, in relation to the size of the picturebox
            SolidBrush myBrush = new SolidBrush(Color.White);
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;

            Font titleFont = new Font("Lucida Console", pbGame.Height / 10);
            Rectangle titleRect = new Rectangle(0, pbGame.Height / 8, pbGame.Width, pbGame.Height);
            g.DrawString("Pong", titleFont, myBrush, titleRect, strFormat);

            int spacer = Convert.ToInt32(titleFont.GetHeight());

            Font nameFont = new Font("Lucida Console", pbGame.Height / 25);
            Rectangle nameRect = new Rectangle(0, spacer + (pbGame.Height / 8), pbGame.Width, pbGame.Height);
            g.DrawString("by Colin Chatfield", nameFont, myBrush, nameRect, strFormat);

            Font mode1Font = new Font("Lucida Console", pbGame.Height / 20);
            Rectangle mode1Rect = new Rectangle(0, (2 * spacer) + (pbGame.Height / 8), pbGame.Width, pbGame.Height);
            g.DrawString("Press [ENTER] to start 2 player game", mode1Font, myBrush, mode1Rect, strFormat);

            Font mode2Font = new Font("Lucida Console", pbGame.Height / 20);
            Rectangle mode2Rect = new Rectangle(0, (3 * spacer) + (pbGame.Height / 8), pbGame.Width, pbGame.Height);
            g.DrawString("Press [SPACE] to start 1 player game", mode2Font, myBrush, mode2Rect, strFormat);

            Font controlFont = new Font("Lucida Console", pbGame.Height / 30);
            Rectangle controlRect = new Rectangle(0, (5 * spacer) + (pbGame.Height / 8), pbGame.Width, pbGame.Height);
            g.DrawString("Controls: W and S keys control left side - Up and Down arrow keys control right side - Use arrow keys for single player mode.", controlFont, myBrush, controlRect, strFormat);
        }
        public void UpdatePosition(TimeSpan time)
        {
            //If game is not paused, move ball
            if (isCooldown == false && isWinCooldown1 == false && isWinCooldown2 == false)
                b1.Move(time, p1, p2);
            //If ball is out of horizontal bounds then add one score to opposite side, start cooldown, and reset ball
            if (b1.xPosition < 0 - b1.Size)
            {
                p2.Score += 1;
                isCooldown = true;
                Cooldown = TotalRuntime + TimeSpan.FromSeconds(1);
                b1.ResetRandom();
            }
            else if (b1.xPosition > pbGame.Width)
            {
                p1.Score += 1;
                isCooldown = true;
                Cooldown = TotalRuntime + TimeSpan.FromSeconds(1);
                b1.ResetRandom();
            }
            //If cooldown time has passed, unpause game
            else if (TotalRuntime >= Cooldown)
            {
                isCooldown = false;
            }
            //If any of the movement controls are active, move paddles in the desired direction
            if (SinglePlayer == false)
            {
                if (W == true)
                    p1.MoveUp(time);
                if (S == true)
                    p1.MoveDown(time);
            }
            else
            {
                p1.AutoMove(b1, time);
            }

            if (Up == true)
                p2.MoveUp(time);
            if (Down == true)
                p2.MoveDown(time);
        }
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            string key;
            key = e.KeyCode.ToString();
            //Move up/down in pbGame_Paint while key is pressed
            if (SinglePlayer == false)
            {
                switch (key)
                {
                    case "W":
                        W = true;
                        break;
                    case "S":
                        S = true;
                        break;
                    case "Up":
                        Up = true;
                        break;
                    case "Down":
                        Down = true;
                        break;
                }
            }
            else
            {
                switch (key)
                {

                    case "Up":
                        Up = true;
                        break;
                    case "Down":
                        Down = true;
                        break;
                }
            }

            //Start a 2 player game when user presses Enter
            if (key == Keys.Enter.ToString())
            {
                if (GameRunning == false)
                {
                    SinglePlayer = false;
                    GameRunning = true;
                    isCooldown = true;
                    Cooldown = TotalRuntime + TimeSpan.FromSeconds(1);
                    p1.Speed = 400;
                    b1.ResetCenter();
                    pbGame.Invalidate();
                }
            }
            //Start a 1 player game when user presses Space
            if (key == Keys.Space.ToString())
            {
                if (GameRunning == false)
                {
                    SinglePlayer = true;
                    GameRunning = true;
                    isCooldown = true;
                    Cooldown = TotalRuntime + TimeSpan.FromSeconds(1);
                    b1.ResetCenter();
                    pbGame.Invalidate();
                }
            }
        }
        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            string key;
            key = e.KeyCode.ToString();
            //Stop moving up/down in pbGame_Paint when key is no longer pressed
            if (SinglePlayer == false)
            {
                switch (key)
                {
                    case "W":
                        W = false;
                        break;
                    case "S":
                        S = false;
                        break;
                    case "Up":
                        Up = false;
                        break;
                    case "Down":
                        Down = false;
                        break;
                }
            }
            else
            {
                switch (key)
                {
                    case "Up":
                        Up = false;
                        break;
                    case "Down":
                        Down = false;
                        break;
                }
            }
        }
    }
}
