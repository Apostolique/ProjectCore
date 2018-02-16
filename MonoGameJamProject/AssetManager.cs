using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace MonoGameJamProject
{
    class AssetManager
    {
        protected ContentManager content;
        public AssetManager(ContentManager iContent)
        {
            content = iContent;
        }

        public void PlaySFX(string SFXName, float volume = 1f)
        {
            SoundEffect snd = content.Load<SoundEffect>(SFXName);
            snd.Play(volume, 0f, 0f);
        }

        public void PlayMusic(string assetName, float volume = 0.5f, bool repeat = true)
        {
            MediaPlayer.Volume = volume;
            MediaPlayer.IsRepeating = repeat;
            MediaPlayer.Play(content.Load<Song>(assetName));
        }

        public BitmapFont GetFont(string fontName)
        {
            return content.Load<BitmapFont>(fontName);
        }
    }
}
