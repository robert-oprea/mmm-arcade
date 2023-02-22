using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class RapidFire : Powerup
    {
        float rapidFireDuration;
        float rapidFireShootCD;

        public RapidFire(TiledObject obj) : base("rapid_fire.png", 1, 1, obj)
        {
            Initialize(obj);
        }

        protected override void Initialize(TiledObject obj)
        {
            base.Initialize(obj);
            rapidFireShootCD = obj.GetFloatProperty("rapidFireShootCD", 0.1f) * 1000;
            rapidFireDuration = obj.GetFloatProperty("rapidFireDuration", 5.0f) * 1000;
        }

        public override void PickUp()
        {
            player.rapidFireDuration = rapidFireDuration;
            player.rapidFireShootCD = rapidFireShootCD;
            player.SetState(Player.PlayerState.PICKUPRAPIDFIRE);
            base.PickUp();
        }
    }
}
