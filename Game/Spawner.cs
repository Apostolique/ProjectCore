using Microsoft.Xna.Framework;

namespace GameProject {
    /// <summary>
    /// Goal: A class in charge of spawning minions.
    /// </summary>
    class Spawner {
        public Spawner() {
            _timer1 = new CoolDownTimer(1);
            _timer1.Reset();

            _timer2 = new CoolDownTimer(5);
            _timer2.Reset();
            IsActive = true;
        }

        public bool IsActive { get; set; }

        public void Update(GameTime gameTime, Path p) {
            _timer1.Update(gameTime);
            _timer2.Update(gameTime);

            if (_timer1.IsExpired && IsActive) {
                p.AddMinion(new Minion(0, 0, Utility.MinionType.Fast));
                _timer1.Reset();
            }
            if (_timer2.IsExpired && IsActive) {
                p.AddMinion(new Minion(0, 0, Utility.MinionType.Slow));
                _timer2.Reset();
            }
        }

        CoolDownTimer _timer1;
        CoolDownTimer _timer2;
    }
}
