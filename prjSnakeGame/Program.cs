using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections;

// email:   ariesbuenaventura2019@gmail.com

namespace prjSnakeGame
{
    class Program
    {
        const string msg1 = "   GAME OVER!!!   ";
        const string msg2 = " Press any key to exit. ";
        const string msg3 = " Congratulations!!! ";
        
        struct Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        static Point Direction    = new Point();
        static ArrayList Snake    = new ArrayList();
        static Boolean isGameExit = false;
        
        static Random GenerateRandom = new Random();
        static Point Food = new Point();
        static int Score = 0;

        static int WL; // Window-Left
        static int WR; // Window-Right
        static int WT; // Window-Top
        static int WB; // Window-Bottom

        static ConsoleKeyInfo kb;       // keyboard input
        static ConsoleKey     last_kb;  // save last key pressed
        
        static void Main(string[] args)
        {
            GameDesign();
            DisplayFood();

            // Set the default direction for the snake
            Direction.x = 1;
            Direction.y = 0;

            // Create a timer with a 0.3 second interval.  
            //   Note:
            //     1000ms = 1sec
            Timer GameTimer = new Timer(300);

            // Define the length and position of the snake.
            Snake.Add(new Point(4, 0)); // Head
            Snake.Add(new Point(3, 0)); // Body
            Snake.Add(new Point(2, 0)); // Body
            Snake.Add(new Point(1, 0)); // Body
            Snake.Add(new Point(0, 0)); // Tail
            
            // Set the default value for last key pressed
            last_kb = ConsoleKey.RightArrow;
           
            // Show the snake
            for (int i = 0; i < Snake.Count; i++)
            {
                Console.SetCursorPosition(((Point)Snake[i]).x + WL, ((Point)Snake[i]).y + WT);
                Console.Write("#");
            }
            
            // Hook up the Elapsed event for the timer.
            GameTimer.Elapsed += new ElapsedEventHandler(GameStart);
            // Start the game.
            GameTimer.Start();
            
            while (!isGameExit) // continue looping until escape key has been pressed.
            {
                if(Console.KeyAvailable)
                    kb = Console.ReadKey(true); // get key input
            }

            GameTimer.Stop();

            // Press any key to exit.
            Console.SetCursorPosition((Console.WindowWidth - msg2.Length) / 2,
                                       Console.WindowHeight/2);
            Console.Write(msg2);
            Console.ReadKey();
        }

