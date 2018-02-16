using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameJamProject.Towers;
using System;

namespace MonoGameJamProject
{
    class HUD
    {
        Input input;
        public HUD(Input iInput)
        {
            input = iInput;
        }
        public void DrawPlacementIndicator(SpriteBatch s, Tower tower, bool isValidPosition)
        {
            if (isValidPosition)
            {
                s.FillRectangle(new RectangleF(input.MouseGridPosition().X * Utility.board.GridSize, input.MouseGridPosition().Y * Utility.board.GridSize, Utility.board.GridSize, Utility.board.GridSize), Color.White * 0.5F);
                DrawRangeIndicators(s, new Point(input.MouseToGameGrid().X, input.MouseToGameGrid().Y), tower, 0.4f);
            }
            else s.FillRectangle(new RectangleF(input.MouseGridPosition().X * Utility.board.GridSize, input.MouseGridPosition().Y * Utility.board.GridSize, Utility.board.GridSize, Utility.board.GridSize), Color.Red * 0.7F);
        }
        public void DrawRangeIndicators(SpriteBatch s, Point origin, Tower tower, float transparency = 0.1f)
        {
            //Draws the minimum range.
            RectangleF minRange = new RectangleF(Utility.GameToScreen(tower.X - tower.MinimumRange), Utility.GameToScreen(tower.Y - tower.MinimumRange), (tower.MinimumRange * 2 + 1) * Utility.board.GridSize, (tower.MinimumRange * 2 + 1) * Utility.board.GridSize);
            s.FillRectangle(minRange, Color.Red * transparency);
        }
        public void DrawPlayTime(SpriteBatch s)
        {
            string time = Utility.tdGameTimer.Minutes.ToString("D2") + ":" + Utility.tdGameTimer.Seconds.ToString("D2");
            s.DrawString(Utility.assetManager.GetFont("jura"), time, Vector2.Zero, Color.Red);
        }
    }
}
