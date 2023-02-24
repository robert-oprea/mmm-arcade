using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class startScreen : AnimationSprite
    {
        HUD hud;

        public startScreen(TiledObject obj) : base("Dust Particle.png", 1, 1)
        {
            alpha = 0;
            SetXY(obj.X, obj.Y);
        }
        SoundChannel music;
        void Update()
        {
            if (music == null)
            {
                music = new Sound("Menu music.mp3", true, true).Play();
            }
            if (hud == null)
            {
                hud = game.FindObjectOfType<HUD>();
            }

            if (hud != null)
            {
                hud.showStartingScreen();
            }

            if (Input.AnyKeyDown())
            {
                music.Stop();
                ((MyGame)game).LoadLevel("Levels/Placeholder.tmx");
            }
        }
    }
}
