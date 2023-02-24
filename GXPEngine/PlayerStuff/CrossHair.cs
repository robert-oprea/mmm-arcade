using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public class CrossHair : Sprite
    {
        float moveSpeed = 4.0f;

        public CrossHair() : base("aim.png")
        {
            SetOrigin(width / 2, height / 2);
            x = 400;
            y = 300;
            scale = 1;
        }

        void Update()
        {
            if (Input.GetKey(Key.O))
            {
                x = x - moveSpeed;
            }

            if (Input.GetKey(Key.P))
            {
                x = x + moveSpeed;
            }

            if (Input.GetKey(Key.U))
            {
                y = y - moveSpeed;
            }

            if (Input.GetKey(Key.I))
            {
                y = y + moveSpeed;
            }
        }
    }
}
