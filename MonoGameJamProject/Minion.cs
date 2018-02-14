using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public float Radius
        {
            get; set;
        }

        float inBetween;
        float _distanceTraveled;
        CoolDownTimer damageClock, onFireClock;
        private const int fireDamage = 6;
        public bool dead, isOnFire;
        int hp;

        public Minion(float iX, float iY, float iRadius)
        {
            //FIXME: The speed and movement is really wrong.
            Position = new Vector2(iX, iY);
            speed = 0.005f;
            inBetween = 0;
            _distanceTraveled = 0;
            Radius = iRadius;

            waypoints = new List<Waypoint>();

            damageClock = new CoolDownTimer(0.5F);
            onFireClock = new CoolDownTimer(5F);
            hp = 1;
            Reset();
            damageClock.IsExpired = true;
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
            damageClock.Reset();
            onFireClock.Reset();
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

            damageClock.Update(gameTime);
            if (onFireClock.IsExpired)
            {
                isOnFire = false;
                onFireClock.Reset();
            }
            if (isOnFire)
            {
                onFireClock.Update(gameTime);
                if (damageClock.IsExpired)
                {
                    this.TakeDamage(fireDamage);
                    damageClock.Reset();
                }
            }
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

            //inBetween = 0;
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
        public void Draw(SpriteBatch s)
        {
            s.DrawCircle(new CircleF(new Point2(Utility.GameToScreen(Position.X), Utility.GameToScreen(Position.Y)), (Radius + 0.1f) * Utility.board.GridSize), 8, Color.Black, 1.1f * Utility.board.GridSize);
            s.DrawCircle(new CircleF(new Point2(Utility.GameToScreen(Position.X), Utility.GameToScreen(Position.Y)), Radius * Utility.board.GridSize), 8, Color.Green, 1f * Utility.board.GridSize);
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
            if (circleDistance.X > (tileBoundingBox.Width / 2f + Radius))
                return false;
            if (circleDistance.Y > (tileBoundingBox.Height / 2f + Radius))
                return false;

            if (circleDistance.X <= (tileBoundingBox.Width / 2f))
                return true; 
            if (circleDistance.Y <= (tileBoundingBox.Height / 2f))
                return true;

            double cornerDistance_sq = Math.Pow((circleDistance.X - tileBoundingBox.Width / 2f), 2f) + Math.Pow((circleDistance.Y - tileBoundingBox.Height / 2f), 2f);

            return (cornerDistance_sq <= (Radius * Radius));
        }

    }
}

