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
        Vector2 target;
        float distance;
        float inBetween;

        public Minion(float iX, float iY)
        {
            //FIXME: The speed and movement is really wrong.
            position = new Vector2(iX, iY);
            speed = 0.005f;
            distance = 0;
            inBetween = 0;
        }
        public bool IsMoving => inBetween < distance;
        public void Update(GameTime gameTime)
        {
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
    }
}

