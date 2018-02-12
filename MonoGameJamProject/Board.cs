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
        List<Path> paths;
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

            paths = new List<Path>();
            tiles = new Tile[FullWidth, FullHeight];

            for (int i = 0; i < FullWidth; i++)
            {
                for (int j = 0; j < FullHeight; j++)
                {
                    tiles[i, j] = new Tile(i, j);
                }
            }
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
        public int GridSize => Math.Min(Utility.Window.ClientBounds.Width / Width, Utility.Window.ClientBounds.Height / Height);
        public Tile GetTile(Point p)
        {
            return tiles[p.X, p.Y];
        }
        public void ClearPaths()
        {
            paths.Clear();
        }
        public void GeneratePath()
        {
            Path newPath = new Path();

            int preferedSize = 15;
            int length = 0;
            
            //Edge restriction: Only edges in paths at the start and end of a path.
            //1) Pick an edge.
            //2) Enter the field.
            //3) When the path is short, bias towards the center.
            //4) When the path is long, bias towards the edges.
            //5) If game difficulty is hard, bias towards shorter paths.
            HashSet<Tile> edges = getEdgeTiles();
            removeCorners(ref edges);
            removeStartAndEnd(ref edges);

            Tile startTile = edges.ElementAt(Utility.random.Next(edges.Count));
            Tile t = startTile;
            newPath.Add(t);
            Console.WriteLine("Added: " + t.X + ", " + t.Y);
            length++;

            do
            {
                HashSet<Tile> nextTiles = findNeighbors(t);
                removeStartAndEnd(ref nextTiles);
                List<TileValue> nextTileSort = new List<TileValue>();
                foreach (Tile i in nextTiles)
                {
                    nextTileSort.Add(new TileValue(i, getTileValue(i, startTile, length, preferedSize, newPath)));
                }
                nextTileSort.Sort((v1, v2) => v1.value.CompareTo(v2.value));
                foreach (TileValue tv in nextTileSort) {
                    Console.WriteLine("\tSort: " + tv.tile.X + ", " + tv.tile.Y + ": " + tv.value);
                }
                t = nextTileSort.Last().tile;
                newPath.Add(t);
                length++;
                Console.WriteLine("Added: " + t.X + ", " + t.Y);
            } while (t != startTile && !isEdge(t));

            paths.Add(newPath);
        }
        private bool isEdge(Tile t)
        {
            HashSet<Tile> edges = getEdgeTiles();
            return edges.Contains(t);
        }
        private int getTileValue(Tile t, Tile start, int length, int preferedSize, Path p) {
            int tileValue = 0;

            bool isEdge = getEdgeTiles().Contains(t);

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
        private HashSet<Tile> getEdgeTiles()
        {
            HashSet<Tile> edgeTiles = new HashSet<Tile>();

            //Get top row.
            edgeTiles.UnionWith(getRow(0));
            //Get bottom row.
            edgeTiles.UnionWith(getRow(FullHeight - 1));
            //Get left column.
            edgeTiles.UnionWith(getColumn(0));
            //Get right column.
            edgeTiles.UnionWith(getColumn(FullWidth - 1));

            return edgeTiles;
        }
        private void removeEdges(ref HashSet<Tile> tileSet)
        {
            foreach (Tile t in getEdgeTiles())
            {
                tileSet.Remove(t);
            }
        }
        private void removeCorners(ref HashSet<Tile> tileSet)
        {
            tileSet.Remove(tiles[0, 0]);
            tileSet.Remove(tiles[0, FullHeight - 1]);
            tileSet.Remove(tiles[FullWidth - 1, 0]);
            tileSet.Remove(tiles[FullWidth - 1, FullHeight - 1]);
        }
        private void removeStartAndEnd(ref HashSet<Tile> tileSet)
        {
            foreach (Path p in paths) {
                tileSet.Remove(p.First());
                tileSet.Remove(p.Last());
            }
        }
        private HashSet<Tile> getRow(int row)
        {
            HashSet<Tile> rowTiles = new HashSet<Tile>();
            for (int i = 0; i < FullWidth; i++)
            {
                rowTiles.Add(tiles[i, row]);
            }

            return rowTiles;
        }
        private HashSet<Tile> getColumn(int column)
        {
            HashSet<Tile> rowTiles = new HashSet<Tile>();
            for (int i = 0; i < FullHeight; i++)
            {
                rowTiles.Add(tiles[column, i]);
            }

            return rowTiles;
        }
        private HashSet<Tile> findNeighbors(Tile t)
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
            s.FillRectangle(new Rectangle(0, 0, GridSize * Width, GridSize * Height), new Color(10, 10, 10));

            for (int i = 1; i < FullWidth - 1; i++)
            {
                for (int j = 1; j < FullHeight - 1; j++)
                {
                    tiles[i, j].Draw(s, GridSize, Color.White);
                }
            }
        }
        public void DrawPaths(SpriteBatch s, int gridSize)
        {
            foreach (Path p in paths)
            {
                p.Draw(s, gridSize);
            }
        }
    }
}
