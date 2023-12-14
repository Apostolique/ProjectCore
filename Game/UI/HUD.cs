using Apos.Shapes;
using GameProject.Towers;
using Microsoft.Xna.Framework;

namespace GameProject.UI {
    class HUD() {
        public static void DrawPlacementIndicator(ShapeBatch sb, Tower tower, bool isValidPosition) {
            if (isValidPosition) {
                DrawRangeIndicators(sb, Utility.MouseToGameGrid(), tower, 0.3F);
                sb.FillRectangle(Utility.MouseGridBoardPosition().ToVector2(), new Vector2(Utility.Board.GridSize, Utility.Board.GridSize), Color.White * 0.6f);
            }
            else sb.FillRectangle(Utility.MouseGridBoardPosition().ToVector2(), new Vector2(Utility.Board.GridSize, Utility.Board.GridSize), Color.Red * 0.7f);
        }
        public static void DrawRangeIndicators(ShapeBatch sb, Point origin, Tower tower, float transparency = 0.1f) {
            float horizontal0 = origin.X - tower.MaximumRange;
            float horizontal1 = origin.X - tower.MinimumRange;
            float horizontal2 = origin.X + tower.MinimumRange + 1;
            float horizontal3 = origin.X + tower.MaximumRange + 1;
            float vertical0 = origin.Y - tower.MaximumRange;
            float vertical1 = origin.Y - tower.MinimumRange;
            float vertical2 = origin.Y + tower.MinimumRange + 1;
            float vertical3 = origin.Y + tower.MaximumRange + 1;

            //Draws the minimum range.
            Vector2 minRange = new Vector2(Utility.GameToScreen(horizontal1), Utility.GameToScreen(vertical1));
            Vector2 minRangeSize = new Vector2((horizontal2 - horizontal1) * Utility.Board.GridSize, (vertical2 - vertical1) * Utility.Board.GridSize);

            Vector2 maxRangeTop = new Vector2(Utility.GameToScreen(horizontal0), Utility.GameToScreen(vertical0));
            Vector2 maxRangeTopSize = new Vector2((horizontal3 - horizontal0) * Utility.Board.GridSize, (vertical1 - vertical0) * Utility.Board.GridSize);
            Vector2 maxRangeBot = new Vector2(Utility.GameToScreen(horizontal0), Utility.GameToScreen(vertical2));
            Vector2 maxRangeBotSize = new Vector2((horizontal3 - horizontal0) * Utility.Board.GridSize, (vertical3 - vertical2) * Utility.Board.GridSize);
            Vector2 maxRangeLeft = new Vector2(Utility.GameToScreen(horizontal0), Utility.GameToScreen(vertical1));
            Vector2 maxRangeLeftSize = new Vector2((horizontal1 - horizontal0) * Utility.Board.GridSize, (vertical2 - vertical1) * Utility.Board.GridSize);
            Vector2 maxRangeRight = new Vector2(Utility.GameToScreen(horizontal2), Utility.GameToScreen(vertical1));
            Vector2 maxRangeRightSize = new Vector2((horizontal3 - horizontal2) * Utility.Board.GridSize, (vertical2 - vertical1) * Utility.Board.GridSize);

            sb.FillRectangle(minRange, minRangeSize, Color.Red * transparency);
            sb.FillRectangle(maxRangeTop, maxRangeTopSize, Color.Green * transparency);
            sb.FillRectangle(maxRangeBot, maxRangeBotSize, Color.Green * transparency);
            sb.FillRectangle(maxRangeLeft, maxRangeLeftSize, Color.Green * transparency);
            sb.FillRectangle(maxRangeRight, maxRangeRightSize, Color.Green * transparency);
        }
    }
}
