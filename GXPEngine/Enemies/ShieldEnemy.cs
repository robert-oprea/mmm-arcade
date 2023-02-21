using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

using GXPEngine;

class ShieldEnemy : Enemy
{
    int shieldHealth;
    float speedWithoutShield;

    public ShieldEnemy(TiledObject obj) : base("square.png", 1, 1, obj)
    {
        Initialize(obj);
    }

    protected override void Initialize(TiledObject obj)
    {
        base.Initialize(obj);

        shieldHealth = obj.GetIntProperty("shieldHealth", 1);
        speedWithoutShield = obj.GetFloatProperty("speedWithoutShield", 1.0f);
    }

    protected override void HandleChasingState()
    {
        if (shieldHealth < 1)
        {
            speed = speedWithoutShield;
        }
        base.HandleChasingState();
    }

    public override void EnemyTakeDamage()
    {
        if (shieldHealth >= 1)
        {
            shieldHealth -= 1;
            Console.WriteLine("shield damage taken");
        } 
        else if (health >= 1)
        {
            health -= 1;
            Console.WriteLine("health damage taken");
        } 
        
        if (health < 1)
        {
            LateDestroy();
        }
    }
}

