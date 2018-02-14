using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject.Towers
{
    class Shotgun : Tower
    {
        Minion targetedMinion = null;
        private const int targetChance = 33;
        public Shotgun(int iX, int iY) : base(iX, iY)
        {
            towerColor = Color.SaddleBrown;
            type = Utility.TowerType.Shotgun;
            minRange = 1;
            maxRange = 2;
            damage = 5;
        }

        public override void Update(GameTime gameTime, List<Minion> iMinionList)
        {
            TargetRandomMinion(iMinionList);
            base.Update(gameTime, iMinionList);
        }

        public override void Draw(SpriteBatch s)
        {
            base.Draw(s);
        }

        public void TargetRandomMinion(List<Minion> minionList)
        {
            targetedMinion = null;
            if (minionList.Count > 0)
            {
                foreach (Minion m in minionList)
                {
                    if (OutsideMinimumRange(m.Position.X, m.Position.Y))
                        continue;
                    else
                    {
                        if (Utility.random.Next(0, 101) > targetChance)
                            targetedMinion = m;
                    }
                }
            }
        }
    }
}
