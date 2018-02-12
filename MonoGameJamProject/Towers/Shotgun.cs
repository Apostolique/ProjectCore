using Microsoft.Xna.Framework;

namespace MonoGameJamProject.Towers
{
    class Shotgun : Tower
    {
        public Shotgun(int iX, int iY) : base(iX, iY)
        {
            towerColor = Color.SaddleBrown;
            type = Utility.TowerType.Shotgun;
            minRange = 1;
            maxRange = 2;
        }
    }
}
