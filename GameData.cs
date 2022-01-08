using System;

namespace ConsoleApp1
{
    public class GameData
    {
        public Field[,] map;
        public int cursorY;
        public int cursorX;
        public int mineChecked;
        public int mineCount;
        public int uncoveredFields;
        public int originalCountOfMines;
        public TimeSpan beginTime;

        public GameData(int mapSize)
        {
            map = new Field[mapSize, mapSize];
            beginTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }
    }
}