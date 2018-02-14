using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameJamProject
{
    /// <summary>
    /// Goal: Junk class, used for global variables.
    /// </summary>
    static class Utility
    {
        public static Board board = new Board(15, 10);
        public enum TowerType { Sniper, Shotgun, FlameThrower };
        public static SpriteFont mainFont;
        public static GameWindow Window;

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


