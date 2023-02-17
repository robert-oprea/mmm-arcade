﻿using System;
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

            //Console.WriteLine("Start x: {0}; Start y: {1}, Target x: {2}; Target y: {3};", startX, startY, targetX, targetY);
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

            foreach (GameObject col in collisions)
            {
                if (col != owner)
                {
                    if (col.parent is Enemy)
                    {
                        LateDestroy();
                    }

                    if (col is Tiles)
                    {
                        LateDestroy();
                    }
                }
            }
        }

        void Update()
        {
            Move();
        }
    }
}