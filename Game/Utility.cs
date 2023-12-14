using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameProject.Towers;

namespace GameProject {
    /// <summary>
    /// Goal: Junk class, used for global variables.
    /// </summary>
    static class Utility {
        public static Board Board = null!;
        public enum TowerType { Sniper, Shotgun, FlameThrower};
        public enum MinionType { Slow, Fast, Boss };
        public enum GameState { Playing, GameOver };
        public static GameState CurrentGameState;
        public const int SlowMinionHP = 100;
        public const int FastMinionHP = 10;
        public const int BossMinionHp = 3000;
        public static int TotalNumberOfKills = 0;
        public static int NumberOfLives;
        public static List<Tower> TowerList = null!;
        public static int MaxTowers => Board.Paths.Count + 1;
        public static int Score => (GameDifficulty * 20) + TotalNumberOfKills * GameDifficulty / 3;
        public static GameWindow Window = null!;
        public static AssetManager AssetManager = null!;
        public static int SidebarWidth = 200;
        public static int GameDifficulty = 0;

        public static TimeSpan TDGameTimer;

        /// <summary>
        /// Converts a game coordinate into a screen coordinate.
        /// </summary>
        public static int GameToScreen(int value) {
            return value * Board.GridSize - Board.GridSize;
        }
        /// <summary>
        /// Converts a game coordinate into a screen coordinate.
        /// </summary>
        public static float GameToScreen(float value) {
            return value * Board.GridSize - Board.GridSize;
        }
        /// <summary>
        /// Converts a screen coordinate into a game coordinate.
        /// </summary>
        public static int ScreenToGame(int value) {
            return (value +Board.GridSize) / Board.GridSize;
        }
        /// <summary>
        /// Converts a screen coordinate into a game coordinate.
        /// </summary>
        public static float ScreenToGame(float value) {
            return (value + Board.GridSize) / Board.GridSize;
        }
        public static readonly Random random = new Random();
    }
}
