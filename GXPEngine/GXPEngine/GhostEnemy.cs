using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class GhostEnemy : Enemy
    {
        public GhostEnemy(TiledObject obj) : base("ghostEnemy.png", 1, 1, obj)
        {
            Initialize(obj);
        }
    }
}
