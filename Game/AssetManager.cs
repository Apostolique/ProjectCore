using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace GameProject {
    class AssetManager {
        public AssetManager(ContentManager iContent) {
            _content = iContent;
            FontSystem = new FontSystem();
            FontSystem.AddFont(TitleContainer.OpenStream($"{_content.RootDirectory}/Jura-Bold.ttf"));
        }

        public void PlaySFX(string SFXName, float volume = 1f) {
            SoundEffect snd = _content.Load<SoundEffect>(SFXName);
            snd.Play(volume, 0f, 0f);
        }

        public void PlayMusic(string assetName, float volume = 0.5f, bool repeat = true) {
            MediaPlayer.Volume = volume;
            MediaPlayer.IsRepeating = repeat;
            MediaPlayer.Play(_content.Load<Song>(assetName));
        }

        public DynamicSpriteFont GetFont(float size) {
            return FontSystem.GetFont((int)size);
        }

        public FontSystem FontSystem;

        protected ContentManager _content;
    }
}
