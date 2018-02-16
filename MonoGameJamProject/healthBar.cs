using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject
{
    class HealthBar
    {
        Vector2 position;
        RectangleF healthBarRectangle;
        Color fillColor, outlineColor;
        private int healthBarHeight = 10, healthBarWidth = 30;
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
                    healthBarHeight = 10;
                    healthBarWidth = 25;
                    break;
                case Utility.MinionType.slow:
                    maxHP = Utility.slowMinionHP;
                    healthBarHeight = 12;
                    healthBarWidth = 35;
                    break;
                case Utility.MinionType.boss:
                    maxHP = Utility.bossMinionHp;
                    healthBarHeight= 20;
                    healthBarWidth = 50;
                    break;
            }
            position = new Vector2(Utility.GameToScreen(owner.Position.X - owner.Radius), Utility.GameToScreen(owner.Position.Y - owner.Radius));
            healthBarRectangle = new RectangleF(position.X, position.Y, healthBarWidth, healthBarHeight);
            fillColor = Color.DarkRed;
            outlineColor = Color.Black;
        }

        public void Update()
        {
            position = new Vector2(Utility.GameToScreen(owner.Position.X - owner.Radius), Utility.GameToScreen(owner.Position.Y - owner.Radius * 2));
            healthBarRectangle = new RectangleF(position.X, position.Y, healthBarWidth * (owner.HP / maxHP), healthBarHeight);
        }

        public void Draw(SpriteBatch s)
        {
            s.FillRectangle(healthBarRectangle, fillColor);
            DrawOutline(s);
        }

        private void DrawOutline(SpriteBatch s)
        {
            // Draw Left
            s.DrawLine(position.X, position.Y, position.X, position.Y + healthBarHeight, outlineColor, outlineThickness);
            // Draw Right
            s.DrawLine(position.X + healthBarWidth, position.Y, position.X + healthBarWidth, position.Y + healthBarHeight, outlineColor, outlineThickness);
            // Draw Top
            s.DrawLine(position.X, position.Y, position.X + healthBarWidth, position.Y, outlineColor, outlineThickness);
            // Draw Bottom
            s.DrawLine(position.X, position.Y + healthBarHeight, position.X + healthBarWidth, position.Y + healthBarHeight, outlineColor, outlineThickness);
        }
    }
}
