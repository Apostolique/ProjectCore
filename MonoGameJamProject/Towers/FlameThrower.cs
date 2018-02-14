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
            damage = 0;
        }

        public override void Update(GameTime gameTime, List<Minion> iMinionList)
        {
            if (!disabled)
            {
                GenerateDamageTiles();
                CheckDamageTiles(iMinionList);
                base.Update(gameTime, iMinionList);
            }
        }

        public void GenerateDamageTiles()
        {
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
                    if (p.X == m.Position.X && p.Y == m.Position.Y)
                        m.IsOnFire = true;
                }
            }
        }
    }
}
