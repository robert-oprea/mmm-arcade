using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TiledMapParser;
using GXPEngine.Core;

namespace GXPEngine
{
    public class Enemy : AnimationSprite
    {
        protected float speed;
        protected float triggerRange;
        protected float health;

        protected float speedX;
        protected float speedY;

        float knockBackAngle;
        float knockBackStartTime;
        protected float knockBackSpeed;
        protected float knockBackDuration;

        float flashTimer;
        bool isRed;
        //Pog state machine
        protected enum State
        {
            IDLE,
            CHASING,
            EXPLODING,
            BURROWING,
            KNOCKBACKED,
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

            triggerRange = obj.GetFloatProperty("triggerRange", 200.0f);
            speed = obj.GetFloatProperty("speed", 1f);
            knockBackDuration = obj.GetFloatProperty("knockBackDuration", 1.0f) * 1000;
            knockBackSpeed = obj.GetFloatProperty("knockBackSpeed", 4.0f);
            health = obj.GetIntProperty("health", 1);
        }

        protected void Update()
        {
            if (target != null)
            {
                HandleState();
            }

            Animate(0.1f);
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
                case State.KNOCKBACKED:
                    HandleKnockbackedState();
                    break;
            }
        }

        protected void HandleKnockbackedState()
        {
            float vx = (float)Math.Cos(knockBackAngle) * knockBackSpeed;
            float vy = (float)Math.Sin(knockBackAngle) * knockBackSpeed;

            x += vx;
            y += vy;

            flashTimer -= 1;

            if (flashTimer <= 1f)
            {
                flashTimer = 25.0f; // reset the timer

                // alternate between SetColor(1.0f, 0, 0) and SetColor(1.0f, 1, 0f, 1, 0f)
                if (isRed)
                {
                    SetColor(1.0f, 1.0f, 1.0f);

                }
                else
                {
                    SetColor(1.0f, 0, 0);
                }

                isRed = !isRed; // toggle the flag for the next time
            }

            if (Time.time > knockBackStartTime + knockBackDuration)
            {
                SetColor(1.0f, 1.0f, 1.0f);

                flashTimer = 0;

                isRed = false;

                SetState(State.IDLE);

                EnemyTakeDamage();
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

        public virtual void EnemyTakeDamage()
        {
            health -= 1;

            if (health < 1)
            {
                LateDestroy();
            }
        }
        SoundChannel enemyHit;
        protected void KnockBack(GameObject bullet)
        {
            if (enemyHit == null)
            {
                enemyHit = new Sound("Enemy hit.mp3").Play();
            }
            knockBackAngle = Mathf.Atan2(bullet.y - y, bullet.x - x) - Mathf.PI;

            knockBackStartTime = Time.time;

            SetState(State.KNOCKBACKED);
        }

        protected virtual void OnCollision(GameObject collider)
        {
            if (collider is Bullet && state != State.KNOCKBACKED)
            {
                KnockBack(collider);
                collider.LateDestroy();
            }
        }
    }
}
