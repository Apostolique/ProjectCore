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
        public Vector2 position
        {
            get; set;
        }
        struct waypoint
        {
            public Vector2 start;
            public Vector2 target;
            public float distance;
            
            public waypoint(Vector2 iStart, Vector2 iTarget, float iDistance)
            {
                start = iStart;
                target = iTarget;
                distance = iDistance;
            }
        }
        List<waypoint> waypoints;
        float speed;
        public float Radius
        {
            get; set;
        }

        float inBetween;
        CoolDownTimer damageClock, onFireClock;
        private const int fireDamage = 6;
        public bool dead, isOnFire;
        int hp;

        public Minion(float iX, float iY, float iRadius)
        {
            //FIXME: The speed and movement is really wrong.
            position = new Vector2(iX, iY);
            speed = 0.0005f;
            inBetween = 0;
            Radius = iRadius;

            waypoints = new List<waypoint>();

            damageClock = new CoolDownTimer(0.5F);
            onFireClock = new CoolDownTimer(5F);
            hp = 1;
            Reset();
            damageClock.IsExpired = true;
        }
        public bool IsMoving
        {
            get
            {
                if (waypoints.Count > 0) {
                    return inBetween < waypoints[0].distance;
                }
                return false;
            }
        }
        public void Reset()
        {
            damageClock.Reset();
            onFireClock.Reset();
        }
        public void Update(GameTime gameTime)
        {
            while (waypoints.Count() > 0 && inBetween >= waypoints.First().distance) {
                inBetween -= waypoints[0].distance;
                waypoints.RemoveAt(0);
            }
            if (waypoints.Count > 0) {
                inBetween += speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                inBetween = Math.Min(waypoints[0].distance, inBetween);
                position = Vector2.Lerp(waypoints[0].start, waypoints[0].target, inBetween / waypoints[0].distance);
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
            position = b;
            waypoints.Clear();
        }
        public void WalkTo(Vector2 b)
        {
            if (waypoints.Count > 0) {
                waypoint waypoint = new waypoint(waypoints.Last().target, b, Vector2.Distance(waypoints.Last().target, b));
                waypoints.Add(waypoint);
            } else {
                waypoint waypoint = new waypoint(position, b, Vector2.Distance(position, b));
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
        public void Draw(SpriteBatch s, int gridSize)
        {
            s.DrawCircle(new CircleF(new Point2(Utility.GameToScreen(position.X, gridSize), Utility.GameToScreen(position.Y, gridSize)), Radius * gridSize), 8, Color.Green, Radius * gridSize);
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

    }
}

