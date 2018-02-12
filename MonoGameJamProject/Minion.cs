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

        Vector2 target;
        float reach;
        float reachSpeed;

        public Minion(float iX, float iY)
        {
            position = new Vector2(iX, iY);
            speed = 1;
            reach = 1;
        }
        public bool IsMoving => reach < 1;
        public void Update()
        {
            if (IsMoving) {
                position = Vector2.Lerp(position, target, reach);
            }
        }
        public void MoveTo(Vector2 b)
        {
            reach = 0;
            target = b;
            float distance = Vector2.Distance(position, target);
            reachSpeed = distance / speed;
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

