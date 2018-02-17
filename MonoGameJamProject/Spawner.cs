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
        CoolDownTimer timer1;
        CoolDownTimer timer2;
        public bool IsActive
        {
            get; set;
        }
        public Spawner()
        {
            timer1 = new CoolDownTimer(1);
            timer1.Reset();

            timer2 = new CoolDownTimer(5);
            timer2.Reset();
            IsActive = true;
        }
        public void Update(GameTime gameTime, Path p)
        {
            timer1.Update(gameTime);
            timer2.Update(gameTime);

            if (timer1.IsExpired && IsActive)
            {
                p.AddMinion(new Minion(0, 0, Utility.MinionType.fast));
                timer1.Reset();
            }
            if (timer2.IsExpired && IsActive)
            {
                p.AddMinion(new Minion(0, 0, Utility.MinionType.slow));
                timer2.Reset();
            }
        }
    }
}

