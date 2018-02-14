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
            if (targetedMinion != null)
                if (OutsideMinimumRange(targetedMinion.Position.X, targetedMinion.Position.Y))
                    targetedMinion = null;
            TargetClosestMinion(iMinionList);
            base.Update(gameTime, iMinionList);
        }

        public override void Draw(SpriteBatch s)
        {
            base.Draw(s);
            if (!disabled && !(targetedMinion == null))
                s.DrawLine(Utility.GameToScreen(this.X) + Utility.board.GridSize / 2, Utility.GameToScreen(this.Y) + Utility.board.GridSize / 2, Utility.GameToScreen(targetedMinion.Position.X), Utility.GameToScreen(targetedMinion.Position.Y), Color.Red, 2f);

        }

        private void TargetClosestMinion(List<Minion> minionList)
        {
            if (minionList.Count > 0)
            {
                foreach (Minion m in minionList)
                {
                    if (OutsideMinimumRange(m.Position.X, m.Position.Y))
                        continue;
                    else if (targetedMinion == null)
                        targetedMinion = m;
                    else if (m.DistanceTraveled > targetedMinion.DistanceTraveled)
                    {
                        targetedMinion = m;
                        break;
                    }
                }
            }
            else
                targetedMinion = null;
            
        }

    }
}
