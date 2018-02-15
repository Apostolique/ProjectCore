using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject.Towers
{
    class ShotgunPellet
    {
        private const float bulletRadius = 0.1f;
        private const float bulletSpeed = 3f;
        private Vector2 position, direction;
        private float distanceTravelled;
        public ShotgunPellet(Vector2 iPos, Vector2 iDirection)
        {
            position = iPos;
            direction = iDirection;
        }
        public void Update(GameTime gameTime)
        {
            position += direction * bulletSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            distanceTravelled += bulletSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch s)
        {
            s.DrawCircle(new CircleF(new Point2(Utility.GameToScreen(position.X), Utility.GameToScreen(position.Y)), bulletRadius * Utility.board.GridSize), 8, Color.SandyBrown, bulletRadius * Utility.board.GridSize);
        }

        public float DistanceTravelled
        {
            get { return distanceTravelled; }
        }

        public float Radius
        {
            get { return bulletRadius; }
        }

        public Vector2 Position
        {
            get { return position; }
        }
    }
}
