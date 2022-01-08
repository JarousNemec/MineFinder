using System;

namespace ConsoleApp1
{
    public class GameMenuData
    {
        public int cursorPos = 0;
        public string[] mainMenuOptions = {"Začít", "Nastavení", "Odejít"};
        public string[] settingsMenuOptions = {"Uživatelské jméno", "Velikost mapy", "Opustit nastavení"};
        public string[] fieldSizeMenuOptions = {"5x5", "10x10", "15x15", "20x20", "25x25", "30x30"};
        public string[] currentShowingMenuText;
        public int currentShowingMenuId; //0 - main menu, 1 - settings, 2 - username set, 3 - map size set

        public GameMenuData()
        {
            currentShowingMenuText = mainMenuOptions;
        }
    }

    public class GameMenu
    {
        public GameMenu()
        {
            Console.Clear();
            GameMenuData data = new GameMenuData();
            DataOperator dataOperator = new DataOperator();
            Menu(data,dataOperator);
        }
        private bool menurun = true;
        private void Menu(GameMenuData data,DataOperator dataOperator)
        {
            data.cursorPos = 0;
            
            while (menurun)
            {
                RenderGameMenu(data.cursorPos, data.currentShowingMenuText);
                KeyControl(data,dataOperator);
            }
        }

        private void KeyControl(GameMenuData data, DataOperator dataOperator)
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
                    if (data.cursorPos + 1 < data.currentShowingMenuText.Length)
                    {
                        data.cursorPos += 1;
                    }
                }
                    break;
                case ConsoleKey.Enter:
                {
                    if (data.currentShowingMenuId == 0)
                    {
                        switch (data.cursorPos)
                        {
                            case 0:
                                Console.Clear();
                                Console.WriteLine("Načítání...");
                                var game = new Game();
                                break;
                            case 1:
                                data.currentShowingMenuId = 1;
                                data.currentShowingMenuText = data.settingsMenuOptions;
                                break;
                            case 2:
                                Environment.Exit(0);
                                break;
                        }
                    }
                    else if (data.currentShowingMenuId == 1)
                    {
                        switch (data.cursorPos)
                        {
                            case 0:
                                SetUserName(dataOperator);
                                break;
                            case 1:
                                data.currentShowingMenuId = 3;
                                data.currentShowingMenuText = data.fieldSizeMenuOptions;
                                break;
                            case 2:
                                data.currentShowingMenuId = 0;
                                data.currentShowingMenuText = data.mainMenuOptions;
                                break;
                        }
                    }
                    else if (data.currentShowingMenuId == 3)
                    {
                        SetMapSize(dataOperator,(data.cursorPos+1)*5);
                        data.currentShowingMenuId = 1;
                        data.currentShowingMenuText = data.settingsMenuOptions;
                    }

                    data.cursorPos = 0;
                }
                    break;
            }
        }

        private void SetUserName(DataOperator dataOperator)
        {
            Console.Clear();
            Console.WriteLine("Zadejte nové uživatelské jméno: ");
            dataOperator.WriteSetting(new string[]{Console.ReadLine(),""});
        }
        private void SetMapSize(DataOperator dataOperator,int value)
        {
            dataOperator.WriteSetting(new string[]{"",Convert.ToString(value)});
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