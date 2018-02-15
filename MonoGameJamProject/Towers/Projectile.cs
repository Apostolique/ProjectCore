using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject.Towers
{
    class Projectile
    {
        private const float radius = 0.1f;
        float speed;
        private Vector2 position, direction;
        private float distanceTravelled;
        private Color colour;
        public Projectile(Vector2 iPos, Vector2 iDirection, Color iColour, float iSpeed = 7f)
        {
            position = iPos;
            direction = iDirection;
            colour = iColour;
            speed = iSpeed;
            MarkedForDeletion = false;
        }
        public void Update(GameTime gameTime)
        {
            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            distanceTravelled += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch s)
        {
            s.DrawCircle(new CircleF(new Point2(Utility.GameToScreen(position.X), Utility.GameToScreen(position.Y)), radius * Utility.board.GridSize), 8, colour, radius * Utility.board.GridSize);
        }

        public float DistanceTravelled
        {
            get { return distanceTravelled; }
        }

        public float Radius
        {
            get { return radius; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public bool MarkedForDeletion
        {
            get; set;
        }
    }
}
