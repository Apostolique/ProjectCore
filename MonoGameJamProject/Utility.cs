using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameJamProject.Towers;

namespace MonoGameJamProject
{
    /// <summary>
    /// Goal: Junk class, used for global variables.
    /// </summary>
    static class Utility
    {
        public static Board board;
        public enum TowerType { Sniper, Shotgun, FlameThrower};
        public enum MinionType { slow, fast, boss };
        public enum GameState { Playing, GameOver };
        public static GameState currentGamestate;
        public const int slowMinionHP = 100;
        public const int fastMinionHP = 10;
        public const int bossMinionHp = 3000;
        public static int totalNumberOfKills = 0;
        public static int numberOfLives;
        public static List<Tower> TowerList;
        public static int MaxTowers => Utility.board.Paths.Count + 1;
        public static GameWindow Window;
        public static AssetManager assetManager;
        public static int sidebarWidth = 200;
        public static int GameDifficulty = 0;

        public static TimeSpan tdGameTimer;

        /// <summary>
        /// Converts a game coordinate into a screen coordinate.
        /// </summary>
        public static int GameToScreen(int value)
        {
            return value * board.GridSize - board.GridSize;
        }
        /// <summary>
        /// Converts a game coordinate into a screen coordinate.
        /// </summary>
        public static float GameToScreen(float value)
        {
            return value * board.GridSize - board.GridSize;
        }
        /// <summary>
        /// Converts a screen coordinate into a game coordinate.
        /// </summary>
        public static int ScreenToGame(int value)
        {
            return (value +board.GridSize) / board.GridSize;
        }
        /// <summary>
        /// Converts a screen coordinate into a game coordinate.
        /// </summary>
        public static float ScreenToGame(float value)
        {
            return (value + board.GridSize) / board.GridSize;
        }
        public static readonly Random random = new Random();
    }
}


