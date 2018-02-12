using Microsoft.Xna.Framework;

namespace MonoGameJamProject
{
    class CoolDownTimer
    {
        // floats for the targettime for a timer to be expired, and the currenttime
        private float targettime, currentime;
        // bools for if a timer is paused or expired
        private bool paused, expired;
        public CoolDownTimer(float targettime)
        {
            this.targettime = targettime;
            this.currentime = 0;
            paused = true;
            expired = false;
        }

        // Updates the timer
        public void Update(GameTime gameTime)
        {
            // if the timer expires, we do not want the timer to (possibly) overflow so we pause it
            if (expired)
            {
                IsPaused = true;
            }
            // if the timer is not paused, we check if it is expired and update the currenttime
            else if (!paused)
            {
                currentime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                expired = currentime >= targettime;
            }
        }


        //Resets the timer
        public void Reset()
        {
            currentime = 0;
            IsPaused = false;
            expired = false;
        }

        // Properties for the timer
        public bool IsPaused
        {
            get { return paused; }
            set { paused = value; }
        }
        public float MaxTime
        {
            get { return targettime; }
        }
        public float SecondsElapsed
        {
            get { return currentime; }
            set { currentime = value; }
        }
        public bool IsExpired
        {
            get { return expired; }
            set { expired = value; }
        }
    }
}
