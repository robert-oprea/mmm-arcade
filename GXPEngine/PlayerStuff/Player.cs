using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    public class Player : AnimationSprite 
    {
        public int health;
        int maxHealth;

        SoundChannel moving;

        String currentPowerUp;

        float damageFlashDuration = 400;
        float damageFlashTime;

        float bulletSpeed;
        float invincibilityFrames;
        float lastDamageTaken;

        float shootCD;
        float normalShootCD;
        float lastShotFired;

        float staggerStart;
        float staggerDuration;

        float shieldCD;
        float internalShieldCD;

        bool rapidFireOn;
        public float rapidFireDuration;
        public float rapidFireShootCD;

        public float invincibilityDuration;
        float invincibilityTime;
        bool invincible;

        int score;
        float lastScoreIncrease;

        Sprite playerHitBox;
        
        CrossHair crossHair;

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
        SoundChannel bgMusic;
        int scoreIncreaseAmount = 1;
        int scoreThreshold;
        public void Update()
        {
            y += movementSpeed;
            if (Time.time > staggerStart + staggerDuration)
            {
                movementSpeed = 0;
            }
            UsePowerUP();

            if (moving == null)
            {
                moving = new Sound("Minecart.mp3", true).Play();
                moving.Volume = 0.6f;
            }

            if (bgMusic == null)
            {
                bgMusic = new Sound("Background3.mp3", true, true).Play();
                bgMusic.Volume = 0.7f;
            }

            if (hud == null)
            {
                hud = game.FindObjectOfType<HUD>();
            }

            if (parent != null && crossHair == null)
            {
                crossHair = new CrossHair();
                crossHair.SetOrigin(playerHitBox.width / 2, playerHitBox.height / 2);
                parent.AddChild(crossHair);
            }

            if (Time.time > lastScoreIncrease + 25)
            {
                score += scoreIncreaseAmount;
                lastScoreIncrease = Time.time;
            }

            if (score > scoreThreshold)
            {
                scoreIncreaseAmount += 1;
                scoreThreshold += 1000;
            }

            HandleHud();

            HandleAnimation();

            HandleCollisions();

            HandleState();
        }

        void HandleAnimation()
        {
            if (Time.time < damageFlashTime + damageFlashDuration)
            {
                SetColor(255, 0, 0);
            }
            else
            {
                SetColor(255, 255, 255);
            }

            Animate(0.1f);

            float directionX = crossHair.x - x;
            float directionY = crossHair.y - y;

            float angle = (float)Math.Atan2(directionY, directionX);

            switch (actionState)
            {
                case ActionState.SHOOTING:
                    int[] shootCycles = { 6, 5, 4, 3, 2, 1, 0, 7, 6 };
                    int shootCycleIndex = (int)Math.Round(angle / 0.75) + 4;
                    SetCycle(shootCycles[shootCycleIndex]);
                    break;
                case ActionState.SHIELDING:
                    int[] blockCycles = { 14, 13, 12, 11, 10, 9, 8, 15, 14 };
                    int blockCycleIndex = (int)Math.Round(angle / 0.75) + 4;
                    SetCycle(blockCycles[blockCycleIndex]);
                    break;
            }
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

            if (Time.time > invincibilityTime + invincibilityDuration)
            {
                invincible = false;
            }
        }

        void HandleShootingState()
        {
            Shoot();

            if (internalShieldCD > 0)
            {
                internalShieldCD -= 1;
            }

            if (Input.GetKeyDown(Key.R) && internalShieldCD <= 0)
            {
                SetActionState(ActionState.SHIELDING);
            }
        }

        void HandleShieldingState()
        {
            if (Input.GetKeyDown(Key.R))
            {
                SetActionState(ActionState.SHOOTING);
            }
        }

        void HandleStaggeredState()
        {
            if (Time.time > staggerStart + staggerDuration)
            {
                movementSpeed = 0;
                SetState(PlayerState.NORMAL);
            }
        }

        void HandleTakingDamageState()
        {
            Playermove();
            damageFlashTime = Time.time;
            if (Time.time > lastDamageTaken + invincibilityFrames && actionState != ActionState.SHIELDING)
            {
                health -= 1;
                lastDamageTaken = Time.time;
                SoundChannel hit = new Sound("hit.mp3").Play();
            } 
            else if (actionState == ActionState.SHIELDING)
            {
                SetActionState(ActionState.SHOOTING);

                internalShieldCD = shieldCD;
            }

            if (health <= 0)
            {
                moving.Stop();
                bgMusic.Stop();
                ((MyGame)game).LoadLevel("Levels/Start Menu.tmx");
            }

            SetState(PlayerState.NORMAL);
        }

        void HandlePickupRapidFireState()
        {
            // TODO: Implement the behavior for the Pickup Rapid Fire state


            currentPowerUp = "rapidFire";

            hud.displayPowerUP("rapidFire");

            SetState(PlayerState.NORMAL);
        }

        void HandlePickupHealthState()
        {
            // TODO: Implement the behavior for the Pickup Health state

            currentPowerUp = "healthPot";

            hud.displayPowerUP("healthPot");

            SetState(PlayerState.NORMAL);
        }

        void HandlePickupInvincibilityState()
        {
            // TODO: Implement the behavior for the Pickup Invincibility state
            currentPowerUp = "invincibility";

            hud.displayPowerUP("invincibility");

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
        
        void UsePowerUP()
        {
            if (Input.GetKeyDown(Key.T))
            {
                if (currentPowerUp == "invincibility")
                {
                    invincibilityTime = Time.time;
                    invincible = true;

                    hud.displayPowerUP("");
                }

                if (currentPowerUp == "healthPot")
                {
                    health += 1;

                    if (health > maxHealth)
                    {
                        health = maxHealth;
                    }

                    hud.displayPowerUP("");
                }

                if (currentPowerUp == "rapidFire")
                {
                    rapidFireOn = true;

                    hud.displayPowerUP("");
                }

                currentPowerUp = "";
            }

        }

        void Initialize(TiledObject obj)
        {
            SetOrigin(width / 2, height / 2);

            x = obj.X;
            y = obj.Y;

            playerHitBox = new Sprite("square.png");
            playerHitBox.SetOrigin(playerHitBox.width / 2, playerHitBox.height / 2 - 40);
            playerHitBox.alpha = 0;
            playerHitBox.scale = 0.7f;
            AddChild(playerHitBox);

            health = obj.GetIntProperty("health", 3);
            invincibilityFrames = obj.GetFloatProperty("invincibilityFrames", 0.5f) * 1000;
            bulletSpeed = obj.GetFloatProperty("bulletSpeed", 5.0f);

            shootCD = obj.GetFloatProperty("shootCD", 0.2f) * 1000;
            normalShootCD = shootCD;

            shieldCD = obj.GetFloatProperty("shieldCD", 1.0f) * 100;

            staggerDuration = obj.GetFloatProperty("staggerDuration", 100.0f) * 1000;

            maxHealth = health;

            _collider = null;
        }

        float movementSpeed;

        void Playermove() // controls of the player
        {
            if (Input.GetKeyDown(Key.W) && y - 96 > 120) // w
            {
                movementSpeed -= 5;
                staggerStart = Time.time;
                SetState(PlayerState.STAGGERED);
            }

            if (Input.GetKeyDown(Key.S) && y + 96 < 400) // s
            {
                movementSpeed += 5;
                staggerStart = Time.time;
                SetState(PlayerState.STAGGERED);
            }
        }

        public void TakeDamage()
        {
            if (invincible == false)
            {
                SetState(PlayerState.TAKINGDAMAGE);
            }
        }

        void HandleHud()
        {
            hud.SetHealth(health);

            hud.SetScore(score);
        }

        void HandleCollisions()
        {
            GameObject[] collisions = playerHitBox.GetCollisions();
            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i] is Tiles && state != PlayerState.STAGGERED)
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
                Bullet bullet = new Bullet(crossHair.x, crossHair.y, bulletSpeed, playerHitBox);
                bullet.SetXY(x, y);

                parent.AddChild(bullet);

                lastShotFired = Time.time;

                SoundChannel shoot = new Sound("Shooting.mp3").Play();
                shoot.Volume = 0.3f;
            }
        }
    }
}

