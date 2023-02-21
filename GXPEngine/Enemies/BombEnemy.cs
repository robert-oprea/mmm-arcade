using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

using GXPEngine;

class BombEnemy : Enemy
{
    int explosionRadius;
    float explosionTriggerRange;
    float explosionTime;
    float explosionDuration;

    bool exploded;
    bool hasHitPlayer;

    Sprite explosionHitbox;

    public BombEnemy(TiledObject obj) : base("bombEnemy.png", 1, 1, obj)
    {
        Initialize(obj);
    }

    protected override void Initialize(TiledObject obj)
    {
        base.Initialize(obj);

        explosionRadius = obj.GetIntProperty("explosionRadius", 100);
        explosionTriggerRange = obj.GetFloatProperty("explosionTriggerRange", 30f);
        explosionDuration = obj.GetFloatProperty("explosionDuration", 1.0f) * 1000;
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
            case State.EXPLODING:
                HandleExplodingState();
                break;
            case State.KNOCKBACKED:
                HandleKnockbackedState();
                break;
        }
    }

    protected override void HandleChasingState()
    {
        base.HandleChasingState();

        if (x < explosionTriggerRange)
        {
            SetState(State.EXPLODING);
        }
    }

    void HandleExplodingState()
    {
        //explosion anim

        if (exploded == false)
        {
            //this is just the hitbox, alpha should == 0 when we have an animation
            explosionHitbox = new Sprite("circle.png");
            explosionHitbox.SetOrigin(explosionHitbox.width / 2, explosionHitbox.height / 2 - 5);
            explosionHitbox.alpha = 1;
            explosionHitbox.width = explosionRadius;
            explosionHitbox.height = explosionRadius;
            AddChild(explosionHitbox);

            exploded = true;

            explosionTime = Time.time;

            _collider = null;
        }

        GameObject[] collisions = explosionHitbox.GetCollisions();
        for (int i = 0; i < collisions.Length; i++)
        {
            if (collisions[i].parent is Player && hasHitPlayer == false)
            {
                //do smth
                target.TakeDamage();

                hasHitPlayer = true;
            }

            if (collisions[i] is Enemy)
            {
                Enemy enemy = collisions[i] as Enemy;
                enemy.EnemyTakeDamage();
            }
        }

        //LateDestroy() after animation plays. no anim yet
        if (Time.time > explosionTime + explosionDuration)
        {
            LateDestroy();
        }
    }

    public override void EnemyTakeDamage()
    {
        health -= 1;

        if (health < 1)
        {
            SetState(State.EXPLODING);
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

        if (collider is Bullet)
        {
            KnockBack(collider);

            collider.LateDestroy();

            EnemyTakeDamage();
        }
    }
}

