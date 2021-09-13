using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Towers {
    class FlameThrower : Tower {
        public FlameThrower(int x, int y, int hotkeyNumber) : base(x, y, 1f, hotkeyNumber) {
            TowerColor = Color.OrangeRed;
            _damageTiles = new List<Point>();
            _flameList = new List<Projectile>();
            Type = Utility.TowerType.FlameThrower;
            Damage = 2;
            _minRange = 0;
            _maxRange = 2;
            TowerInfo = "Flame Turret\nMin. Range: " + _minRange + "\nMax. Range: " + _maxRange + "\nDamage: " + Damage + "\nLights minions on fire\nuse multiple to stack DMG";
        }

        public float BurnTime => 5;

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (!_disabled && _attackTimer.IsExpired) {
                GenerateDamageTiles();
                CheckDamageTiles();
                GenerateFireEffect();
                _attackTimer.Reset();
            }

            for (int x = _flameList.Count - 1; x >= 0; x--) {
                if (_flameList[x].MarkedForDeletion)
                    _flameList.Remove(_flameList[x]);
            }
            foreach (Projectile f in _flameList) {
                f.Update(gameTime);
                // bullets start at the center, therefore an extra 0.5f is added to the range
                if (f.DistanceTravelled > _maxRange + 0.5f)
                    f.MarkedForDeletion = true;
            }
        }
        public override void Draw(SpriteBatch s) {
            base.Draw(s);
        }

        public void DrawFireEffect(SpriteBatch s) {
            foreach (Projectile f in _flameList)
                f.Draw(s);
        }

        private void GenerateFireEffect() {
            for (int i = 0; i <= _amountOfFlamesPerCycle; i++) {
                Vector2 randomDirection = RandomDirectionGenerator();
                Projectile flame = new Projectile(new Vector2(this.X + 0.5f, this.Y + 0.5f), randomDirection, Color.Orange, 3.5F);
                _flameList.Add(flame);
            }
        }

        private Vector2 RandomDirectionGenerator() {
            Vector2 randomDirection = new Vector2((float)Utility.random.NextDouble(), (float)Utility.random.NextDouble());
            int r = Utility.random.Next(0, 5);
            if (r == 2)
                randomDirection.X = -randomDirection.X;
            else if (r == 3)
                randomDirection.Y = -randomDirection.Y;
            else if (r == 4)
                randomDirection = -randomDirection;
            return randomDirection;
        }
        public void GenerateDamageTiles() {
            _damageTiles.Clear();
            for(int i = -_maxRange; i <= _maxRange; i++) {
                for(int j = -_maxRange; j <= _maxRange; j++) {
                    _damageTiles.Add(new Point(X + i, Y + j));
                }
            }
        }
        public void CheckDamageTiles() {
            foreach (Path p in Utility.Board.Paths) {
                foreach(Minion m in p.MinionList) {
                    if (!RangeChecker(m.Position.X, m.Position.Y, _maxRange))
                        continue;
                    foreach (Point point in _damageTiles) {
                        if (m.IsInTile(point.X, point.Y)) {
                            m.AddFireStack(this);
                        }
                    }
                }
            }
        }

        private const int _amountOfFlamesPerCycle = 200;
        private List<Projectile> _flameList;
        List<Point> _damageTiles;
    }
}
