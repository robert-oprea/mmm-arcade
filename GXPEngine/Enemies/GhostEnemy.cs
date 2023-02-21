using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

using GXPEngine;

class GhostEnemy : Enemy
{
    public GhostEnemy(TiledObject obj) : base("ghostEnemy.png", 1, 1, obj)
    {
        Initialize(obj);
    }

    protected override void OnCollision(GameObject collider)
    {
        if (collider is Bullet)
        {
            KnockBack(collider);
            collider.LateDestroy();
            EnemyTakeDamage();
        }
    }
}

