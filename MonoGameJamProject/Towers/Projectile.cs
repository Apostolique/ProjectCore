using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject.Towers
{
    class Projectile
    {
        private const float radius = 0.1f;
        private const float speed = 3f;
        private Vector2 position, direction;
        private float distanceTravelled;
        private Color color;
        public Projectile(Vector2 iPos, Vector2 iDirection, Color iColour)
        {
            position = iPos;
            direction = iDirection;
            color = iColour;
            MarkedForDeletion = false;
        }
        public void Update(GameTime gameTime)
        {
            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            distanceTravelled += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch s)
        {
            Point2 tempP = new Point2(Utility.GameToScreen(position.X), Utility.GameToScreen(position.Y));
            s.DrawCircle(new CircleF(tempP, radius * Utility.board.GridSize), 8, color, radius * Utility.board.GridSize);
            s.DrawCircle(new CircleF(tempP, radius * Utility.board.GridSize), 8, Color.Black, 1);
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
