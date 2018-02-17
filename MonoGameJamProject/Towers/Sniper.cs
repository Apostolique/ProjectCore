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
        public Sniper(int iX, int iY) : base(iX, iY, 5F)
        {
            towerColor = Color.White;
            type = Utility.TowerType.Sniper;
            minRange = 2;
            maxRange = Math.Max(Utility.board.FullWidth, Utility.board.FullHeight);
            damage = 20;
            towerInfo = "Sniper Tower\nMin. Range: " + minRange + "\nMax. Range: " + maxRange + "\nDamage: " + damage + "\nSingle target\nhigh damage tower";
        }
        public override void Update(GameTime gameTime)
        {
            if (!disabled)
            {
                attackTimer.Update(gameTime);
                if (targetedMinion != null)
                    if (attackTimer.IsExpired)
                    {
                        targetedMinion.TakeDamage(damage);
                        Utility.assetManager.PlaySFX("sniper_shot", 0.75f);
                        attackTimer.Reset();
                    }
                TargetMinion();
            }
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch s)
        {
            base.Draw(s);
            if (!disabled && !(targetedMinion == null))
                s.DrawLine(Utility.GameToScreen(this.X) + Utility.board.GridSize / 2, Utility.GameToScreen(this.Y) + Utility.board.GridSize / 2, Utility.GameToScreen(targetedMinion.Position.X), Utility.GameToScreen(targetedMinion.Position.Y), Color.Red, 2f);
        }
        private void TargetMinion()
        {
            targetedMinion = null;
            foreach (Path p in Utility.board.Paths)
            {
                foreach (Minion m in p.MinionList)
                {
                    if (RangeChecker(m.Position.X, m.Position.Y, MinimumRange))
                        continue;
                    else if (targetedMinion == null)
                        targetedMinion = m;
                    else if (m.DistanceTraveled > targetedMinion.DistanceTraveled)
                    {
                        targetedMinion = m;
                    }
                }
            }
        }
    }
}
