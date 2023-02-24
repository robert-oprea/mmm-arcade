using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GXPEngine
{
    class HUD : GameObject
    {
        float flashingTimer = 1000;
        float lastFlash;

        List<Sprite> hearts = new List<Sprite>();

        Sprite heart;

        EasyDraw score;

        EasyDraw start;

        Font pixelFont;

        Sprite mainMenu;

        bool isVisible;

        public HUD()
        {
            pixelFont = Utils.LoadFont("pixelFont.ttf", 15);
            score = new EasyDraw(250, 60, false);
            score.TextFont(pixelFont);
            score.TextAlign(CenterMode.Max, CenterMode.Center);
            score.Text("");
            score.SetXY(530, 0);
            AddChild(score);

            start = new EasyDraw(600, 60, false);
            start.TextFont(pixelFont);
            start.TextAlign(CenterMode.Min, CenterMode.Center);
            start.Text("");
            start.SetXY(135, 500);
            AddChild(start);
        }

        public void SetScore(int points)
        {
            score.Text(String.Format("{0:D8}", points), true);
        }

        public void showStartingScreen()
        {
            start.Text(String.Format("PRESS ANY KET TO CONTINUE"), true);

            if (mainMenu == null)
            {
                mainMenu = new Sprite("final_game_poster.png");
                mainMenu.scale = 0.228f;
                mainMenu.y = -470;
                AddChild(mainMenu);
                SetChildIndex(mainMenu, 0);
            }

            if (Time.time > lastFlash + flashingTimer)
            {
                if (isVisible == false)
                {
                    start.alpha = 1;
                }
                else if (isVisible == true)
                {
                    start.alpha = 0;
                }

                isVisible = !isVisible;
                lastFlash = Time.time;
            }
            
        }

        public void SetHealth(float health)
        {
            float nextHeartX = 0;

            // Create a new list to hold the heart sprites


            // Remove any previously created heart sprites
            if (hearts != null)
            {
                foreach (Sprite heart in hearts)
                {
                    RemoveChild(heart);
                }
                hearts.Clear();
            }

            for (int i = 0; i < health; i++)
            {
                heart = new Sprite("maxhp.png", false, false);
                heart.SetOrigin(heart.width / 2, heart.height / 2);
                heart.scale = 1.7f;
                heart.SetXY(40, 40);
                heart.x += nextHeartX;
                AddChild(heart);
                hearts.Add(heart);
                nextHeartX += heart.width + 5;
            }
        }
    }
}
