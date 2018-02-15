using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject.Towers
{
    class ShotgunPellet
    {
        private const float bulletRadius = 0.1f;
        private Vector2 position, velocity;
        public ShotgunPellet(Vector2 iPos, Vector2 iVelocity)
        {
            position = iPos;
            velocity = iVelocity;
        }

        public void Update(GameTime gameTime)
        {
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch s)
        {
            s.DrawCircle(new CircleF(new Point2(Utility.GameToScreen(position.X), Utility.GameToScreen(position.Y)), bulletRadius * Utility.board.GridSize), 16, Color.SandyBrown, 11f);
        }
    }
}
