using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Towers {
    class Shotgun : Tower {
        public Shotgun(int x, int y, int hotKeyNumber) : base(x, y, 1.5f, hotKeyNumber) {
            TowerColor = Color.DeepPink;
            Type = Utility.TowerType.Shotgun;
            _bulletList = [];
            _minRange = 1;
            _maxRange = 3;
            Damage = 4;
            TowerInfo = "Shotgun Tower\nMin. Range: " + _minRange + "\nMax. Range: " + _maxRange + "\nDamage: " + Damage + "\nShoots bullets in a spread\ngood for crowds of minions";
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (!_disabled && _attackTimer.IsExpired) {
                TargetMinion();
                if (_targetedMinion != null) {
                    _attackTimer.Reset();
                    ShootAtTargetedMinion();
                }
            }

            for(int x = _bulletList.Count - 1; x >=0; x--) {
                if (_bulletList[x].MarkedForDeletion)
                    _bulletList.Remove(_bulletList[x]);
            }
            foreach(Projectile b in _bulletList) {
                b.Update(gameTime);
                BulletCollisionChecker();
                // bullets start at the center, therefore an extra 0.5f is added to the range
                if (b.DistanceTraveled > _maxRange + 0.5f)
                    b.MarkedForDeletion = true;
            }

        }

        private void BulletCollisionChecker() {
            foreach (Path p in Utility.Board.Paths) {
                foreach(Minion m in p.MinionList) {
                    foreach(Projectile b in _bulletList) {
                        if (m.CollidesWithBullet(b.Position, Projectile.Radius)) {
                            m.TakeDamage(Damage);
                            b.MarkedForDeletion = true;
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch s) {
            base.Draw(s);
        }

        public void DrawProjectiles(SpriteBatch s) {
            foreach (Projectile b in _bulletList)
                b.Draw(s);
        }

        private void ShootAtTargetedMinion() {
            Vector2 direction = Vector2.Normalize(new Vector2(_targetedMinion!.Position.X - X, _targetedMinion.Position.Y - Y));
            for(int i = 0; i <= _amountOfPellets; i++) {
                Vector2 newDirection = GenerateDirectionOffset(direction);
                Projectile pellet = new Projectile(new Vector2(X + 0.5f, Y + 0.5f), newDirection, Color.Red, 6F);
                _bulletList.Add(pellet);
            }
            Utility.AssetManager.PlaySFX("shotgun_shot", 0.15f);
        }

        private static Vector2 GenerateDirectionOffset(Vector2 initialDirection) {
            float randomizedDirectionOffset = (float)(Utility.random.NextDouble() / _pelletDistribution);
            if (Utility.random.Next(0, 2) > 0)
                randomizedDirectionOffset = -randomizedDirectionOffset;
            Vector2 offsetDirection = new Vector2(initialDirection.X - randomizedDirectionOffset, initialDirection.Y - randomizedDirectionOffset);
            offsetDirection.Normalize();
            return offsetDirection;
        }

        private void TargetMinion() {
            _targetedMinion = null;
            foreach (Path p in Utility.Board.Paths) {
                foreach (Minion m in p.MinionList) {
                    if (RangeChecker(m.Position.X, m.Position.Y, _minRange))
                        continue;
                    if (!RangeChecker(m.Position.X, m.Position.Y, _maxRange))
                        continue;
                    else {
                        if (_targetedMinion == null) {
                            _targetedMinion = m;
                        } else if (_targetedMinion.DistanceTraveled < m.DistanceTraveled) {
                            _targetedMinion = m;
                        }
                    }
                }
            }
        }

        Minion? _targetedMinion = null;
        readonly List<Projectile> _bulletList;
        private const int _targetChance = 33;
        private const int _amountOfPellets = 6;
        // The higher, the tighter the spread
        private const int _pelletDistribution = 3;
    }
}
