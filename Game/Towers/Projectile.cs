using Apos.Shapes;
using Microsoft.Xna.Framework;

namespace GameProject.Towers {
    class Projectile(Vector2 position, Vector2 direction, Color color, float speed) {
        public void Update(GameTime gameTime) {
            _position += _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _distanceTraveled += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(ShapeBatch sb) {
            Vector2 tempP = new Vector2(Utility.GameToScreen(_position.X), Utility.GameToScreen(_position.Y));
            sb.DrawCircle(tempP, Radius * Utility.Board.GridSize, _color, Color.Black, 1f);
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
