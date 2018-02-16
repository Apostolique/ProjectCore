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
        Vector2 position;
        public Sidebar(Vector2 iPosition)
        {
            position = iPosition;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch s)
        {

        }

        public void DrawTowerInfo(SpriteBatch s, Tower tower)
        {
            s.DrawString(Utility.assetManager.GetFont("jura"), tower.towerInfo, new Vector2(position.X, position.Y + towerInfoOffset), Color.White, 0f, Vector2.Zero, 0.5F, SpriteEffects.None, 0);
        }

        public void DrawPlayTime(SpriteBatch s)
        {
            string time = Utility.tdGameTimer.Minutes.ToString("D2") + ":" + Utility.tdGameTimer.Seconds.ToString("D2");
            s.DrawString(Utility.assetManager.GetFont("jura"), time, Vector2.Zero, Color.Red, 0f, Vector2.Zero, 0.5F, SpriteEffects.None, 0);
        }
    }
}
