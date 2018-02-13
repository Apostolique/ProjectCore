using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject.Towers
{
    class Sniper : Tower
    {
        Minion targetedMinion;
        public Sniper(int iX, int iY) : base(iX, iY)
        {
            towerColor = Color.White;
            type = Utility.TowerType.Sniper;
            minRange = 2;
            towerInfo = "Sniper Tower\n MinRange: " + minRange + "\nMaxRange: INFINITE!\nPosX:";
        }

        public override void Update(GameTime gameTime, List<Minion> iMinionList)
        {
            TargetClosestMinion(iMinionList);
            base.Update(gameTime, iMinionList);
        }

        public override void Draw(SpriteBatch s, int gridSize)
        {
            base.Draw(s, gridSize);
            if (!disabled && !(targetedMinion == null))
                s.DrawLine(Utility.GameToScreen(this.X, gridSize) + gridSize / 2, Utility.GameToScreen(this.Y, gridSize) + gridSize / 2, Utility.GameToScreen(targetedMinion.position.X, gridSize), Utility.GameToScreen(targetedMinion.position.Y, gridSize), Color.Red, 2f);
        }

        private void TargetClosestMinion(List<Minion> minionList)
        {
            foreach(Minion m in minionList)
            {
                if (targetedMinion == null)
                    targetedMinion = m;
                        break;
            }
        }

    }
}
