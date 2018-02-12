using Microsoft.Xna.Framework;

namespace MonoGameJamProject.Towers
{
    class Sniper : Tower
    {
        public Sniper(int iX, int iY) : base(iX, iY)
        {
            towerColor = Color.DeepPink;
            type = Utility.TowerType.Sniper;
            minimumRange = 2;
        }
    }
}
