using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGameJamProject
{
    class HUD
    {
        TimeSpan tdGameTimer;
        Input input;
        int gridSize;
        public HUD(Input iInput, int iGridSize)
        {
            input = iInput;
            gridSize = iGridSize;
            tdGameTimer = TimeSpan.Zero;
        }

        public void Update(GameTime gameTime)
        {
            tdGameTimer += gameTime.ElapsedGameTime;
        }
        
        public void DrawPlacementIndicator(SpriteBatch s, int minimumRange)
        {
            s.FillRectangle(new RectangleF(input.MouseGridPosition(gridSize).X * gridSize, input.MouseGridPosition(gridSize).Y * gridSize, gridSize, gridSize), Color.White * 0.5F);
            DrawRangeIndicators(s, new Point(input.MouseToGameGrid(gridSize).X, input.MouseToGameGrid(gridSize).Y), minimumRange, 0.4f);
        }

        public void DrawRangeIndicators(SpriteBatch s, Point origin, int minimumRange, float transparency = 0.1f)
        {
            for (int i = -minimumRange; i <= minimumRange; i++)
            {
                for (int j = -minimumRange; j <= minimumRange; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    s.FillRectangle(new RectangleF(Utility.GameToScreen(origin.X + i, gridSize), Utility.GameToScreen(origin.Y + j, gridSize), gridSize, gridSize), Color.Red * transparency);
                }
            }
        }

        public void DrawPlayTime(SpriteBatch s)
        {
            string time = tdGameTimer.Minutes.ToString() + " : " + tdGameTimer.Seconds.ToString();
            s.DrawString(Utility.mainFont, time, Vector2.Zero, Color.Red);
        }

        public int GridSize
        {
            get { return gridSize; }
            set { gridSize = value; }
        }
    }
}
