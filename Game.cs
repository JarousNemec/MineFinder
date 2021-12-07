using System;
using System.Text;

namespace ConsoleApp1
{
    public class Game
    {
        public Game()
        {
            DataOperator dataOp = new DataOperator();
            GameData data = LoadData(dataOp);
            Play(data, dataOp);
            Console.ReadKey();
        }

        private void Play(GameData data, DataOperator dataOp)
        {
            GenerateMap(data);
            bool isGameRunning = true;
            while (isGameRunning)
            {
                RenderMap(data);
                isGameRunning = KeyController(data);
                if (data.mineCount == 0 && (data.map.Length - data.originalCountOfMines) == data.uncoveredFields)
                {
                    isGameRunning = false;
                    Win(data, dataOp);
                }
            }
        }

        private GameData LoadData(DataOperator dataOperator)
        {
            if (int.TryParse(dataOperator.ReadSetting("mapSize"), out int size))
            {
                return new GameData(size);
            }

            return new GameData(10);
        }

        private void Win(GameData data, DataOperator dataOp)
        {
            dataOp.LogUserPerformenceToDb(
                CalculatePlayTime(data), data.map.Length, data.originalCountOfMines);
            WinningScreen(data);
        }

        private void WinningScreen(GameData data)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Vyhráli jste");
            Console.WriteLine("Herní čas je: " + (CalculatePlayTime(data)));
            Console.ResetColor();
        }

        private TimeSpan CalculatePlayTime(GameData data)
        {
            return (new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) - data.beginTime);
        }

        private static bool KeyController(GameData data)
        {
            ConsoleKeyInfo key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    if (!UncoverField(data)) return false;
                    break;
                case ConsoleKey.Spacebar:
                    MarkField(data);
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    MoveCursorLeft(data);
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    MoveCursorUp(data);
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    MoveCursorRight(data);
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    MoveCursorDown(data);
                    break;
            }

            return true;
        }

        private static bool UncoverField(GameData data)
        {
            if (data.map[data.cursorX, data.cursorY].isMine)
            {
                DefeatScreen();
                return false;
            }

            if (data.map[data.cursorX, data.cursorY].isCovered)
            {
                data.map[data.cursorX, data.cursorY].isCovered = false;

                data.uncoveredFields++;
            }

            return true;
        }

        private static void DefeatScreen()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Vybouchli jste :)");
            Console.ResetColor();
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
            if (data.cursorX + 1 < data.map.GetLength(0))
            {
                data.cursorX += 1;
            }
        }

        private static void MoveCursorDown(GameData data)
        {
            if (data.cursorY + 1 < data.map.GetLength(1))
            {
                data.cursorY += 1;
            }
        }

        private void GenerateMap(GameData data)
        {
            FillEmptyMap(data);

            Random r = new Random();
            while (data.mineCount < 2)
            {
                data.mineCount = 0;
                for (int y = 0; y < data.map.GetLength(0); y++)
                {
                    for (int x = 0; x < data.map.GetLength(1); x++)
                    {
                        InitializeNewField(data, r, x, y);
                    }
                }

                data.mineChecked = data.mineCount;
                data.originalCountOfMines = data.mineCount;
            }
        }

        private static void InitializeNewField(GameData data, Random r, int x, int y)
        {
            int itemId = r.Next(0, 10);
            if (itemId == 8)
            {
                CreateMine(data, x, y);
            }
            else
            {
                CreateEmptyField(data, x, y);
            }
        }

        private static void CreateEmptyField(GameData data, int x, int y)
        {
            data.map[x, y].around += 0;
        }

        private static void CreateMine(GameData data, int x, int y)
        {
            data.map[x, y].isMine = true;
            data.mineCount += 1;
            IncreaseValueByOneInFieldsAroundMine(data, x, y);
        }

        private static void IncreaseValueByOneInFieldsAroundMine(GameData data, int x, int y)
        {
            if ((x - 1) >= 0)
            {
                data.map[x - 1, y].around += 1;
            }

            if ((x + 1) < data.map.GetLength(1))
            {
                data.map[x + 1, y].around += 1;
            }

            if ((y - 1) >= 0)
            {
                data.map[x, y - 1].around += 1;
            }

            if ((y + 1) < data.map.GetLength(0))
            {
                data.map[x, y + 1].around += 1;
            }

            if ((x - 1) >= 0 && (y - 1) >= 0)
            {
                data.map[x - 1, y - 1].around += 1;
            }

            if ((x + 1) < data.map.GetLength(1) && (y + 1) < data.map.GetLength(0))
            {
                data.map[x + 1, y + 1].around += 1;
            }

            if ((x + 1) < data.map.GetLength(1) && (y - 1) >= 0)
            {
                data.map[x + 1, y - 1].around += 1;
            }

            if ((x - 1) >= 0 && (y + 1) < data.map.GetLength(0))
            {
                data.map[x - 1, y + 1].around += 1;
            }
        }

        private void FillEmptyMap(GameData data)
        {
            for (int y = 0; y < data.map.GetLength(0); y++)
            {
                for (int x = 0; x < data.map.GetLength(1); x++)
                {
                    data.map[x, y] = new Field();
                }
            }
        }

        private StringBuilder sb;

        private void RenderMap(GameData data)
        {
            Console.Clear();
            sb = new StringBuilder();

            for (int y = 0; y < data.map.GetLength(0); y++)
            {
                for (int x = 0; x < data.map.GetLength(1); x++)
                {
                    if (data.cursorX == x && data.cursorY == y)
                    {
                        if (data.map[x, y].isChecked)
                        {
                            sb.Append($"[⚑]");
                            continue;
                        }

                        if (data.map[x, y].isCovered)
                        {
                            sb.Append($"[.]");
                        }
                        else
                        {
                            sb.Append($"[{data.map[x, y].around}]");
                        }
                    }
                    else
                    {
                        if (data.map[x, y].isChecked)
                        {
                            sb.Append($" ⚑ ");
                            continue;
                        }

                        if (data.map[x, y].isCovered)
                        {
                            sb.Append($" . ");
                        }
                        else
                        {
                            sb.Append($" {data.map[x, y].around} ");
                        }
                    }
                }

                sb.Append(Environment.NewLine);
            }

            sb.Append("počet zbývajících min je :" + data.mineChecked);
            Console.WriteLine(sb);
        }
    }
}