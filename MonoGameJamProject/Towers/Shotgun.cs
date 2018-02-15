using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject.Towers
{
    class Shotgun : Tower
    {
        Minion targetedMinion = null;
        List<ShotgunPellet> bulletList;
        private const int targetChance = 33;
        private const int amountOfPellets = 5;
        public Shotgun(int iX, int iY) : base(iX, iY, 1.5F)
        {
            towerColor = Color.SaddleBrown;
            type = Utility.TowerType.Shotgun;
            bulletList = new List<ShotgunPellet>();
            minRange = 1;
            maxRange = 3;
            damage = 5;
        }

        public override void Update(GameTime gameTime, List<Minion> iMinionList)
        {
            base.Update(gameTime, iMinionList);
            if (!disabled && attackTimer.IsExpired)
            {
                TargetRandomMinion(iMinionList);
                if (targetedMinion != null)
                    ShootAtTargetedMinion();
                attackTimer.Reset();
            }

            for(int x = bulletList.Count - 1; x >=0; x--)
            {
                if (bulletList[x].MarkedForDeletion)
                    bulletList.Remove(bulletList[x]);
            }
            foreach(ShotgunPellet b in bulletList)
            {
                b.Update(gameTime);
                BulletCollisionChecker(iMinionList);
                // bullets start at the center, therefore an extra 0.5f is added to the range
                if (b.DistanceTravelled > maxRange + 0.5f)
                    b.MarkedForDeletion = true;
            }

        }

        private void BulletCollisionChecker(List<Minion> iMinionList)
        {
            foreach(Minion m in iMinionList)
            {
                foreach(ShotgunPellet b in bulletList)
                {
                    if (m.CollidesWithBullet(b.Position, b.Radius))
                    {
                        m.TakeDamage(damage);
                        b.MarkedForDeletion = true;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch s)
        {
            foreach (ShotgunPellet b in bulletList)
                b.Draw(s);
            base.Draw(s);
        }

        private void ShootAtTargetedMinion()
        {
            Vector2 direction = Vector2.Normalize(new Vector2(targetedMinion.Position.X - this.X, targetedMinion.Position.Y - this.Y));
            for(int i = 0; i <= amountOfPellets; i++)
            {
                Vector2 newDirection = GenerateDirectionOffset(direction);
                ShotgunPellet pellet = new ShotgunPellet(new Vector2(this.X + 0.5f, this.Y + 0.5f), newDirection);
                bulletList.Add(pellet);
            }
            Utility.assetManager.PlaySFX("shotgun_shot", 0.25f);
        }

        private Vector2 GenerateDirectionOffset(Vector2 initialDirection)
        {
            Vector2 offsettedDirection = initialDirection;
            bool negative = Utility.random.Next(0, 2) > 0;
            float randomizedDirectionOffset = (float)(Utility.random.NextDouble() / 4);
            if (negative)
                randomizedDirectionOffset = -randomizedDirectionOffset;
            if (Math.Abs(offsettedDirection.Y) > Math.Abs(offsettedDirection.X))
                offsettedDirection.X += randomizedDirectionOffset;
            else
                offsettedDirection.Y += randomizedDirectionOffset;
            return offsettedDirection;
        }

        private void TargetRandomMinion(List<Minion> minionList)
        {
            targetedMinion = null;
            if (minionList.Count > 0)
            {
                foreach (Minion m in minionList)
                {
                    if (RangeChecker(m.Position.X, m.Position.Y, minRange))
                        continue;
                    if (!RangeChecker(m.Position.X, m.Position.Y, maxRange))
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
