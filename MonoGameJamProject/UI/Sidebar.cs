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
        private Vector2 towerInfoPosition;
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
            s.DrawString(Utility.assetManager.GetFont("jura"), tower.towerInfo, towerInfoPosition, Color.White);
        }
    }
}
