using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoGameJamProject.Towers
{
    class FlameThrower : Tower
    {
        List<Point> damageTiles;
        public FlameThrower(int iX, int iY) : base(iX, iY, 1F)
        {
            towerColor = Color.OrangeRed;
            damageTiles = new List<Point>();
            type = Utility.TowerType.FlameThrower;
            minRange = 0;
            maxRange = 1;
        }
        public override void Update(GameTime gameTime, List<Minion> iMinionList)
        {
            base.Update(gameTime, iMinionList);
            if (!disabled && attackTimer.IsExpired)
            {
                GenerateDamageTiles();
                CheckDamageTiles(iMinionList);
                attackTimer.Reset();
            }
        }
        public void GenerateDamageTiles()
        {
            damageTiles.Clear();
            for(int i = -minRange; i <= maxRange; i++)
            {
                for(int j = -minRange; j <= maxRange; j++)
                {
                    damageTiles.Add(new Point(X + i, Y + j));
                }
            }
        }
        public void CheckDamageTiles(List<Minion> iMinionList)
        {
            foreach(Minion m in iMinionList)
            {
                foreach(Point p in damageTiles)
                {
                    if (m.IsInTile(p.X, p.Y))
                        m.isOnFire = true;
                }
            }
        }
    }
}
