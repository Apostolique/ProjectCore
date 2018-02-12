using System;
using System.Collections.Generic;
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
        float speed;
        Vector2 start;
        CoolDownTimer damageClock, onFireClock;
        Vector2 target;
        private const int fireDamage = 6;
        float distance;
        float inBetween;
        public bool dead;
        int hp;

        public Minion(float iX, float iY)
        {
            //FIXME: The speed and movement is really wrong.
            position = new Vector2(iX, iY);
            damageClock = new CoolDownTimer(0.5F);
            onFireClock = new CoolDownTimer(5F);
            speed = 0.005f;
            distance = 0;
            inBetween = 0;
            Reset();
            damageClock.IsExpired = true;
        }
        public bool IsMoving => inBetween < distance;

        public void Reset()
        {
            damageClock.Reset();
            onFireClock.Reset();
        }
        public void Update(GameTime gameTime)
        {
            damageClock.Update(gameTime);
            if (onFireClock.IsExpired)
            {
                IsOnFire = false;
                onFireClock.Reset();
            }
            if (IsOnFire)
            {
                onFireClock.Update(gameTime);
                if (damageClock.IsExpired)
                {
                    this.TakeDamage(fireDamage);
                    damageClock.Reset();
                }
            }

            if (IsMoving) {
                inBetween += speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                inBetween = Math.Min(distance, inBetween);
                position = Vector2.Lerp(start, target, inBetween / distance);
            }
        }
        public void MoveTo(Vector2 b)
        {
            inBetween = 0;
            start = position;
            target = b;
            distance = Vector2.Distance(start, target);
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

        public bool IsOnFire
        {
            get; set;
        }

    }
}

