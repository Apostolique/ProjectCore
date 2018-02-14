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
        CoolDownTimer shootTimer;
        public Sniper(int iX, int iY) : base(iX, iY)
        {
            towerColor = Color.White;
            type = Utility.TowerType.Sniper;
            shootTimer = new CoolDownTimer(3);
            shootTimer.IsExpired = true;
            minRange = 2;
            damage = 20;
            towerInfo = "Sniper Tower\n MinRange: " + minRange + "\nMaxRange: INFINITE!\nPosX:";
        }
        public override void Update(GameTime gameTime, List<Minion> iMinionList)
        {
            shootTimer.Update(gameTime);
            if (targetedMinion != null)
                if (shootTimer.IsExpired)
                {
                    targetedMinion.TakeDamage(damage);
                    shootTimer.Reset();
                }
            TargetMinion(iMinionList);
            base.Update(gameTime, iMinionList);
        }
        public override void Draw(SpriteBatch s)
        {
            base.Draw(s);
            if (!disabled && !(targetedMinion == null))
                s.DrawLine(Utility.GameToScreen(this.X) + Utility.board.GridSize / 2, Utility.GameToScreen(this.Y) + Utility.board.GridSize / 2, Utility.GameToScreen(targetedMinion.Position.X), Utility.GameToScreen(targetedMinion.Position.Y), Color.Red, 2f);
        }
        private void TargetMinion(List<Minion> minionList)
        {
            targetedMinion = null;
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
        }
    }
}
