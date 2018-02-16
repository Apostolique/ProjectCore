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
        enum animation { spawn, despawn, none }
        public List<Tile> pathway;
        public List<Minion> MinionList;
        private CoolDownTimer spawnTimer;
        private int pathsShown;
        private animation sequence;
        private bool _done;
        public bool Done
        {
            get => _done;
        }

        public Path()
        {
            pathway = new List<Tile>();
            MinionList = new List<Minion>();
            spawnTimer = new CoolDownTimer(0.2f);
            spawnTimer.Reset();
            pathsShown = 0;
            sequence = animation.spawn;
            _done = false;
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
            if (sequence == animation.spawn) {
                for (int i = 0; i < pathsShown; i++)
                {
                    if (pathway[i] == tile) {
                        return true;
                    }
                }
            } else if (sequence == animation.despawn) {
                for (int i = pathsShown; i < pathway.Count; i++)
                {
                    if (pathway[i] == tile) {
                        return true;
                    }
                }
            } else {
                return pathway.Contains(tile);
            }

            return false;
        }
        public void Despawn()
        {
            spawnTimer.Reset();
            pathsShown = 0;
            sequence = animation.despawn;
        }
        public void AddMinion(Minion m)
        {
            m.FollowPath(this);
            MinionList.Add(m);
        }
        public void Update(GameTime gameTime)
        {
            if (sequence == animation.spawn)
            {
                spawnTimer.Update(gameTime);
                if (spawnTimer.IsExpired)
                {
                    pathsShown++;
                    Utility.assetManager.PlaySFX("Robot_Servo_006", 0.1f);
                    spawnTimer.Reset();
                    if (pathsShown >= pathway.Count)
                    {
                        sequence = animation.none;
                    }
                }
            } else if (sequence == animation.despawn)
            {
                spawnTimer.Update(gameTime);
                if (spawnTimer.IsExpired)
                {
                    pathsShown++;
                    Utility.assetManager.PlaySFX("Robot_Servo_006", 0.1f);
                    spawnTimer.Reset();
                    if (pathsShown >= pathway.Count)
                    {
                        sequence = animation.none;
                        _done = true;
                    }
                }
            }
            for(int i = MinionList.Count - 1; i >= 0; i--)
            {
                MinionList[i].Update(gameTime);
                if (MinionList[i].dead || !MinionList[i].IsMoving)
                    MinionList.Remove(MinionList[i]);
            }
        }

        public void Draw(SpriteBatch s)
        {
            if (sequence == animation.spawn)
            {
                for (int i = 0; i < pathsShown; i++)
                {
                    drawPathTile(s, i);
                }
            } else if (sequence == animation.despawn)
            {
                for (int i = pathsShown; i < pathway.Count; i++)
                {
                    drawPathTile(s, i);
                }
            } else
            {
                for (int i = 0; i < pathway.Count; i++) 
                {
                    drawPathTile(s, i);
                }
            }
        }
        public void DrawLine(SpriteBatch s)
        {
            if (sequence == animation.spawn)
            {
                for (int i = 1; i < pathsShown; i++)
                {
                    drawPathLine(s, i);
                }
            } else if (sequence == animation.despawn)
            {
                for (int i = pathsShown + 1; i < pathway.Count; i++)
                {
                    drawPathLine(s, i);
                }
            } else
            {
                for (int i = 1; i < pathway.Count; i++)
                {
                    drawPathLine(s, i);
                }
            }
        }
        public void DrawMinions(SpriteBatch s)
        {
            foreach (Minion m in MinionList)
                m.Draw(s);
        }
        private void drawPathTile(SpriteBatch s, int i)
        {
            s.FillRectangle(new Rectangle(Utility.GameToScreen(pathway[i].X), Utility.GameToScreen(pathway[i].Y), Utility.board.GridSize, Utility.board.GridSize), new Color(19, 59, 131));
        }
        private void drawPathLine(SpriteBatch s, int i)
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
