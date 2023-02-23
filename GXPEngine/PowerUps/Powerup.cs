using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class Powerup : AnimationSprite
    {
        protected Player player;

        public Powerup(string Sprite, int columns, int rows, TiledObject obj) : base(Sprite, columns, rows)
        {
            Initialize(obj);
        }

        protected virtual void Initialize(TiledObject obj)
        {
            SetOrigin(width / 2, height / 2);

            scale = scale * 1.3f;
        }

        public virtual void PickUp()
        {
            Console.WriteLine("Pickup grabbed");

            _collider = null;

            LateDestroy();
        }

        protected void Update()
        {
            if (player == null)
            {
                player = parent.FindObjectOfType<Player>();
            }
        }

        protected void OnCollision(GameObject collider)
        {
            if (collider is Bullet)
            {
                collider.LateDestroy();
                PickUp();
            }

            if (collider.parent is BombEnemy)
            {
                LateDestroy();
            }
        }
    }
}
