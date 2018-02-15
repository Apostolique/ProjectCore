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
        private CoolDownTimer spawnTimer;
        private int pathsShown;
        private bool spawnSequence;

        public Path()
        {
            pathway = new List<Tile>();
            spawnTimer = new CoolDownTimer(0.2f);
            spawnTimer.Reset();
            pathsShown = 0;
            spawnSequence = true;
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
            for (int i = 0; i < pathsShown; i++)
            {
                if (pathway[i] == tile) {
                    return true;
                }
            }
            return false;
        }
        public void Update(GameTime gameTime)
        {
            if (spawnSequence)
            {
                spawnTimer.Update(gameTime);
                if (spawnTimer.IsExpired)
                {
                    pathsShown++;
                    Utility.assetManager.PlaySFX("Robot_Servo_006", 0.1f);
                    spawnTimer.Reset();
                    if (pathsShown >= pathway.Count)
                    {
                        Console.WriteLine("Spawn done.");
                        spawnSequence = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch s)
        {
            for (int i = 0; i < pathsShown; i++)
            {
                s.FillRectangle(new Rectangle(Utility.GameToScreen(pathway[i].X), Utility.GameToScreen(pathway[i].Y), Utility.board.GridSize, Utility.board.GridSize), Color.Blue);
            }
        }
        public void DrawLine(SpriteBatch s)
        {
            for (int i = 1; i < pathsShown; i++)
            {
                float x1 = Utility.GameToScreen(pathway[i].X + 0.5f);
                float y1 = Utility.GameToScreen(pathway[i].Y + 0.5f);
                float x2 = Utility.GameToScreen(pathway[i - 1].X + 0.5f);
                float y2 = Utility.GameToScreen(pathway[i - 1].Y + 0.5f);

                s.DrawLine(x1, y1, x2, y2, Color.Black, Utility.board.GridSize * 0.15f);
                s.DrawLine(x1, y1, x2, y2, Color.White, Utility.board.GridSize * 0.1f);
            }
        }
    }
}
