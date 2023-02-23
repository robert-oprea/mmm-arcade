using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class Invincibility : Powerup
    {
        float invincibilityDuration;

        public Invincibility(TiledObject obj) : base("invincibility.png", 1, 1, obj)
        {
            Initialize(obj);
        }

        protected override void Initialize(TiledObject obj)
        {
            base.Initialize(obj);
            invincibilityDuration = obj.GetFloatProperty("invincibilityDuration", 1.0f) * 1000;
        }

        public override void PickUp()
        {
            player.invincibilityDuration = invincibilityDuration;
            player.SetState(Player.PlayerState.PICKUPINVINCIBILITY);
            base.PickUp();
        }
    }
}
