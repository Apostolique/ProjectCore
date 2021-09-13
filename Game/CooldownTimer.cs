using Microsoft.Xna.Framework;

namespace GameProject {
    class CoolDownTimer {
        public CoolDownTimer(float targetTime) {
            this._targetTime = targetTime;
            this._currentTime = 0;
            _paused = true;
            _expired = false;
        }

        // Updates the timer
        public void Update(GameTime gameTime) {
            // if the timer expires, we do not want the timer to (possibly) overflow so we pause it
            if (_expired) {
                IsPaused = true;
            }
            // if the timer is not paused, we check if it is expired and update the currenttime
            else if (!_paused) {
                _currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _expired = _currentTime >= _targetTime;
            }
        }
        //Resets the timer
        public void Reset() {
            _currentTime = 0;
            IsPaused = false;
            _expired = false;
        }
        // Properties for the timer
        public bool IsPaused {
            get => _paused;
            set { _paused = value; }
        }
        public float MaxTime => _targetTime;
        public float SecondsElapsed {
            get { return _currentTime; }
            set { _currentTime = value; }
        }
        public bool IsExpired {
            get { return _expired; }
            set { _expired = value; }
        }

        // floats for the targettime for a timer to be expired, and the currenttime
        private float _targetTime, _currentTime;
        // bools for if a timer is paused or expired
        private bool _paused, _expired;
    }
}
