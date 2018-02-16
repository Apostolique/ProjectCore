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
    /// Goal: Board, not actually sure what it will handle...
    /// </summary>
    class Board
    {
        public List<Path> Paths
        {
            get; set;
        }
        public List<Path> OldPaths
        {
            get; set;
        }
        Tile[,] tiles;
        struct TileValue
        {
            public Tile tile;
            public int value;
            public TileValue(Tile t, int v)
            {
                tile = t;
                value = v;
            }
        }
        public Board(int iWidth, int iHeight)
        {
            Width = iWidth;
            Height = iHeight;

            Paths = new List<Path>();
            OldPaths = new List<Path>();
            tiles = new Tile[FullWidth, FullHeight];

            for (int i = 0; i < FullWidth; i++)
            {
                for (int j = 0; j < FullHeight; j++)
                {
                    tiles[i, j] = new Tile(i, j);
                }
            }
            CacheGridSize();
        }
        public void Update(GameTime gameTime)
        {
            CacheGridSize();
            foreach (Path p in Paths)
            {
                p.Update(gameTime);
            }
            for (int i = OldPaths.Count - 1; i >= 0; i--)
            {
                OldPaths[i].Update(gameTime);
                if (OldPaths[i].Done) {
                    OldPaths.RemoveAt(i);
                }
            }
        }
        public void CacheGridSize()
        {
            _gridSize = Math.Min((Utility.Window.ClientBounds.Width - Utility.sidebarWidth) / Width, Utility.Window.ClientBounds.Height / Height);
        }
        public int Width
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
        public int FullWidth
        {
            get => Width + 2;
        }
        public int FullHeight
        {
            get => Height + 2;
        }
        int _gridSize = 1;
        public int GridSize => _gridSize;
        public Tile GetTile(Point p)
        {
            return tiles[p.X, p.Y];
        }
        public void ClearPaths()
        {
            foreach (Path p in Paths)
            {
                OldPaths.Add(p);
                p.Despawn();
            }
            Paths.Clear();
        }
        public void GeneratePath()
        {
            Path newPath = new Path();

            int preferedSize = 13;
            int length = 0;
            
            //Edge restriction: Only edges in paths at the start and end of a path.
            //1) Pick an edge.
            //2) Enter the field.
            //3) When the path is short, bias towards the center.
            //4) When the path is long, bias towards the edges.
            //5) If game difficulty is hard, bias towards shorter paths.
            HashSet<Tile> edges = GetEdgeTiles();
            RemoveCorners(ref edges);
            RemoveStartAndEnd(ref edges);

            Tile startTile = edges.ElementAt(Utility.random.Next(edges.Count));
            Tile t = startTile;
            newPath.Add(t);
            length++;

            do
            {
                HashSet<Tile> nextTiles = FindNeighbors(t);
                RemoveStartAndEnd(ref nextTiles);
                List<TileValue> nextTileSort = new List<TileValue>();
                foreach (Tile i in nextTiles)
                {
                    nextTileSort.Add(new TileValue(i, GetTileValue(i, startTile, length, preferedSize, newPath)));
                }
                nextTileSort.Sort((v1, v2) => v1.value.CompareTo(v2.value));
                foreach (TileValue tv in nextTileSort) {
                }
                t = nextTileSort.Last().tile;
                newPath.Add(t);
                length++;
            } while (t != startTile && !IsEdge(t));

            Paths.Add(newPath);
        }
        public bool IsTileOnPath(Tile tile)
        {
            foreach (Path p in Paths)
            {
                if (p.Contains(tile))
                {
                    return true;
                }
            }
            foreach (Path p in OldPaths)
            {
                if (p.Contains(tile))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsEdge(Tile tile)
        {
            HashSet<Tile> edges = GetEdgeTiles();
            return edges.Contains(tile);
        }
        private int GetTileValue(Tile t, Tile start, int length, int preferedSize, Path p) {
            int tileValue = 0;

            bool isEdge = GetEdgeTiles().Contains(t);

            Vector2 relativeCenter = new Vector2(0.5f, 0.5f);
            Vector2 relativeStart = new Vector2((start.X + 0.5f) / FullWidth, (start.Y + 0.5f) / FullHeight);
            Vector2 relativeMapping = new Vector2((t.X + 0.5f) / FullWidth, (t.Y + 0.5f) / FullHeight);

            int centerDistance = (int)(Vector2.Distance(relativeCenter, relativeMapping) * 100f);
            int startDistance = (int)(Vector2.Distance(relativeStart, relativeMapping) * 50f);

            if (isEdge && length > preferedSize) {
                tileValue += Utility.random.Next(50, 120);
            } else if (!isEdge && length > preferedSize) {
                tileValue += centerDistance + Utility.random.Next(0, 20);
            } else if (!isEdge) {
                tileValue += (100 - centerDistance) * 3 + Utility.random.Next(0, 100);
            }

            if (p.Contains(t)) {
                tileValue -= 30;
            }

            tileValue -= 75 - startDistance;

            return tileValue;
        }
        private HashSet<Tile> GetEdgeTiles()
        {
            HashSet<Tile> edgeTiles = new HashSet<Tile>();

            //Get top row.
            edgeTiles.UnionWith(GetRow(0));
            //Get bottom row.
            edgeTiles.UnionWith(GetRow(FullHeight - 1));
            //Get left column.
            edgeTiles.UnionWith(GetColumn(0));
            //Get right column.
            edgeTiles.UnionWith(GetColumn(FullWidth - 1));

            return edgeTiles;
        }
        private void RemoveEdges(ref HashSet<Tile> tileSet)
        {
            foreach (Tile t in GetEdgeTiles())
            {
                tileSet.Remove(t);
            }
        }
        private void RemoveCorners(ref HashSet<Tile> tileSet)
        {
            tileSet.Remove(tiles[0, 0]);
            tileSet.Remove(tiles[0, FullHeight - 1]);
            tileSet.Remove(tiles[FullWidth - 1, 0]);
            tileSet.Remove(tiles[FullWidth - 1, FullHeight - 1]);
        }
        private void RemoveStartAndEnd(ref HashSet<Tile> tileSet)
        {
            foreach (Path p in Paths) {
                tileSet.Remove(p.First());
                tileSet.Remove(p.Last());
            }
        }
        private HashSet<Tile> GetRow(int row)
        {
            HashSet<Tile> rowTiles = new HashSet<Tile>();
            for (int i = 0; i < FullWidth; i++)
            {
                rowTiles.Add(tiles[i, row]);
            }

            return rowTiles;
        }
        private HashSet<Tile> GetColumn(int column)
        {
            HashSet<Tile> rowTiles = new HashSet<Tile>();
            for (int i = 0; i < FullHeight; i++)
            {
                rowTiles.Add(tiles[column, i]);
            }

            return rowTiles;
        }
        private HashSet<Tile> FindNeighbors(Tile t)
        {
            HashSet<Tile> neighbors = new HashSet<Tile>();
            if (t.X - 1 >= 0)
            {
                neighbors.Add(tiles[t.X - 1, t.Y]);
            }
            if (t.X + 1 < FullWidth)
            {
                neighbors.Add(tiles[t.X + 1, t.Y]);
            }
            if (t.Y - 1 >= 0)
            {
                neighbors.Add(tiles[t.X, t.Y - 1]);
            }
            if (t.Y + 1 < FullHeight)
            {
                neighbors.Add(tiles[t.X, t.Y + 1]);
            }

            return neighbors;
        }
        public void Draw(SpriteBatch s)
        {
            s.FillRectangle(new Rectangle(0, 0, GridSize * Width, GridSize * Height), new Color(15, 15, 15));

            for (int i = 1; i < FullWidth - 1; i++)
            {
                for (int j = 1; j < FullHeight - 1; j++)
                {
                    tiles[i, j].Draw(s, Color.White);
                }
            }
            DrawPaths(s, OldPaths);
            DrawPathLines(s, OldPaths);
            DrawPaths(s, Paths);
            DrawPathLines(s, Paths);
        }
        public void DrawPaths(SpriteBatch s, List<Path> paths)
        {
            foreach (Path p in paths)
            {
                p.Draw(s);
            }
        }
        public void DrawPathLines(SpriteBatch s, List<Path> paths)
        {
            foreach (Path p in paths)
            {
                p.DrawLine(s);
            }
        }
    }
}
