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

        public Board(int iWidth, int iHeight) {
            Width = iWidth;
            Height = iHeight;

            paths = new List<Path>();
            tiles = new Tile[FullWidth, FullHeight];

            for (int i = 0; i < FullWidth; i++) {
                for (int j = 0; j < FullHeight; j++) {
                    tiles[i, j] = new Tile(i, j);
                }
            }
        }
        public int Width {
            get;
            set;
        }
        public int Height {
            get;
            set;
        }
        public int FullWidth {
            get => Width + 2;
        }
        public int FullHeight {
            get => Height + 2;
        }
        public int GridSize => Math.Min(Utility.Window.ClientBounds.Width / Width, Utility.Window.ClientBounds.Height / Height);
        public void GeneratePath() {
            paths.Add(new Path());
        }
        private List<HashSet<Tile>> findIslands() {
            //TODO: This function finds zones of continuous open tiles. Call that an Island.
            //      Right now we start by marking all the tiles as open.
            //      Then we pick one of them, switch it to closed, and
            //      start exploring the zone.
            //      Something something, we are done when all tiles are closed.
            HashSet<Tile> closedTiles = new HashSet<Tile>();
            HashSet<Tile> openTiles = new HashSet<Tile>();
            List<HashSet<Tile>> islands = new List<HashSet<Tile>>();

            //Start by adding all the tiles.
            for (int i = 0; i < FullWidth; i++) {
                for (int j = 0; j < FullHeight; j++) {
                    openTiles.Add(tiles[i, j]);
                }
            }

            Tile t = openTiles.First();
            openTiles.Remove(t);
            closedTiles.Add(t);


            
            return islands;
        }
        private List<Tile> findNeighbors(Tile t) {
            List<Tile> neighbors = new List<Tile>();
            if (t.X - 1 >= 0) {
                neighbors.Add(tiles[t.X - 1, t.Y]);
            }
            if (t.X + 1 < Width + 2) {
                neighbors.Add(tiles[t.X + 1, t.Y]);
            }
            if (t.Y - 1 >= 0) {
                neighbors.Add(tiles[t.X, t.Y - 1]);
            }
            if (t.Y + 1 < Width + 2) {
                neighbors.Add(tiles[t.X, t.Y + 1]);
            }

            return neighbors;
        }
        public void Draw(SpriteBatch s) {
            s.FillRectangle(new Rectangle(0, 0, GridSize * Width, GridSize * Height), new Color(10, 10, 10));

            for (int i = 1; i < FullWidth - 1; i++) {
                for (int j = 1; j < FullHeight - 1; j++) {
                    tiles[i, j].Draw(s, GridSize);
                }
            }
        }
    }
}
