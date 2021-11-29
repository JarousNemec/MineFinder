using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    public class gameData
    {
        private static int mapSizeX = 50;
        private static int mapSizeY = mapSizeX;
        public cell[,] map = new cell[mapSizeX, mapSizeY];
        public string[,] display = new string[mapSizeX, mapSizeY];
        public int cursorX = 0;
        public int cursorY = 0;
        public int mineChecked = 0;
        public int mineCount = 0;
        public int originalCountOfMines = 0;
    }

    public class cell
    {
        public bool isMine = false;
        public bool isChecked = false;
        public int around = 0;
    }
    public class Game
    {

        public Game()
        {
            gameData data = new gameData();

            generateMap(data);

            fillEmptyDisplay(data);
            RenderMap(data);

            bool run = true;

            while (run)
            {
                RenderMap(data);
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case (ConsoleKey)13:
                        {
                            if (data.map[data.cursorX, data.cursorY].around >= 8)
                            {
                                run = false;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Game Over");
                            }
                            if (data.map[data.cursorX, data.cursorY].around < 8)
                            {
                                data.display[data.cursorX, data.cursorY] = Convert.ToString(data.map[data.cursorX, data.cursorY].around);
                            }

                        }
                        break;
                    case (ConsoleKey)32:
                        {
                            if (data.map[data.cursorX, data.cursorY].isChecked)
                            {
                                if (data.mineChecked < data.originalCountOfMines)
                                {
                                    data.mineChecked += 1;
                                    if (data.map[data.cursorX, data.cursorY].isMine)
                                    {
                                        data.mineCount += 1;
                                    }
                                    data.map[data.cursorX, data.cursorY].isChecked = false;
                                }
                            }
                            else
                            {
                                if (data.mineChecked > 0)
                                {
                                    data.mineChecked -= 1;
                                    if (data.map[data.cursorX, data.cursorY].isMine)
                                    {
                                        data.mineCount -= 1;
                                    }
                                    data.map[data.cursorX, data.cursorY].isChecked = true;
                                }
                            }

                        }
                        break;
                    case (ConsoleKey)37:
                        {
                            if (data.cursorX - 1 >= 0)
                            {
                                data.cursorX -= 1;
                            }
                        }
                        break;
                    case (ConsoleKey)38:
                        {
                            if (data.cursorY - 1 >= 0)
                            {
                                data.cursorY -= 1;
                            }
                        }
                        break;
                    case (ConsoleKey)39:
                        {
                            if (data.cursorX + 1 < data.display.GetLength(0))
                            {
                                data.cursorX += 1;
                            }
                        }
                        break;
                    case (ConsoleKey)40:
                        if (data.cursorY + 1 < data.display.GetLength(1))
                        {
                            data.cursorY += 1;
                        }
                        break;
                }

                if (data.mineCount == 0)
                {
                    run = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("You WON!!!!!!!!!!");
                }
            }



            Console.ReadKey();
        }

        private void generateMap(gameData data)
        {
            fillEmptyMap(data);
            Random r = new Random();
            float itemID;
            for (int i = 0; i < data.map.GetLength(0); i++)
            {
                for (int j = 0; j < data.map.GetLength(1); j++)
                {
                    itemID = r.Next(0, 10);
                    if (itemID == 8)
                    {
                        data.map[i, j].around = 8;
                        data.map[i, j].isMine = true;
                        data.mineCount += 1;

                        if ((i - 1) >= 0)
                        {
                            data.map[i - 1, j].around += 1;
                        }
                        if ((i + 1) < data.map.GetLength(0))
                        {
                            data.map[i + 1, j].around += 1;
                        }
                        if ((j - 1) >= 0)
                        {
                            data.map[i, j - 1].around += 1;
                        }
                        if ((j + 1) < data.map.GetLength(1))
                        {
                            data.map[i, j + 1].around += 1;
                        }
                        if ((i - 1) >= 0 && (j - 1) >= 0)
                        {
                            data.map[i - 1, j - 1].around += 1;
                        }
                        if ((i + 1) < data.map.GetLength(0) && (j + 1) < data.map.GetLength(1))
                        {
                            data.map[i + 1, j + 1].around += 1;
                        }
                        if ((i + 1) < data.map.GetLength(0) && (j - 1) >= 0)
                        {
                            data.map[i + 1, j - 1].around += 1;
                        }
                        if ((i - 1) >= 0 && (j + 1) < data.map.GetLength(1))
                        {
                            data.map[i - 1, j + 1].around += 1;
                        }
                    }
                    else
                    {
                        data.map[i, j].around += 0;
                    }
                }
            }

            data.mineChecked = data.mineCount;
            data.originalCountOfMines = data.mineCount;
        }

        private void fillEmptyDisplay(gameData data)
        {
            for (int i = 0; i < data.display.GetLength(0); i++)
            {
                for (int j = 0; j < data.display.GetLength(1); j++)
                {
                    data.display[i, j] = ".";
                }
            }
        }

        private void fillEmptyMap(gameData data)
        {
            for (int i = 0; i < data.map.GetLength(0); i++)
            {
                for (int j = 0; j < data.map.GetLength(1); j++)
                {
                    data.map[i, j] = new cell();
                }
            }
        }
        private void RenderMap(gameData data)
        {
            Console.Clear();

            for (int i = 0; i < data.display.GetLength(0); i++)
            {
                for (int j = 0; j < data.display.GetLength(1); j++)
                {
                    if (data.map[j, i].isChecked)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    if (data.cursorX == j && data.cursorY == i)
                    {
                        Console.Write($"[{data.display[j, i]}]");
                    }
                    else
                    {
                        Console.Write($" {data.display[j, i]} ");
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
            Console.WriteLine("počet zbývajících min je :" + data.mineChecked);
        }
    }
}
