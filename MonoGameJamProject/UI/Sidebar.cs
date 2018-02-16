using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameJamProject.Towers;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;

namespace MonoGameJamProject.UI
{
    class Sidebar
    {
        private float towerInfoOffset;
        Vector2 position, offset;
        public Sidebar(Vector2 iOffset)
        {
            offset = iOffset;
            towerInfoOffset = 170;
        }

        public void Update()
        {
            position = new Vector2(Utility.Window.ClientBounds.Width - offset.X, offset.Y);
        }

        public void Draw(SpriteBatch s)
        {
            DrawPlayTime(s);
            // Draw amount of kills
            s.DrawString(Utility.assetManager.GetFont("Jura"), "Kills: " + Utility.totalNumberOfKills, new Vector2(position.X, position.Y + 30), Color.White, 0f, Vector2.Zero, 0.5F, SpriteEffects.None, 0);
            // Draw the number of paths
            s.DrawString(Utility.assetManager.GetFont("Jura"), "Path amount: " + Utility.board.Paths.Count, new Vector2(position.X, position.Y + 60), Color.White, 0f, Vector2.Zero, 0.5F, SpriteEffects.None, 0);
            // Draw the time until next path: (ADD THE TIMER HERE LATER)
            s.DrawString(Utility.assetManager.GetFont("Jura"), "Next Path: " + "UND" , new Vector2(position.X, position.Y + 90), Color.White, 0f, Vector2.Zero, 0.5F, SpriteEffects.None, 0);
            // Draw the difficulty
            s.DrawString(Utility.assetManager.GetFont("Jura"), "Difficulty: " + "UND", new Vector2(position.X, position.Y + 120), Color.White, 0f, Vector2.Zero, 0.5F, SpriteEffects.None, 0);

        }

        public void DrawTowerInfo(SpriteBatch s, Tower tower)
        {
            s.DrawString(Utility.assetManager.GetFont("Jura"), tower.towerInfo, new Vector2(position.X, position.Y + towerInfoOffset), Color.Yellow, 0f, Vector2.Zero, 0.35F, SpriteEffects.None, 0);
        }

        public void DrawPlayTime(SpriteBatch s)
        {
            string time = Utility.tdGameTimer.Minutes.ToString("D2") + ":" + Utility.tdGameTimer.Seconds.ToString("D2");
            s.DrawString(Utility.assetManager.GetFont("Jura"), time, position, Color.Red, 0f, Vector2.Zero, 0.5F, SpriteEffects.None, 0);
        }
    }
}
