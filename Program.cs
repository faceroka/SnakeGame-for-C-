using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        static int Width = 40;
        static int Height = 20;
        static List<Point> snake = new List<Point>();
        static Point food;
        static Random rand = new Random();
        static char dir = 'R';
        static char nextDir = 'R';
        static int score = 0;
        static bool gameOver = false;

        struct Point
        {
            public int X, Y;
            public Point(int x, int y) { X = x; Y = y; }
        }

        static void Main()
        {
            Console.CursorVisible = false;
            if (OperatingSystem.IsWindows())
                Console.SetWindowSize(Width + 2, Height + 4);

            do
            {
                Init();
                GameLoop();
                Console.Clear();
                Console.SetCursorPosition(Width / 2 - 10, Height / 2);
                Console.WriteLine("GAME OVER! Счёт: {0}", score);
                Console.SetCursorPosition(Width / 2 - 12, Height / 2 + 2);
                Console.Write("Нажмите любую клавишу...");
                Console.ReadKey(true);
            } while (true);
        }

        static void Init()
        {
            snake.Clear();
            int cx = Width / 2, cy = Height / 2;
            snake.Add(new Point(cx, cy));
            snake.Add(new Point(cx - 1, cy));
            snake.Add(new Point(cx - 2, cy));
            dir = 'R';
            nextDir = 'R';
            score = 0;
            gameOver = false;
            SpawnFood();
        }

        static void SpawnFood()
        {
            do
            {
                food = new Point(rand.Next(1, Width - 1), rand.Next(1, Height - 1));
            } while (snake.Any(p => p.X == food.X && p.Y == food.Y));
        }

        static void GameLoop()
        {
            var lastTick = DateTime.Now;
            int interval = 150;

            while (!gameOver)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            if (dir != 'D') nextDir = 'U';
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            if (dir != 'U') nextDir = 'D';
                            break;
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            if (dir != 'R') nextDir = 'L';
                            break;
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            if (dir != 'L') nextDir = 'R';
                            break;
                    }
                }

                var now = DateTime.Now;
                if ((now - lastTick).TotalMilliseconds < interval)
                {
                    Thread.Sleep(10);
                    continue;
                }
                lastTick = now;

                dir = nextDir;
                Move();
                Draw();

                if (score > 0 && score % 5 == 0)
                    interval = Math.Max(60, interval - 10);
            }
        }

        static void Move()
        {
            var head = snake[0];
            Point newHead = head;

            switch (dir)
            {
                case 'U': newHead = new Point(head.X, head.Y - 1); break;
                case 'D': newHead = new Point(head.X, head.Y + 1); break;
                case 'L': newHead = new Point(head.X - 1, head.Y); break;
                case 'R': newHead = new Point(head.X + 1, head.Y); break;
            }

            if (newHead.X < 1 || newHead.X >= Width - 1 ||
                newHead.Y < 1 || newHead.Y >= Height - 1)
            {
                gameOver = true;
                return;
            }

            if (snake.Any(p => p.X == newHead.X && p.Y == newHead.Y))
            {
                gameOver = true;
                return;
            }

            snake.Insert(0, newHead);

            if (newHead.X == food.X && newHead.Y == food.Y)
            {
                score++;
                SpawnFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        static void Draw()
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                        Console.Write("█");
                    else if (snake[0].X == x && snake[0].Y == y)
                        Console.Write("@");
                    else if (snake.Any(p => p.X == x && p.Y == y))
                        Console.Write("o");
                    else if (food.X == x && food.Y == y)
                        Console.Write("*");
                    else
                        Console.Write(" ");
                }
                Console.Write("\r\n");
            }
            Console.Write($"Счёт: {score}\r\n");
        }
    }
}   