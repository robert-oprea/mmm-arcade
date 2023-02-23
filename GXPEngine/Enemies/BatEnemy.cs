using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

using GXPEngine;

class BatEnemy : Enemy
{
    public BatEnemy(TiledObject obj) : base("bat.png", 4, 1, obj)
    {
        Initialize(obj);
    }
}

