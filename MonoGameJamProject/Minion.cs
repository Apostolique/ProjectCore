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

        float inBetween;
        public bool dead;
        int hp;

        public Minion(float iX, float iY)
        {
            //FIXME: The speed and movement is really wrong.
            position = new Vector2(iX, iY);
            speed = 0.005f;
            inBetween = 0;

            waypoints = new List<waypoint>();
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
        }
        public void MoveTo(Vector2 b)
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

        public void Draw(SpriteBatch s, float size, int gridSize)
        {
            float halfSize = size * gridSize / 2f;
            s.FillRectangle(new RectangleF(Utility.GameToScreen(position.X, gridSize) - halfSize, Utility.GameToScreen(position.Y, gridSize) - halfSize, size * gridSize, size * gridSize), Color.Green);
        }

        public void TakeDamage(int damage)
        {
            this.hp -= damage;
            if(hp < 0)
            {
                dead = true;
            }
        }

    }
}