        private static void GameStart(object sender, ElapsedEventArgs e)
        {
            if(!kb.Key.Equals(ConsoleKey.Escape))
            {
                //   Check the last key input. This prevents the snake to move backward.
                // For instance, when the user pressed the left or right arrow key, the next available keys
                // for the user are arrow up/down, and vice versa.
                if((last_kb.Equals(ConsoleKey.LeftArrow)) || (last_kb.Equals(ConsoleKey.RightArrow)))
                {
                    switch (kb.Key)
                    {
                        case ConsoleKey.UpArrow:
                            Direction.x = 0; // same direction
                            Direction.y = -1; // above.
                            break;
                        case ConsoleKey.DownArrow:
                            Direction.x = 0; // same direction
                            Direction.y = +1; // below.
                            break;
                    }
                }
                else if((last_kb.Equals(ConsoleKey.UpArrow)) || (last_kb.Equals(ConsoleKey.DownArrow)))
                {
                    switch (kb.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            Direction.x = -1; // left.
                            Direction.y = 0;  // same direction
                            break;
                        case ConsoleKey.RightArrow:
                            Direction.x = +1; // right.
                            Direction.y = 0;  // same direction
                            break;
                    }
                } 
            }
            else
            {
                isGameExit = true;
                return;
            }

            // Is key has been pressed?
            if(!Convert.ToInt32(kb.Key).Equals(0))
                if(kb.Key.Equals(ConsoleKey.LeftArrow) || kb.Key.Equals(ConsoleKey.RightArrow) ||
                   kb.Key.Equals(ConsoleKey.UpArrow) || kb.Key.Equals(ConsoleKey.DownArrow))
                        last_kb = kb.Key;
            
            // remove the trail
            Console.SetCursorPosition(((Point)Snake[Snake.Count-1]).x+WL, ((Point)Snake[Snake.Count-1]).y+WT);
            Console.Write(" ");

            for (int i=(Snake.Count-1); i>=0; i--)
            {
                if (i.Equals(0)) // Is this the head of the snake?
                    // Move the snake to its new direction.
                    Snake[0] = new Point(((Point)Snake[0]).x + Direction.x,
                                         ((Point)Snake[0]).y + Direction.y);
                else
                    // Track the body.
                    //
                    // [n].x <- [n-1].x
                    // [n].y <- [n-1].y
                    // 
                    // example:
                    //     [4].x = [3].x : [4].y = [3].y
                    //     [3].x = [2].x : [3].y = [2].y
                    //     [2].x = [1].x : [2].y = [1].y
                    //     [1].x = [0].x : [1].y = [0].y
                    Snake[i] = new Point(((Point)Snake[i-1]).x, 
                                         ((Point)Snake[i-1]).y);
            }

            if (!IsCollided())
            {
                if (((((Point)Snake[0]).x + WL).Equals(Food.x)) && ((((Point)Snake[0]).y + WT).Equals(Food.y)))
                {
                    // Is the length of the snake reached 100?
                    if(Snake.Count.Equals(100))
                    {
                        isGameExit = true;
                        // yes, let's end the game
                        // Congratulations!!!
                        Console.SetCursorPosition((Console.WindowWidth - msg3.Length)/2,
                                                   Console.WindowHeight/2-1);
                        Console.Write(msg3); 
                    }
                    else
                    {
                        // no, continue playing
                        // increase the length of the snake by 1
                        Snake.Add(new Point(((Point)Snake[Snake.Count - 1]).x, 
                                            ((Point)Snake[Snake.Count - 1]).y));
                        Score += 10;
                        DisplayScore();
                        DisplayFood();
                    }
                }

                for (int i = 0; i < Snake.Count; i++)
                {
                    Console.SetCursorPosition(((Point)Snake[i]).x + WL, ((Point)Snake[i]).y + WT);
                    Console.Write("#");
                }
            }
            else
            {
                // Game Over!!!
                Console.SetCursorPosition((Console.WindowWidth - msg1.Length)/2,
                                           Console.WindowHeight/2-1);
                Console.Write(msg1);
                isGameExit = true;
            }
        }

        private static void GameDesign()
        {
            const string dsgnTB = "▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒";
            const string dsgnLR = "▒                        ▒";
            
            // Define window size.
            WL = (Console.WindowWidth-dsgnTB.Length )/2+1;
            WR = WL+dsgnTB.Length-3;
            WT = 3;
            WB = Console.WindowHeight - 4;

            // hide the cursor
            Console.CursorVisible = false;
            // draw the top border
            Console.SetCursorPosition((Console.WindowWidth-dsgnTB.Length )/2, 2);
            Console.Write(dsgnTB);
            // draw the bottom border
            Console.SetCursorPosition((Console.WindowWidth - dsgnTB.Length) / 2, 
                                       Console.WindowHeight - 3);
            Console.Write(dsgnTB);
            
            // draw the left and right border
            for(int i=3; i<Console.WindowHeight -3; i++)
            {
                Console.SetCursorPosition((Console.WindowWidth - dsgnTB.Length) / 2, i);
                Console.Write(dsgnLR);
            }

            DisplayScore();
        }

        private static void DisplayFood()
        {
            // Creates a new food.
            Food.x = GenerateRandom.Next(WL, WR);
            Food.y = GenerateRandom.Next(WT, WB);

            Console.SetCursorPosition(Food.x, Food.y);
            Console.Write("@");
        }

        private static void DisplayScore()
        {
            Console.SetCursorPosition(WR + 3, Console.WindowHeight / 2);
            Console.Write("SCORE : {0}", Score);
        }

        private static Boolean IsCollided()
        {
            int x = ((Point)Snake[0]).x + WL; // head_x + window_left
            int y = ((Point)Snake[0]).y + WT; // head_y + window_top

            // Is out of boundary?
            if ((x < WL) || (x > WR) || (y < WT) || (y > WB))
                return true; 
            else
                for(int i=1; i<=Snake.Count-1; i++)
                    // Is the head of the snake collided to the body?
                    if((x.Equals((((Point)Snake[i]).x + WL)) && (y.Equals((((Point)Snake[i]).y + WT)))))
                        return true;

            return false;
        }
    }
}
