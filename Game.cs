using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    public class GameData
    {
        private static int mapSizeX = 10;
        private static int mapSizeY = mapSizeX;
        public Field[,] map = new Field[mapSizeX, mapSizeY];
        public string[,] display = new string[mapSizeX, mapSizeY];
        public int cursorX = 0;
        public int cursorY = 0;
        public int mineChecked = 0;
        public int mineCount = 0;
        public int originalCountOfMines = 0;
    }
    public class Field
    {
        public bool isMine = false;
        public bool isChecked = false;
        public int around = 0;
    }
    public class Game
    {
        public Game()
        {
            GameData data = new GameData();
            Play(data);
            Console.ReadKey();
        }
        private void Play(GameData data)
        {
            GenerateMap(data);
            FillEmptyDisplay(data);
            RenderMap(data);
            bool isGameRunning = true;
            while (isGameRunning)
            {
                RenderMap(data);
                isGameRunning = KeyController(data);
                if (data.mineCount == 0)
                {
                    isGameRunning = false;
                    WinningScreen();
                }
            }
        }
        private static void WinningScreen()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You WON!!!!!!!!!!");
        }
        private static bool KeyController(GameData data)
        {
            ConsoleKeyInfo key = Console.ReadKey();
            switch (key.Key)
            {
                case (ConsoleKey) 13:
                    if (!UncoverField(data)) return false;
                    break;
                case (ConsoleKey) 32:
                    MarkField(data);
                    break;
                case (ConsoleKey) 37:
                    MoveCursorLeft(data);
                    break;
                case (ConsoleKey) 38:
                    MoveCursorUp(data);
                    break;
                case (ConsoleKey) 39:
                    MoveCursorRight(data);
                    break;
                case (ConsoleKey) 40:
                    MoveCursorDown(data);
                    break;
            }
            return true;
        }
        private static bool UncoverField(GameData data)
        {
            if (data.map[data.cursorX, data.cursorY].around >= 8)
            {
                DefeatScreen();
                return false;
            }

            if (data.map[data.cursorX, data.cursorY].around < 8)
            {
                data.display[data.cursorX, data.cursorY] =
                    Convert.ToString(data.map[data.cursorX, data.cursorY].around);
            }

            return true;
        }
        private static void DefeatScreen()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Game Over");
        }
        private static void MarkField(GameData data)
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
        private static void MoveCursorLeft(GameData data)
        {
            if (data.cursorX - 1 >= 0)
            {
                data.cursorX -= 1;
            }
        }
        private static void MoveCursorUp(GameData data)
        {
            if (data.cursorY - 1 >= 0)
            {
                data.cursorY -= 1;
            }
        }
        private static void MoveCursorRight(GameData data)
        {
            if (data.cursorX + 1 < data.display.GetLength(0))
            {
                data.cursorX += 1;
            }
        }
        private static void MoveCursorDown(GameData data)
        {
            if (data.cursorY + 1 < data.display.GetLength(1))
            {
                data.cursorY += 1;
            }
        }
        private void GenerateMap(GameData data)
        {
            FillEmptyMap(data);

            Random r = new Random();

            for (int i = 0; i < data.map.GetLength(0); i++)
            {
                for (int j = 0; j < data.map.GetLength(1); j++)
                {
                    InitializeNewField(data, r, i, j);
                }
            }

            data.mineChecked = data.mineCount;
            data.originalCountOfMines = data.mineCount;
        }
        private static void InitializeNewField(GameData data, Random r, int i, int j)
        {
            int itemId = r.Next(0, 10);
            if (itemId == 8)
            {
                CreateMine(data, i, j);
            }
            else
            {
                CreateEmptyField(data, i, j);
            }
        }
        private static void CreateEmptyField(GameData data, int i, int j)
        {
            data.map[i, j].around += 0;
        }
        private static void CreateMine(GameData data, int i, int j)
        {
            data.map[i, j].around = 8;
            data.map[i, j].isMine = true;
            data.mineCount += 1;
            IncreaseValueByOneInFieldsAroundMine(data, i, j);
        }
        private static void IncreaseValueByOneInFieldsAroundMine(GameData data, int i, int j)
        {
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
        private void FillEmptyDisplay(GameData data)
        {
            for (int i = 0; i < data.display.GetLength(0); i++)
            {
                for (int j = 0; j < data.display.GetLength(1); j++)
                {
                    data.display[i, j] = ".";
                }
            }
        }
        private void FillEmptyMap(GameData data)
        {
            for (int i = 0; i < data.map.GetLength(0); i++)
            {
                for (int j = 0; j < data.map.GetLength(1); j++)
                {
                    data.map[i, j] = new Field();
                }
            }
        }
        private void RenderMap(GameData data)
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
