using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameJamProject.Towers;
using System;
using MonoGame.Extended.BitmapFonts;

namespace MonoGameJamProject.UI
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
                DrawRangeIndicators(s, new Point(input.MouseToGameGrid().X, input.MouseToGameGrid().Y), tower, 0.3F);
                s.FillRectangle(new RectangleF(input.MouseGridPosition().X * Utility.board.GridSize, input.MouseGridPosition().Y * Utility.board.GridSize, Utility.board.GridSize, Utility.board.GridSize), Color.White * 0.6F);
            }
            else s.FillRectangle(new RectangleF(input.MouseGridPosition().X * Utility.board.GridSize, input.MouseGridPosition().Y * Utility.board.GridSize, Utility.board.GridSize, Utility.board.GridSize), Color.Red * 0.7F);
        }
        public void DrawRangeIndicators(SpriteBatch s, Point origin, Tower tower, float transparency = 0.1f)
        {
            float horizontal0 = origin.X - tower.MaximumRange;
            float horizontal1 = origin.X - tower.MinimumRange;
            float horizontal2 = origin.X + tower.MinimumRange + 1;
            float horizontal3 = origin.X + tower.MaximumRange + 1;
            float vertical0 = origin.Y - tower.MaximumRange;
            float vertical1 = origin.Y - tower.MinimumRange;
            float vertical2 = origin.Y + tower.MinimumRange + 1;
            float vertical3 = origin.Y + tower.MaximumRange + 1;

            //Draws the minimum range.
            RectangleF minRange = new RectangleF(Utility.GameToScreen(horizontal1), Utility.GameToScreen(vertical1), (horizontal2 - horizontal1) * Utility.board.GridSize, (vertical2 - vertical1) * Utility.board.GridSize);


            RectangleF maxRangeTop = new RectangleF(Utility.GameToScreen(horizontal0), Utility.GameToScreen(vertical0), (horizontal3 - horizontal0) * Utility.board.GridSize, (vertical1 - vertical0) * Utility.board.GridSize);
            RectangleF maxRangeBot = new RectangleF(Utility.GameToScreen(horizontal0), Utility.GameToScreen(vertical2), (horizontal3 - horizontal0) * Utility.board.GridSize, (vertical3 - vertical2) * Utility.board.GridSize);
            RectangleF maxRangeLeft = new RectangleF(Utility.GameToScreen(horizontal0), Utility.GameToScreen(vertical1), (horizontal1 - horizontal0) * Utility.board.GridSize, (vertical2 - vertical1) * Utility.board.GridSize);
            RectangleF maxRangeRight = new RectangleF(Utility.GameToScreen(horizontal2), Utility.GameToScreen(vertical1), (horizontal3 - horizontal2) * Utility.board.GridSize, (vertical2 - vertical1) * Utility.board.GridSize);

            s.FillRectangle(minRange, Color.Red * transparency);
            s.FillRectangle(maxRangeTop, Color.Green * transparency);
            s.FillRectangle(maxRangeBot, Color.Green * transparency);
            s.FillRectangle(maxRangeLeft, Color.Green * transparency);
            s.FillRectangle(maxRangeRight, Color.Green * transparency);
        }
        
    }
}
