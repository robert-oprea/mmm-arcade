using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    class Bullet : Sprite
    {
        float targetX;
        float targetY;
        float startX;
        float startY;

        GameObject owner;

        float bulletSpeed;

        public Bullet(float pTargetX, float pTargetY, float pBulletSpeed, GameObject pOwner) : base("Dust Particle.png")
        {
            SetOrigin(width / 2, height / 2);
            targetX = pTargetX;
            targetY = pTargetY;

            owner = pOwner;

            startX = owner.parent.x;
            startY = owner.parent.y;

            bulletSpeed = pBulletSpeed;
        }

        void Move()
        {
            float directionX = targetX - startX;
            float directionY = targetY - startY;

            float angle = (float)Math.Atan2(directionY, directionX);

            float vx = (float)Math.Cos(angle) * bulletSpeed;
            float vy = (float)Math.Sin(angle) * bulletSpeed;

            x += vx;
            y += vy;

            GameObject[] collisions = GetCollisions();

            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i] != owner)
                {
                    if (collisions[i].parent is Enemy && collisions[i].name != "circle.png")
                    {
                        Console.WriteLine(collisions[i].name);
                        LateDestroy();
                    }

                    if (collisions[i] is Tiles)
                    {
                        LateDestroy();
                    }

                    if (collisions[i] is Powerup)
                    {
                        Powerup powerup = collisions[i] as Powerup;
                        powerup.PickUp();
                    }
                }
            }
        }

        void Update()
        {
            Move();

            CheckOffScreen();
        }

        void CheckOffScreen()
        {
            if (x < game.x || x > game.x + game.width || y < game.y || y > game.y + game.height)
            {
                LateDestroy();
            }
        }
    }
}
