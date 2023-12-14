using FontStashSharp;
using GameProject.Towers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject.UI {
    class Sidebar(Vector2 offset) {
        public void Update(GameTime gameTime) {
            _position = new Vector2(Utility.Window.ClientBounds.Width - _offset.X, _offset.Y);
            _effectTick += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_effectTick >= 2000)
                _effectTick = 0;
        }

        public void Draw(SpriteBatch s) {
            DrawPlayTime(s);

            Color placeTowersColor = Color.Gray;
            if (Utility.MaxTowers - Utility.TowerList.Count > 0) {
                if (_effectTick > 1000) {
                    placeTowersColor = Color.Red;
                }
                else
                    placeTowersColor = Color.White;
            }
            s.DrawString(Utility.AssetManager.GetFont(22), "Lives: " + Utility.NumberOfLives, new Vector2(_position.X, _position.Y + 30), Color.Green);
            s.DrawString(Utility.AssetManager.GetFont(22), "Difficulty: " + Utility.GameDifficulty, new Vector2(_position.X, _position.Y + 60), Color.Aqua);
            s.DrawString(Utility.AssetManager.GetFont(22), "Kills: " + Utility.TotalNumberOfKills, new Vector2(_position.X, _position.Y + 90), Color.White);
            s.DrawString(Utility.AssetManager.GetFont(22), "Score: " + Utility.Score, new Vector2(_position.X, _position.Y + 120), Color.White);
            s.DrawString(Utility.AssetManager.GetFont(22), "Placeable towers: " + (Utility.MaxTowers - Utility.TowerList.Count), new Vector2(_position.X, _position.Y + 150), placeTowersColor);

            s.DrawString(Utility.AssetManager.GetFont(20), "Music by YERZMYEY", new Vector2(_position.X, _position.Y + 440), Color.LightGray);

        }

        public void DrawTowerInfo(SpriteBatch s, Tower tower) {
            s.FillRectangle(new RectangleF(_position.X + 20, _position.Y + _towerInfoOffset, Utility.Board.GridSize, Utility.Board.GridSize), tower.TowerColor);
            s.DrawRectangle(new RectangleF(_position.X + 20, _position.Y + _towerInfoOffset, Utility.Board.GridSize, Utility.Board.GridSize), Color.White, 1);
            s.DrawString(Utility.AssetManager.GetFont(16), tower.TowerInfo, new Vector2(_position.X, _position.Y + (_towerInfoOffset + Utility.Board.GridSize + 5)), Color.Yellow);
        }

        public void DrawPlayTime(SpriteBatch s) {
            string time = Utility.TDGameTimer.Minutes.ToString("D2") + ":" + Utility.TDGameTimer.Seconds.ToString("D2");
            s.DrawString(Utility.AssetManager.GetFont(22), time, _position, Color.Red);
        }

        private readonly float _towerInfoOffset = 190;
        float _effectTick = 0;
        Vector2 _position, _offset = offset;
    }
}
