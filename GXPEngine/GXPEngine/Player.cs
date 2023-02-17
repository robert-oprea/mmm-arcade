using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    public class Player : AnimationSprite //Playerclass with very basic movement and basic collisions
    {
        float bulletSpeed;

        int health;
        int maxHealth;

        float invincibilityFrames = 1000.0f;
        float lastDamageTaken;

        Sprite playerHitBox;

        HUD hud;

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

            Shoot();

            Playermove();

            HandleCollisions();
        }

        void Initialize(TiledObject obj)
        {
            SetOrigin(width / 2, height / 2);

            playerHitBox = new Sprite("square.png");
            playerHitBox.SetOrigin(playerHitBox.width / 2, playerHitBox.height / 2 - 5);
            playerHitBox.alpha = 0;
            playerHitBox.scale = 1f;
            AddChild(playerHitBox);

            health = obj.GetIntProperty("health", 3);
            invincibilityFrames = obj.GetFloatProperty("invincibilityFrames", 500.0f);
            bulletSpeed = obj.GetFloatProperty("bulletSpeed", 5.0f);

            maxHealth = health;

            _collider = null;
        }

        void Playermove() // controls of the player
        {
            if (Input.GetKeyDown(Key.W)) // w
            {
                y -= 150;
            }

            if (Input.GetKeyDown(Key.S)) // s
            {
                y += 150;
            }
        }

        public void TakeDamage()
        {
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
            if (Input.GetMouseButtonDown(0))
            {
                Bullet bullet = new Bullet(Input.mouseX, Input.mouseY, bulletSpeed, playerHitBox);
                bullet.SetXY(x, y);

                parent.AddChild(bullet);
            }
        }
    }
}

