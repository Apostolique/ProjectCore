using Microsoft.Xna.Framework;

namespace MonoGameJamProject.Towers
{
    class Sniper : Tower
    {
        private const int minimumRange = 3;
        public Sniper(int iX, int iY) : base(iX, iY)
        {
            towerColor = Color.Blue;
        }
    }
}
