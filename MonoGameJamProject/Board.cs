using System;
using System.Collections.Generic;
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
        public int Width {
            get;
            set;
        }
        public int Height {
            get;
            set;
        }
        public int BoardRatio {
            get => Math.Min(Utility.Window.ClientBounds.Width / Width, Utility.Window.ClientBounds.Height / Height);
        }
        public Board(int iWidth, int iHeight) {
            Width = iWidth;
            Height = iHeight;
        }

        public void Draw(SpriteBatch s) {
            s.FillRectangle(new Rectangle(0, 0, BoardRatio * Width, BoardRatio * Height), new Color(10, 10, 10));

            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    s.DrawRectangle(new Rectangle(i * BoardRatio, j * BoardRatio, BoardRatio, BoardRatio), Color.White, 0.02f * BoardRatio);
                }
            }
        }
    }
}
