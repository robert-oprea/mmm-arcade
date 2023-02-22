using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    public class Player : AnimationSprite //Playerclass with very basic movement and basic collisions
    {
        int health;
        int maxHealth;

        float bulletSpeed;
        float invincibilityFrames;
        float lastDamageTaken;

        float shootCD = 100.0f;
        float normalShootCD;
        float lastShotFired;

        float staggerStart;
        float staggerDuration;

        bool rapidFireOn;
        public float rapidFireDuration;
        public float rapidFireShootCD;

        Sprite playerHitBox;

        HUD hud;

        public enum PlayerState
        {   
            NORMAL,
            STAGGERED,
            TAKINGDAMAGE,
            PICKUPRAPIDFIRE,
            PICKUPHEALTH,
            PICKUPINVINCIBILITY,
        }

        enum ActionState
        {
            SHOOTING, 
            SHIELDING,
        }

        PlayerState state;

        ActionState actionState;
        
        public Player(string Sprite, int columns, int rows, TiledObject obj) : base(Sprite, columns, rows)
        {
            Initialize(obj);
        }

        public void Update()
        {
            if (hud == null)
            {
                hud = game.FindObjectOfType<HUD>();
            }

            HandleCollisions();

            HandleState();
        }

        void HandleState()
        {
            switch (state)
            {
                case PlayerState.NORMAL:
                    HandleNormalState();
                    break;
                case PlayerState.STAGGERED:
                    HandleStaggeredState();
                    break;
                case PlayerState.TAKINGDAMAGE:
                    HandleTakingDamageState();
                    break;
                case PlayerState.PICKUPRAPIDFIRE:
                    HandlePickupRapidFireState();
                    break;
                case PlayerState.PICKUPHEALTH:
                    HandlePickupHealthState();
                    break;
                case PlayerState.PICKUPINVINCIBILITY:
                    HandlePickupInvincibilityState();
                    break;
            }

            switch (actionState)
            {
                case ActionState.SHOOTING:
                    HandleShootingState();
                    break;
                case ActionState.SHIELDING:
                    HandleShieldingState();
                    break;
            }
        }

        void HandleNormalState()
        {
            Playermove();
        }

        void HandleShootingState()
        {
            Shoot();

            if (Input.GetKeyDown(Key.R))
            {
                SetActionState(ActionState.SHIELDING);
            }
        }

        void HandleShieldingState()
        {
            if (Input.GetMouseButtonDown(0) && Time.time > lastShotFired + shootCD)
            {
                Console.WriteLine("Shielding");
            }

            if (Input.GetKeyDown(Key.R))
            {
                SetActionState(ActionState.SHOOTING);
            }
        }

        void HandleStaggeredState()
        {
            if (Time.time > staggerStart + staggerDuration)
            {
                SetState(PlayerState.NORMAL);
            }
        }

        void HandleTakingDamageState()
        {
            Playermove();

            if (Time.time > lastDamageTaken + invincibilityFrames)
            {
                health -= 1;
                lastDamageTaken = Time.time;
                Console.WriteLine("damage taken");
            }

            if (health <= 0)
            {
                ((MyGame)game).LoadLevel("Levels/Placeholder.tmx");
            }

            HandleHud();

            SetState(PlayerState.NORMAL);
        }

        void HandlePickupRapidFireState()
        {
            // TODO: Implement the behavior for the Pickup Rapid Fire state
            rapidFireOn = true;

            SetState(PlayerState.NORMAL);
        }

        void HandlePickupHealthState()
        {
            // TODO: Implement the behavior for the Pickup Health state
            SetState(PlayerState.NORMAL);
        }

        void HandlePickupInvincibilityState()
        {
            // TODO: Implement the behavior for the Pickup Invincibility state
            SetState(PlayerState.NORMAL);
        }


        public void SetState(PlayerState newState)
        {
            if (state != newState)
            {
                state = newState;
            }
        }

        void SetActionState(ActionState newState)
        {
            if (actionState != newState)
            {
                actionState = newState;
            }
        }

        void Initialize(TiledObject obj)
        {
            SetOrigin(width / 2, height / 2);

            x = obj.X;
            y = game.height / 2;

            playerHitBox = new Sprite("square.png");
            playerHitBox.SetOrigin(playerHitBox.width / 2, playerHitBox.height / 2 - 5);
            playerHitBox.alpha = 0;
            playerHitBox.scale = 1f;
            AddChild(playerHitBox);

            health = obj.GetIntProperty("health", 3);
            invincibilityFrames = obj.GetFloatProperty("invincibilityFrames", 0.5f) * 1000;
            bulletSpeed = obj.GetFloatProperty("bulletSpeed", 5.0f);

            shootCD = obj.GetFloatProperty("shootCD", 0.2f) * 1000;
            normalShootCD = shootCD;

            staggerDuration = obj.GetFloatProperty("staggerDuration", 100.0f);

            maxHealth = health;

            _collider = null;
        }

        void Playermove() // controls of the player
        {
            if (Input.GetKeyDown(Key.W) && y - game.height / 3 > 0) // w
            {
                y -= game.height / 3;
                staggerStart = Time.time;
                SetState(PlayerState.STAGGERED);
            }

            if (Input.GetKeyDown(Key.S) && y + game.height / 3 < game.height) // s
            {
                y += game.height / 3;
                staggerStart = Time.time;
                SetState(PlayerState.STAGGERED);
            }
        }

        public void TakeDamage()
        {
            SetState(PlayerState.TAKINGDAMAGE);
        }

        void HandleHud()
        {
            hud.SetHealth((float)health / maxHealth);
        }

        void HandleCollisions()
        {
            GameObject[] collisions = playerHitBox.GetCollisions();
            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i] is Tiles)
                {
                    //do smth
                    TakeDamage();
                }
                
                if (collisions[i] is Enemy)
                {
                    TakeDamage();
                    collisions[i].LateDestroy();
                }
            }
        }
        void Shoot()
        {
            //do the thing
            if (rapidFireOn)
            {
                shootCD = rapidFireShootCD;

                rapidFireDuration -= 1;

                if (rapidFireDuration < 0)
                {
                    rapidFireOn = false;
                    shootCD = normalShootCD;
                }
            }

            if (Input.GetMouseButtonDown(0) && Time.time > lastShotFired + shootCD)
            {
                Bullet bullet = new Bullet(Input.mouseX, Input.mouseY, bulletSpeed, playerHitBox);
                bullet.SetXY(x, y);

                parent.AddChild(bullet);

                lastShotFired = Time.time;
            }
        }
    }
}

