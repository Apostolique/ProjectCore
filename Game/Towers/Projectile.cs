using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject.Towers {
    class Projectile {
        public Projectile(Vector2 position, Vector2 direction, Color color, float speed) {
            _position = position;
            _direction = direction;
            _color = color;
            _speed = speed;
            MarkedForDeletion = false;
        }

        public void Update(GameTime gameTime) {
            _position += _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _distanceTravelled += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch s) {
            Point2 tempP = new Point2(Utility.GameToScreen(_position.X), Utility.GameToScreen(_position.Y));
            s.DrawCircle(new CircleF(tempP, _radius * Utility.Board.GridSize), 8, _color, _radius * Utility.Board.GridSize);
            s.DrawCircle(new CircleF(tempP, _radius * Utility.Board.GridSize), 8, Color.Black, 1);
        }

        public float DistanceTravelled => _distanceTravelled;
        public float Radius => _radius;
        public Vector2 Position => _position;
        public bool MarkedForDeletion { get; set; }

        private const float _radius = 0.1f;
        private float _speed;
        private Vector2 _position, _direction;
        private float _distanceTravelled;
        private Color _color;
    }
}
