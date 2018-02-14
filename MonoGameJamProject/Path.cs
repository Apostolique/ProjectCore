using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject
{
    /// <summary>
    /// Goal: Base class for the game's pathways.
    ///       Steps:
    ///         1) Pick a tile on the edges.
    ///         2) Make a list of all the surrounding tiles.
    ///         3) Eliminate tiles that have a path of an other color.
    ///         4) Pick one of the available tiles based on:
    ///             a) If path is short, bias towards the center.
    ///             b) If path is long, bias towars the edges.
    ///             c) Also bias for longer paths if the mode is easy.
    /// </summary>
    class Path
    {
        public List<Tile> pathway;
        public Path()
        {
            pathway = new List<Tile>();
        }

        public void Add(Tile p)
        {
            pathway.Add(p);
        }
        public Tile First()
        {
            return pathway.First();
        }
        public Tile Last()
        {
            return pathway.Last();
        }
        public int Count()
        {
            return pathway.Count;
        }
        public bool Contains(Tile tile)
        {
            return pathway.Contains(tile);
        }

        public void Draw(SpriteBatch s, int gridSize)
        {
            foreach (Tile p in pathway)
            {
                s.FillRectangle(new Rectangle(Utility.GameToScreen(p.X), Utility.GameToScreen(p.Y), gridSize, gridSize), Color.Blue);
            }
        }
        public void DrawLine(SpriteBatch s, int gridSize)
        {
            for (int i = 1; i < pathway.Count; i++)
            {
                float x1 = Utility.GameToScreen(pathway[i].X + 0.5f);
                float y1 = Utility.GameToScreen(pathway[i].Y + 0.5f);
                float x2 = Utility.GameToScreen(pathway[i - 1].X + 0.5f);
                float y2 = Utility.GameToScreen(pathway[i - 1].Y + 0.5f);

                s.DrawLine(x1, y1, x2, y2, Color.Black, gridSize * 0.15f);
                s.DrawLine(x1, y1, x2, y2, Color.White, gridSize * 0.1f);
            }
        }
    }
}
