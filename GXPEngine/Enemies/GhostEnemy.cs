using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

using GXPEngine;

class GhostEnemy : Enemy
{
    public GhostEnemy(TiledObject obj) : base("ghosprite.png", 4, 2, obj)
    {
        Initialize(obj);
    }

    protected override void OnCollision(GameObject collider)
    {
        if (collider is Bullet && state != State.KNOCKBACKED)
        {
            KnockBack(collider);
            collider.LateDestroy();
        }
    }

    protected override void HandleIdleState()
    {
        SetCycle(0, 3);

        base.HandleIdleState();
    }

    protected override void HandleChasingState()
    {
        SetCycle(4, 7);

        base.HandleChasingState();
    }
}

