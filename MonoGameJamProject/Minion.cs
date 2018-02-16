using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameJamProject.Towers;
using MonoGameJamProject.UI;
using MonoGame.Extended;

namespace MonoGameJamProject
{
    /// <summary>
    /// Goal: Base class for the game's minions.
    /// </summary>
    class Minion
    {
        
        public Vector2 Position
        {
            get; set;
        }
        struct Waypoint
        {
            public Vector2 start;
            public Vector2 target;
            public float distance;
            
            public Waypoint(Vector2 iStart, Vector2 iTarget, float iDistance)
            {
                start = iStart;
                target = iTarget;
                distance = iDistance;
            }
        }
        List<Waypoint> waypoints;
        float speed;
        float radius;

        float inBetween;
        float _distanceTraveled;
        HealthBar healthBar;
        CoolDownTimer onFireClock;
        List<FlameThrower> stackFlamethrowers;
        public bool dead, isOnFire;
        float hp;
        Utility.MinionType type;

        public Minion(float iX, float iY, Utility.MinionType iType)
        {
            Position = new Vector2(iX, iY);
            inBetween = 0;
            _distanceTraveled = 0;
            waypoints = new List<Waypoint>();
            onFireClock = new CoolDownTimer(7F);
            stackFlamethrowers = new List<FlameThrower>();
            Reset();
            type = iType;

            if (type == Utility.MinionType.fast)
            {
                hp = Utility.fastMinionHP;
                speed = 0.002f;
                radius = 0.2f;
            } else if (type == Utility.MinionType.slow)
            {
                hp = Utility.slowMinionHP;
                speed = 0.0005f;
                radius = 0.4f;
            } else if (type == Utility.MinionType.boss)
            {
                hp = Utility.bossMinionHp;
                speed = 0.0001f;
                radius = 0.6f;
            }
            healthBar = new HealthBar(this);
        }
        public float DistanceTraveled => _distanceTraveled;
        public bool IsMoving
        {
            get
            {
                return waypoints.Count > 0;
            }
        }
        public void Reset()
        {
            onFireClock.Reset();
        }
        public void MoveTo(Vector2 b)
        {
            Position = b;
            waypoints.Clear();
        }
        public void WalkTo(Vector2 b)
        {
            if (waypoints.Count > 0) {
                Waypoint waypoint = new Waypoint(waypoints.Last().target, b, Vector2.Distance(waypoints.Last().target, b));
                waypoints.Add(waypoint);
            } else {
                Waypoint waypoint = new Waypoint(Position, b, Vector2.Distance(Position, b));
                waypoints.Add(waypoint);
            }
        }
        public void FollowPath(Path p)
        {
            if (p.Count() > 0) {
                Tile t = p.First();
                Vector2 v1 = new Vector2(t.X + 0.5f, t.Y + 0.5f);
                MoveTo(v1);
                for (int i = 1; i < p.Count(); i++)
                {
                    Vector2 v2 = new Vector2(p.pathway[i].X + 0.5f, p.pathway[i].Y + 0.5f);
                    WalkTo(v2);
                }
            }
        }
        public void TakeDamage(int damage)
        {
            this.hp -= damage;
            if(hp < 0)
            {
                dead = true;
            }
        }
        public bool IsOnFire
        {
            get { return isOnFire; }
            set { isOnFire = value; }
        }
        public bool IsInTile(int iX, int iY)
        {
            Rectangle tileBoundingBox = new Rectangle(iX, iY, 1, 1);
            Vector2 circleDistance = new Vector2(Math.Abs(Position.X - iX), Math.Abs(Position.Y - iY));
            if (circleDistance.X > (tileBoundingBox.Width / 2f + radius))
                return false;
            if (circleDistance.Y > (tileBoundingBox.Height / 2f + radius))
                return false;

            if (circleDistance.X <= (tileBoundingBox.Width / 2f))
                return true; 
            if (circleDistance.Y <= (tileBoundingBox.Height / 2f))
                return true;

            double cornerDistance_sq = Math.Pow((circleDistance.X - tileBoundingBox.Width / 2f), 2f) + Math.Pow((circleDistance.Y - tileBoundingBox.Height / 2f), 2f);

            return (cornerDistance_sq <= (radius * radius));
        }
        public bool CollidesWithBullet(Vector2 bulletOrigin , float bulletRadius)
        {
            var totalRadius = radius + bulletRadius;
            var dX = Position.X - bulletOrigin.X;
            var dY = Position.Y - bulletOrigin.Y;
            return dX * dX + dY * dY <= totalRadius * totalRadius;
        }
        public void Update(GameTime gameTime)
        {
            _distanceTraveled += speed + (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            inBetween += speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            while (waypoints.Count() > 0 && inBetween >= waypoints.First().distance) {
                inBetween -= waypoints[0].distance;
                waypoints.RemoveAt(0);
            }
            if (waypoints.Count > 0) {
                inBetween = Math.Min(waypoints[0].distance, inBetween);
                Position = Vector2.Lerp(waypoints[0].start, waypoints[0].target, inBetween / waypoints[0].distance);
            } else {
                inBetween = 0;
            }

            if (onFireClock.IsExpired)
            {
                stackFlamethrowers.Clear();
                isOnFire = false;
                onFireClock.Reset();
            }
        }
        public void Draw(SpriteBatch s)
        {
            Point2 tempP = new Point2(Utility.GameToScreen(Position.X), Utility.GameToScreen(Position.Y));
            if (type == Utility.MinionType.fast)
            {
                s.DrawCircle(new CircleF(tempP, radius * Utility.board.GridSize), 8, Color.Green, 0.1f * Utility.board.GridSize);
                s.DrawCircle(new CircleF(tempP, radius * Utility.board.GridSize), 8, Color.Black, 2f);
            } else if (type == Utility.MinionType.slow)
            {
                s.DrawCircle(new CircleF(tempP, radius * Utility.board.GridSize), 8, Color.Orange, radius * Utility.board.GridSize);
                s.DrawCircle(new CircleF(tempP, radius * Utility.board.GridSize), 8, Color.Black, 0.1f * Utility.board.GridSize);
            } else
            {
                s.DrawCircle(new CircleF(tempP, radius * Utility.board.GridSize), 8, Color.Black, (radius + 1f) * Utility.board.GridSize);
                s.DrawCircle(new CircleF(tempP, radius * Utility.board.GridSize), 8, Color.Red, (radius + 0.8f) * Utility.board.GridSize);
            }
            healthBar.Draw(s);
        }

        public float Radius
        {
            get { return radius; }
        }

        public float HP
        {
            get { return hp; }
        }

        public Utility.MinionType Type
        {
            get { return type; }
        }

        public List<FlameThrower> StackFlamethrowers
        {
            get { return stackFlamethrowers; }
        }
    }
}

