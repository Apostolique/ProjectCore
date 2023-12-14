using System;
using System.Collections.Generic;
using System.Linq;
using GameProject.Towers;
using GameProject.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject {
    /// <summary>
    /// Goal: Base class for the game's minions.
    /// </summary>
    class Minion {
        public Minion(float x, float y, Utility.MinionType type) {
            Position = new Vector2(x, y);
            _inBetween = 0;
            _distanceTraveled = 0;
            _waypoints = [];
            _stackFlamethrowers = [];
            _fireClock = new CoolDownTimer(1);
            _fireClock.Reset();
            _type = type;

            if (_type == Utility.MinionType.Fast) {
                _hp = Utility.FastMinionHP;
                MaxHP = Utility.FastMinionHP;
                _speed = 0.002f;
                _radius = 0.2f;
            } else if (_type == Utility.MinionType.Slow) {
                _hp = Utility.SlowMinionHP;
                MaxHP = Utility.SlowMinionHP;
                _speed = 0.0005f;
                _radius = 0.4f;
            } else if (_type == Utility.MinionType.Boss) {
                _hp = Utility.BossMinionHp;
                MaxHP = Utility.BossMinionHp;
                _speed = 0.0001f;
                _radius = 0.6f;
            }
            _healthBar = new HealthBar(this);
        }

        public Vector2 Position { get; set; }

        public bool Dead;
        public float MaxHP;

        public float DistanceTraveled => _distanceTraveled;
        public bool IsMoving => _waypoints.Count > 0;
        public bool IsOnFire => _stackFlamethrowers.Count > 0;

        public float Radius => _radius;

        public float HP => _hp;

        public Utility.MinionType Type => _type;
        public int FireStacks => _stackFlamethrowers.Count;

        public void MoveTo(Vector2 b) {
            Position = b;
            _waypoints.Clear();
        }
        public void WalkTo(Vector2 b) {
            if (_waypoints.Count > 0) {
                Waypoint waypoint = new Waypoint(_waypoints.Last().Target, b, Vector2.Distance(_waypoints.Last().Target, b));
                _waypoints.Add(waypoint);
            } else {
                Waypoint waypoint = new Waypoint(Position, b, Vector2.Distance(Position, b));
                _waypoints.Add(waypoint);
            }
        }
        public void FollowPath(Path p) {
            if (p.Count() > 0) {
                Tile t = p.First();
                Vector2 v1 = new Vector2(t.X + 0.5f, t.Y + 0.5f);
                MoveTo(v1);
                for (int i = 1; i < p.Count(); i++) {
                    Vector2 v2 = new Vector2(p.pathway[i].X + 0.5f, p.pathway[i].Y + 0.5f);
                    WalkTo(v2);
                }
            }
        }
        public void TakeDamage(int damage) {
            _hp -= damage;
            if(_hp < 0) {
                Dead = true;
            }
        }

        public bool IsInTile(int iX, int iY) {
            Rectangle tileBoundingBox = new Rectangle(iX, iY, 1, 1);
            Vector2 circleDistance = new Vector2(Math.Abs(Position.X - iX), Math.Abs(Position.Y - iY));
            if (circleDistance.X > (tileBoundingBox.Width / 2f + _radius))
                return false;
            if (circleDistance.Y > (tileBoundingBox.Height / 2f + _radius))
                return false;

            if (circleDistance.X <= (tileBoundingBox.Width / 2f))
                return true;
            if (circleDistance.Y <= (tileBoundingBox.Height / 2f))
                return true;

            double cornerDistance_sq = Math.Pow(circleDistance.X - tileBoundingBox.Width / 2f, 2f) + Math.Pow(circleDistance.Y - tileBoundingBox.Height / 2f, 2f);

            return cornerDistance_sq <= (_radius * _radius);
        }
        public bool CollidesWithBullet(Vector2 bulletOrigin , float bulletRadius) {
            var totalRadius = _radius + bulletRadius;
            var dX = Position.X - bulletOrigin.X;
            var dY = Position.Y - bulletOrigin.Y;
            return dX * dX + dY * dY <= totalRadius * totalRadius;
        }
        public void Update(GameTime gameTime) {
            _distanceTraveled += _speed + (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _inBetween += _speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            while (_waypoints.Count > 0 && _inBetween >= _waypoints.First().Distance) {
                _inBetween -= _waypoints[0].Distance;
                _waypoints.RemoveAt(0);
            }
            if (_waypoints.Count > 0) {
                _inBetween = Math.Min(_waypoints[0].Distance, _inBetween);
                Position = Vector2.Lerp(_waypoints[0].Start, _waypoints[0].Target, _inBetween / _waypoints[0].Distance);
            } else {
                _inBetween = 0;
            }
            _fireClock.Update(gameTime);

            for (int i = _stackFlamethrowers.Count - 1; i >= 0; i--) {
                _stackFlamethrowers[i].FireTimer.Update(gameTime);
                if (_stackFlamethrowers[i].FireTimer.IsExpired) {
                    _stackFlamethrowers.RemoveAt(i);
                }
            }

            if (_fireClock.IsExpired) {
                foreach (FireStack fs in _stackFlamethrowers) {
                    TakeDamage(fs.FlameThrower.Damage);
                }
                _fireClock.Reset();
            }
        }
        public void Draw(SpriteBatch s) {
            Point2 tempP = new Point2(Utility.GameToScreen(Position.X), Utility.GameToScreen(Position.Y));
            if (_type == Utility.MinionType.Fast) {
                s.DrawCircle(new CircleF(tempP, _radius * Utility.Board.GridSize), 8, Color.Green, 0.1f * Utility.Board.GridSize);
                s.DrawCircle(new CircleF(tempP, _radius * Utility.Board.GridSize), 8, Color.Black, 2f);
            } else if (_type == Utility.MinionType.Slow) {
                s.DrawCircle(new CircleF(tempP, _radius * Utility.Board.GridSize), 8, Color.Orange, _radius * Utility.Board.GridSize);
                s.DrawCircle(new CircleF(tempP, _radius * Utility.Board.GridSize), 8, Color.Black, 0.1f * Utility.Board.GridSize);
            } else {
                s.DrawCircle(new CircleF(tempP, _radius * Utility.Board.GridSize), 8, Color.Black, (_radius + 1f) * Utility.Board.GridSize);
                s.DrawCircle(new CircleF(tempP, _radius * Utility.Board.GridSize), 8, Color.Red, (_radius + 0.8f) * Utility.Board.GridSize);
            }
            _healthBar.Draw(s);
        }

        public void AddFireStack(FlameThrower f) {
            bool added = false;
            for (int i = 0; i < _stackFlamethrowers.Count; i++) {
                if (_stackFlamethrowers[i].FlameThrower == f) {
                    FireStack fs = new FireStack(f, new CoolDownTimer(FlameThrower.BurnTime));
                    fs.FireTimer.Reset();
                    _stackFlamethrowers[i] = fs;
                    added = true;
                }
            }
            if (!added) {
                FireStack fs = new FireStack(f, new CoolDownTimer(FlameThrower.BurnTime));
                fs.FireTimer.Reset();
                _stackFlamethrowers.Add(fs);
            }
        }

        struct Waypoint(Vector2 start, Vector2 target, float distance) {
            public Vector2 Start = start;
            public Vector2 Target = target;
            public float Distance = distance;
        }
        struct FireStack(FlameThrower flameThrower, CoolDownTimer fireTimer) {
            public FlameThrower FlameThrower = flameThrower;
            public CoolDownTimer FireTimer = fireTimer;
        }
        readonly List<Waypoint> _waypoints;
        readonly float _speed;
        readonly float _radius;
        float _inBetween;
        float _distanceTraveled;
        readonly HealthBar _healthBar;
        readonly List<FireStack> _stackFlamethrowers;
        readonly CoolDownTimer _fireClock;
        float _hp;
        readonly Utility.MinionType _type;
    }
}
