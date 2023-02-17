using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GXPEngine
{
    class HUD : GameObject
    {
        EasyDraw healthBar;

        public HUD()
        {
            healthBar = new EasyDraw(80, 30);
            healthBar.ShapeAlign(CenterMode.Min, CenterMode.Min);
            healthBar.NoStroke();
            healthBar.Fill(Color.Green);
            healthBar.Rect(0, 0, 80, 30);
            healthBar.SetXY(10, 10);

            AddChild(healthBar);
        }

        public void SetHealth(float health)
        {
            healthBar.Clear(Color.Red);
            healthBar.Fill(Color.Green);
            healthBar.Rect(0, 0, healthBar.width * health, healthBar.height);
        }
    }
}
