using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameJamProject.Towers;
using MonoGameJamProject.UI;
using MonoGame.Extended;

namespace MonoGameJamProject
{
    /// <summary>
    /// Goal: A class in charge of spawning minions.
    /// </summary>
    class Spawner
    {
        CoolDownTimer fastTimer;
        public bool IsActive
        {
            get; set;
        }
        public Spawner()
        {
            fastTimer = new CoolDownTimer(1);
            fastTimer.Reset();
            IsActive = true;
        }
        public void Update(GameTime gameTime, Path p)
        {
            fastTimer.Update(gameTime);
            if (fastTimer.IsExpired && IsActive)
            {
                p.AddMinion(new Minion(0, 0, Utility.MinionType.fast));
                fastTimer.Reset();
            }
        }
    }
}

