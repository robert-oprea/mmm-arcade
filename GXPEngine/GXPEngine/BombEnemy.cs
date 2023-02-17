using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class BombEnemy : Enemy
    {
        float explosionRadius;
        float explosionTriggerRange;

        bool exploded;

        Sprite explosionHitbox;

        public BombEnemy(TiledObject obj) : base("bombEnemy.png", 1, 1, obj)
        {
            Initialize(obj);
        }

        protected override void Initialize(TiledObject obj)
        {
            base.Initialize(obj);

            explosionRadius = obj.GetIntProperty("explosionRadius", 100);
            explosionTriggerRange = obj.GetFloatProperty("explosionTriggerRange", 30f);
        }

        protected override void HandleState()
        {
            switch (state)
            {
                case State.IDLE:
                    HandleIdleState();
                    break;
                case State.CHASING:
                    HandleChasingState();
                    break;
                case State.EXPLODING:
                    HandleExplodingState();
                    break;
            }
        }

        protected override void HandleChasingState()
        {
            base.HandleChasingState();

            if (DistanceTo(target) < explosionTriggerRange)
            {
                SetState(State.EXPLODING);
            }
        }

        void HandleExplodingState()
        {
            //explosion anim

            if (exploded == false)
            {
                //this is just the hitbox, alpha should == 0 when we have an animation
                explosionHitbox = new Sprite("circle.png");
                explosionHitbox.SetOrigin(explosionHitbox.width / 2, explosionHitbox.height / 2 - 5);
                explosionHitbox.alpha = 1;
                explosionHitbox.width = (int)explosionRadius;
                explosionHitbox.height = (int)explosionRadius;
                AddChild(explosionHitbox);

                GameObject[] collisions = explosionHitbox.GetCollisions();
                for (int i = 0; i < collisions.Length; i++)
                {
                    if (collisions[i].parent is Player)
                    {
                        //do smth
                        target.TakeDamage();
                    }
                }

                exploded = true;

                _collider = null;
            }

            //LateDestroy() after animation plays
        }
    }
}
