using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace MonoGameJamProject
{
    class HealthBar
    {
        Color fillColor, outlineColor;
        private float healthBarWidth, healthBarHeight;
        private const float outlineThickness = 1f;
        int maxHP;
        Minion owner;
        public HealthBar(Minion iOwner)
        {
            owner = iOwner;
            switch (owner.Type)
            {
                case Utility.MinionType.fast:
                    maxHP = Utility.fastMinionHP;
                    healthBarHeight = 0.10f;
                    healthBarWidth = 0.25f;
                    break;
                case Utility.MinionType.slow:
                    maxHP = Utility.slowMinionHP;
                    healthBarHeight = 0.12f;
                    healthBarWidth = 0.35f;
                    break;
                case Utility.MinionType.boss:
                    maxHP = Utility.bossMinionHp;
                    healthBarHeight= 0.20f;
                    healthBarWidth = 0.50f;
                    break;
            }
            fillColor = Color.DarkRed;
            outlineColor = Color.Black;
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch s)
        {
            Vector2 position = new Vector2(Utility.GameToScreen(owner.Position.X - healthBarWidth / 2), Utility.GameToScreen(owner.Position.Y - owner.Radius - healthBarHeight));
            RectangleF healthRectangle = new RectangleF(position.X, position.Y, healthBarWidth * (owner.HP / maxHP) * Utility.board.GridSize, healthBarHeight * Utility.board.GridSize);
            s.FillRectangle(healthRectangle, fillColor);
            DrawOutline(s, position);
            s.DrawString(Utility.assetManager.GetFont("Jura"), owner.StackFlamethrowers.Count.ToString(), new Vector2(healthRectangle.X + healthRectangle.Width, healthRectangle.Y - healthRectangle.Height), Color.Orange, 0f, Vector2.Zero, 0.005f * Utility.board.GridSize, SpriteEffects.None, 0);
        }

        private void DrawOutline(SpriteBatch s, Vector2 position)
        {
            RectangleF healthRectangle = new RectangleF(position.X, position.Y, healthBarWidth * Utility.board.GridSize, healthBarHeight * Utility.board.GridSize);
            s.DrawRectangle(healthRectangle, outlineColor, outlineThickness);
        }
    }
}
