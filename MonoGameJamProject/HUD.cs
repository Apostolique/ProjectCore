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
        public HUD(Input iInput)
        {
            input = iInput;
            tdGameTimer = TimeSpan.Zero;
        }
        public void Update(GameTime gameTime)
        {
            tdGameTimer += gameTime.ElapsedGameTime;
        }
        public void DrawPlacementIndicator(SpriteBatch s, int minimumRange, bool isValidPosition)
        {
            if (isValidPosition)
            {
                s.FillRectangle(new RectangleF(input.MouseGridPosition().X * Utility.board.GridSize, input.MouseGridPosition().Y * Utility.board.GridSize, Utility.board.GridSize, Utility.board.GridSize), Color.White * 0.5F);
                DrawRangeIndicators(s, new Point(input.MouseToGameGrid().X, input.MouseToGameGrid().Y), minimumRange, 0.4f);
            }
            else s.FillRectangle(new RectangleF(input.MouseGridPosition().X * Utility.board.GridSize, input.MouseGridPosition().Y * Utility.board.GridSize, Utility.board.GridSize, Utility.board.GridSize), Color.Red * 0.7F);
        }
        public void DrawRangeIndicators(SpriteBatch s, Point origin, int minimumRange, float transparency = 0.1f)
        {
            for (int i = -minimumRange; i <= minimumRange; i++)
            {
                for (int j = -minimumRange; j <= minimumRange; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    s.FillRectangle(new RectangleF(Utility.GameToScreen(origin.X + i), Utility.GameToScreen(origin.Y + j), Utility.board.GridSize, Utility.board.GridSize), Color.Red * transparency);
                }
            }
        }
        public void DrawPlayTime(SpriteBatch s)
        {
            string time = tdGameTimer.Minutes.ToString("D2") + ":" + tdGameTimer.Seconds.ToString("D2");
            s.DrawString(Utility.mainFont, time, Vector2.Zero, Color.Red);
        }
    }
}
