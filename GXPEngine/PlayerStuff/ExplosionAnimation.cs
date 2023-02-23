using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class ExplosionAnimation : AnimationSprite
    {
        public ExplosionAnimation() : base("explosion.png", 3, 1)
        {

        }

        void Update()
        {
            Animate(0.1f);
        }
    }
}
