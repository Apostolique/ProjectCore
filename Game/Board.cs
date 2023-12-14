using System;
using System.Collections.Generic;
using System.Linq;
using Apos.Shapes;
using Microsoft.Xna.Framework;

namespace GameProject {
    /// <summary>
    /// Goal: Board, not actually sure what it will handle...
    /// </summary>
    class Board {
        public Board(int width, int height) {
            Width = width;
            Height = height;

            Paths = [];
            OldPaths = [];
            _tiles = new Tile[FullWidth, FullHeight];

            _bossTimer = new CoolDownTimer(60);
            _bossTimer.Reset();

            for (int i = 0; i < FullWidth; i++) {
                for (int j = 0; j < FullHeight; j++) {
                    _tiles[i, j] = new Tile(i, j);
                }
            }
            CacheGridSize();
        }

        public List<Path> Paths { get; set; }
        public List<Path> OldPaths { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int FullWidth => Width + 2;
        public int FullHeight => Height + 2;
        public int GridSize => _gridSize;

        public void Update(GameTime gameTime) {
            _bossTimer.Update(gameTime);
            CacheGridSize();
            for (int i = Paths.Count - 1; i >= 0; i--) {
                Paths[i].Update(gameTime);
                if (Paths[i].Sequence == Path.Animation.Despawn) {
                    OldPaths.Add(Paths[i]);
                    Paths.RemoveAt(i);
                }
            }
            for (int i = OldPaths.Count - 1; i >= 0; i--) {
                OldPaths[i].Update(gameTime);
                if (OldPaths[i].Done) {
                    OldPaths.RemoveAt(i);
                }
            }
            if (Paths.Count < Utility.GameDifficulty / 4 + 1) {
                GeneratePath();
            }
            if (_bossTimer.IsExpired) {
                Paths[0].AddMinion(new Minion(0, 0, Utility.MinionType.Boss));
                _bossTimer.Reset();
            }
        }
        public void CacheGridSize() {
            _gridSize = Math.Min((Utility.Window.ClientBounds.Width - Utility.SidebarWidth) / Width, Utility.Window.ClientBounds.Height / Height);
        }
        public Tile GetTile(Point p) {
            return _tiles[p.X, p.Y];
        }
        public void ClearPaths() {
            foreach (Path p in Paths) {
                p.Despawn();
            }
        }
        public void ResetPaths() {
            Paths.Clear();
            OldPaths.Clear();
            _bossTimer.Reset();
        }
        public void GeneratePath() {
            Path newPath = new Path();

            int preferredSize = (int)Math.Sqrt(FullWidth * FullHeight);
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

            do {
                HashSet<Tile> nextTiles = FindNeighbors(t);
                RemoveStartAndEnd(ref nextTiles);
                List<TileValue> nextTileSort = [];
                foreach (Tile i in nextTiles) {
                    nextTileSort.Add(new TileValue(i, GetTileValue(i, startTile, length, preferredSize, newPath)));
                }
                nextTileSort.Sort((v1, v2) => v1.Value.CompareTo(v2.Value));
                foreach (TileValue tv in nextTileSort) {
                }
                t = nextTileSort.Last().Tile;
                newPath.Add(t);
                length++;
            } while (t != startTile && !IsEdge(t));

            Paths.Add(newPath);
        }
        public bool IsTileOnPath(Tile tile) {
            foreach (Path p in Paths) {
                if (p.Contains(tile)) {
                    return true;
                }
            }
            foreach (Path p in OldPaths) {
                if (p.Contains(tile)) {
                    return true;
                }
            }
            return false;
        }
        private bool IsEdge(Tile tile) {
            HashSet<Tile> edges = GetEdgeTiles();
            return edges.Contains(tile);
        }
        private int GetTileValue(Tile t, Tile start, int length, int preferredSize, Path p) {
            int tileValue = 0;

            bool isEdge = GetEdgeTiles().Contains(t);

            Vector2 relativeCenter = new Vector2(0.5f, 0.5f);
            Vector2 relativeStart = new Vector2((start.X + 0.5f) / FullWidth, (start.Y + 0.5f) / FullHeight);
            Vector2 relativeMapping = new Vector2((t.X + 0.5f) / FullWidth, (t.Y + 0.5f) / FullHeight);

            int centerDistance = (int)(Vector2.Distance(relativeCenter, relativeMapping) * 100f);
            int startDistance = (int)(Vector2.Distance(relativeStart, relativeMapping) * 50f);

            if (isEdge && length > preferredSize) {
                tileValue += Utility.random.Next(50, 120);
            } else if (!isEdge && length > preferredSize) {
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
        private HashSet<Tile> GetEdgeTiles() {
            HashSet<Tile> edgeTiles = [];

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
        private void RemoveEdges(ref HashSet<Tile> tileSet) {
            foreach (Tile t in GetEdgeTiles()) {
                tileSet.Remove(t);
            }
        }
        private void RemoveCorners(ref HashSet<Tile> tileSet) {
            tileSet.Remove(_tiles[0, 0]);
            tileSet.Remove(_tiles[0, FullHeight - 1]);
            tileSet.Remove(_tiles[FullWidth - 1, 0]);
            tileSet.Remove(_tiles[FullWidth - 1, FullHeight - 1]);
        }
        private void RemoveStartAndEnd(ref HashSet<Tile> tileSet) {
            foreach (Path p in Paths) {
                tileSet.Remove(p.First());
                tileSet.Remove(p.Last());
            }
        }
        private HashSet<Tile> GetRow(int row) {
            HashSet<Tile> rowTiles = [];
            for (int i = 0; i < FullWidth; i++) {
                rowTiles.Add(_tiles[i, row]);
            }

            return rowTiles;
        }
        private HashSet<Tile> GetColumn(int column) {
            HashSet<Tile> rowTiles = [];
            for (int i = 0; i < FullHeight; i++) {
                rowTiles.Add(_tiles[column, i]);
            }

            return rowTiles;
        }
        private HashSet<Tile> FindNeighbors(Tile t) {
            HashSet<Tile> neighbors = [];
            if (t.X - 1 >= 0) {
                neighbors.Add(_tiles[t.X - 1, t.Y]);
            }
            if (t.X + 1 < FullWidth) {
                neighbors.Add(_tiles[t.X + 1, t.Y]);
            }
            if (t.Y - 1 >= 0) {
                neighbors.Add(_tiles[t.X, t.Y - 1]);
            }
            if (t.Y + 1 < FullHeight) {
                neighbors.Add(_tiles[t.X, t.Y + 1]);
            }

            return neighbors;
        }
        public void Draw(ShapeBatch sb) {
            sb.FillRectangle(new Vector2(0, 0), new Vector2(GridSize * Width, GridSize * Height), new Color(15, 15, 15));

            for (int i = 1; i < FullWidth - 1; i++) {
                for (int j = 1; j < FullHeight - 1; j++) {
                    _tiles[i, j].Draw(sb, Color.White);
                }
            }
            DrawPaths(sb, OldPaths);
            DrawPathLines(sb, OldPaths);
            DrawPaths(sb, Paths);
            DrawPathLines(sb, Paths);
        }
        public static void DrawPaths(ShapeBatch sb, List<Path> paths) {
            foreach (Path p in paths) {
                p.Draw(sb);
            }
        }
        public static void DrawPathLines(ShapeBatch sb, List<Path> paths) {
            foreach (Path p in paths) {
                p.DrawLine(sb);
            }
        }

        struct TileValue(Tile t, int v) {
            public Tile Tile = t;
            public int Value = v;
        }

        readonly Tile[,] _tiles;
        readonly CoolDownTimer _bossTimer;
        int _gridSize = 1;
    }
}
