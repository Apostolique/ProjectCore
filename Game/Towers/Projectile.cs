using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject.Towers {
    class Projectile(Vector2 position, Vector2 direction, Color color, float speed) {
        public void Update(GameTime gameTime) {
            _position += _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _distanceTraveled += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch s) {
            Point2 tempP = new Point2(Utility.GameToScreen(_position.X), Utility.GameToScreen(_position.Y));
            s.DrawCircle(new CircleF(tempP, Radius * Utility.Board.GridSize), 8, _color, Radius * Utility.Board.GridSize);
            s.DrawCircle(new CircleF(tempP, Radius * Utility.Board.GridSize), 8, Color.Black, 1);
        }

        public float DistanceTraveled => _distanceTraveled;
        public static float Radius => 0.1f;
        public Vector2 Position => _position;
        public bool MarkedForDeletion { get; set; } = false;

        private readonly float _speed = speed;
        private Vector2 _position = position, _direction = direction;
        private float _distanceTraveled;
        private Color _color = color;
    }
}
