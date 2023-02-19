using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    public class Enemy : AnimationSprite
    {
        protected float speed;
        protected float triggerRange;

        float speedX;
        float speedY;
        //Pog state machine
        protected enum State
        {
            IDLE,
            CHASING,
            EXPLODING,
            BURROWING,
        }

        protected Player target;

        protected State state;

        public Enemy(string Sprite, int columns, int rows, TiledObject obj) : base(Sprite, columns, rows)
        {
            Initialize(obj);
        }

        protected virtual void Initialize(TiledObject obj)
        {
            SetOrigin(width / 2, height / 2);
            scale = 0.8f;

            triggerRange = obj.GetFloatProperty("triggerRange", 200.0f);
            speed = obj.GetFloatProperty("speed", 1f);
        }

        protected void Update()
        {
            if (target != null)
            {
                HandleState();
            }
        }

        protected virtual void HandleState()
        {
            switch (state)
            {
                case State.IDLE:
                    HandleIdleState();
                    break;
                case State.CHASING:
                    HandleChasingState();
                    break;
            }
        }

        protected virtual void HandleIdleState()
        {
            //idle anim?

            if (DistanceTo(target) < triggerRange)
            {
                SetState(State.CHASING);
            }
        }

        protected virtual void HandleChasingState()
        {
            //run anim

            ChasePlayer();

            if (DistanceTo(target) > triggerRange)
            {
                SetState(State.IDLE);
            }
        }

        protected void ChasePlayer()
        {
            // Set a damping factor for the movement so it doesn't go zoom
            float damping = 0.1f;

            // Calculate the difference between the target position and current position in x and y direction
            float xDiff = target.x - x;
            float yDiff = target.y - y;

            // Calculate the magnitude of the difference vector
            float magnitude = Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff);

            // Normalize the difference vector
            xDiff = xDiff / magnitude;
            yDiff = yDiff / magnitude;

            // Apply the damping factor to the movement and multiply it by the speed to get the actual speed of the enemy
            // Using Time.deltaTime so framrate doesn't matter
            speedX = xDiff * damping * speed * Time.deltaTime;
            speedY = yDiff * damping * speed * Time.deltaTime;

            //TL;DR some math stuffs and then we have the speed
            Move(speedX, speedY);

            if (speedX > 0)
            {
                Mirror(true, false);
            }
            else if (speedX < 0)
            {
                Mirror(false, false);
            }
        }

        protected void SetState(State newState)
        {
            if (state != newState)
            {
                state = newState;
            }
        }

        public void SetTarget(Player target)
        {
            this.target = target;
        }

        protected virtual void OnCollision(GameObject collider)
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
                LateDestroy();
            }
        }
    }
}
