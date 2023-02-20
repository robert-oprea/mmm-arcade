﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class SnakeEnemy : Enemy
    {
        float burrowStartRange;
        float burrowEndRange;
        float burrowSpeed;
        float normalSpeed;

        public SnakeEnemy(TiledObject obj) : base("snakeEnemy.png", 1, 1, obj)
        {
            Initialize(obj);
        }

        protected override void Initialize(TiledObject obj)
        {
            base.Initialize(obj);
            normalSpeed = speed;

            burrowStartRange = obj.GetFloatProperty("burrowStartRange", 100.0f);
            burrowEndRange = obj.GetFloatProperty("burrowEndRange", 50.0f);
            burrowSpeed = obj.GetFloatProperty("burrowSpeed", 0.8f);
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
                case State.BURROWING:
                    HandleBurrowingState();
                    break;
            }
        }

        protected override void HandleChasingState()
        {
            base.HandleChasingState();

            speed = normalSpeed;

            this.alpha = 1.0f;

            if (DistanceTo(target) < burrowStartRange && DistanceTo(target) > burrowEndRange)
            {
                SetState(State.BURROWING);
            }
        }

        void HandleBurrowingState()
        {
            //burrowing anim

            speed = burrowSpeed;

            ChasePlayer();

            this.alpha = 0.2f;

            if (DistanceTo(target) < burrowEndRange || DistanceTo(target) > burrowStartRange)
            {
                SetState(State.CHASING);
            }
        }

        protected override void OnCollision(GameObject collider)
        {
            // this checks for collisions with tiles
            if (collider is Tiles)
            {
                //do smth
                Move(-speedX, -speedY);
            }

            if (collider is Bullet && state != State.BURROWING)
            {
                collider.LateDestroy();
                EnemyTakeDamage();
            }
        }
    }
}
