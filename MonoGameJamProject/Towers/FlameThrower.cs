using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoGameJamProject.Towers
{
    class FlameThrower : Tower
    {
        List<Point> damageTiles;
        public FlameThrower(int iX, int iY) : base(iX, iY)
        {
            towerColor = Color.OrangeRed;
            damageTiles = new List<Point>();
            type = Utility.TowerType.FlameThrower;
            minRange = 0;
            maxRange = 1;
        }

        public override void Update(GameTime gameTime)
        {
            GenerateDamageTiles();
            base.Update(gameTime);
        }

        public void GenerateDamageTiles()
        {
            for(int x = minRange; x <= maxRange; x++)
            {
                for(int y = minRange; y <= maxRange; y++)
                {
                    damageTiles.Add(new Point(x, y));
                }
            }
        }

        public void CheckDamageTiles(List<Minion> iMinionList)
        {
            foreach(Minion m in iMinionList)
            {
                foreach(Point p in damageTiles)
                {
                    if (p.X == m.position.X && p.Y == m.position.Y)
                        m.TakeDamage(damage);
                }
            }
        }
    }
}
