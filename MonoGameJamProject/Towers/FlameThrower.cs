using Microsoft.Xna.Framework;

namespace MonoGameJamProject.Towers
{
    class FlameThrower : Tower
    {
        public FlameThrower(int iX, int iY) : base(iX, iY)
        {
            towerColor = Color.OrangeRed;
            type = Utility.TowerType.FlameThrower;
            minRange = 0;
            maxRange = 1;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
