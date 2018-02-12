using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoGameJamProject
{
    /// <summary>
    /// Goal: Junk class, used for global variables.
    /// </summary>
    static class Utility
    {
        public static GameWindow Window;
        /// <summary>
        /// Converts a game coordinate into a screen coordinate.
        /// </summary>
        public static int GameToScreen(int value, int gridSize) {
            return value * gridSize - gridSize;
        }
        /// <summary>
        /// Converts a game coordinate into a screen coordinate.
        /// </summary>
        public static float GameToScreen(float value, int gridSize) {
            return value * gridSize - gridSize;
        }
        /// <summary>
        /// Converts a screen coordinate into a game coordinate.
        /// </summary>
        public static int ScreenToGame(int value, int gridSize) {
            return (value + gridSize) / gridSize;
        }
        /// <summary>
        /// Converts a screen coordinate into a game coordinate.
        /// </summary>
        public static float ScreenToGame(float value, int gridSize) {
            return (value + gridSize) / gridSize;
        }
    }
}


