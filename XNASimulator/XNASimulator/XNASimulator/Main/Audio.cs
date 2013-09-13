using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace KruispuntGroep6.Simulator.Main
{
    class Audio
    {
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;
        SoundEffect soundEffect;

        public Audio(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");
        }

        public void PlayBackgroundMusic()
        {
            soundEffect = Content.Load<SoundEffect>("Audio/Music/backgroundmusic");
            soundEffect.Play();
        }
    }
}