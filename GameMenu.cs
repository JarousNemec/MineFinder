using System;
using System.Threading;

namespace ConsoleApp1
{
    public class GameMenuData
    {
        public int cursorPos = 0;
        public string[] mainMenuOptions = new string[] {"START", "SETTINGS", "EXIT"};
        public string[] fieldSizeMenuOptions = new string[] {"5x5", "10x10", "15x15", "20x20", "25x25", "30x30"};
    }

    public class GameMenu
    {
        private GameMenuData data;

        public GameMenu()
        {
            data = new GameMenuData();
            MainMenu(data);
        }

        private void MainMenu(GameMenuData data)
        {
            bool runProgram = true;
            while (runProgram)
            {
                RenderGameMenu(data.cursorPos, data.fieldSizeMenuOptions);
                KeyControl(data, data.mainMenuOptions);
            }
        }

        private void KeyControl(GameMenuData data, string[] options)
        {
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                {
                    if (data.cursorPos - 1 >= 0)
                    {
                        data.cursorPos -= 1;
                    }
                }
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                {
                    if (data.cursorPos + 1 < options.Length)
                    {
                        data.cursorPos += 1;
                    }
                }
                    break;
                case ConsoleKey.Enter:
                {
                    switch (data.cursorPos)
                    {
                        case 0:
                            new Game();
                            break;
                        case 1:
                            break;
                        case 2:
                            Environment.Exit(0);
                            break;
                    }
                }
                    break;
            }
        }

        private void RenderGameMenu(int cursorPos, string[] menuOptions)
        {
            Console.Clear();

            int longestWord = GetLenghtOfLongestWord(menuOptions);
            for (int i = 0; i < menuOptions.Length; i++)
            {
                string row = AsembleRowWithCurrentSpacesCount(menuOptions[i], longestWord, 4);
                if (i == cursorPos)
                {
                    Console.WriteLine($">{row}<");
                }
                else
                {
                    Console.WriteLine($" {row} ");
                }
            }
        }

        private int GetLenghtOfLongestWord(string[] words)
        {
            int longestWord = 0;
            foreach (string word in words)
            {
                if (word.Length > longestWord)
                {
                    longestWord = word.Length;
                }
            }

            return longestWord;
        }

        private string AsembleRowWithCurrentSpacesCount(string optionInRow, int longestWord, int minimalSideSpaces)
        {
            string row = optionInRow;

            if (row.Length % 2 != 0)
            {
                row += " ";
            }

            int currentCharsCount = longestWord + minimalSideSpaces + minimalSideSpaces;
            int spacesCount = currentCharsCount - row.Length;
            int spacesOnOneSide = spacesCount / 2;

            return $"{new string(' ', spacesOnOneSide)}{row}{new string(' ', spacesOnOneSide)}";
        }
    }
}