using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace GameProject {
    class AssetManager {
        public AssetManager(ContentManager content) {
            _content = content;
            FontSystem = new FontSystem();
            FontSystem.AddFont(TitleContainer.OpenStream($"{_content.RootDirectory}/Jura-Bold.ttf"));
            SniperSound = content.Load<SoundEffect>("sniper_shot");
            ShotgunSound = content.Load<SoundEffect>("shotgun_shot");
            DeathSound = content.Load<SoundEffect>("Death");
            BreakSound = content.Load<SoundEffect>("BREAK_HIT_Celery_2");
            RobotSound = content.Load<SoundEffect>("Robot_Servo_006");

            BreakMusic = content.Load<SoundEffect>("break_space");
            AbstractMusic = content.Load<SoundEffect>("AbstractAmbiences-Mix_ST_37");
        }

        public void PlaySFX(SoundEffect se, float volume = 1f) {
            se.Play(volume, 0f, 0f);
        }

        public void PlayMusic(SoundEffect se, float volume = 0.5f, bool repeat = true) {
            // MediaPlayer.Volume = volume;
            // MediaPlayer.IsRepeating = repeat;
            // MediaPlayer.Play(_content.Load<Song>(assetName));

            if (CurrentMusic != null) {
                CurrentMusic.Stop();
                CurrentMusic.Dispose();
            }

            CurrentMusic = se.CreateInstance();
            CurrentMusic.Volume = volume;
            CurrentMusic.IsLooped = repeat;
            CurrentMusic.Play();
        }

        public DynamicSpriteFont GetFont(float size) {
            return FontSystem.GetFont((int)size);
        }

        public FontSystem FontSystem;
        public SoundEffectInstance CurrentMusic = null!;

        public SoundEffect SniperSound;
        public SoundEffect ShotgunSound;
        public SoundEffect DeathSound;
        public SoundEffect BreakSound;
        public SoundEffect RobotSound;

        public SoundEffect BreakMusic;
        public SoundEffect AbstractMusic;

        protected ContentManager _content;
    }
}
