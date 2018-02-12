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
        float x;
        float y;
        float hp;
        public bool dead;
        public Minion(float iX, float iY)
        {
            x = iX;
            y = iY;
        }

        public void Draw(SpriteBatch s, float size, int gridSize)
        {
            float halfSize = size * gridSize / 2f;
            s.FillRectangle(new RectangleF(Utility.GameToScreen(x, gridSize) - halfSize, Utility.GameToScreen(y, gridSize) - halfSize, size * gridSize, size * gridSize), Color.Green);
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

