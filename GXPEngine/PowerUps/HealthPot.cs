using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class HealthPot : Powerup
    {
        int healAmount;

        public HealthPot(TiledObject obj) : base("healthPot.png", 1, 1, obj)
        {
            Initialize(obj);
        }

        protected override void Initialize(TiledObject obj)
        {
            base.Initialize(obj);
            healAmount = obj.GetIntProperty("healAmount", 1);
        }

        public override void PickUp()
        {
            player.health += healAmount;
            player.SetState(Player.PlayerState.PICKUPHEALTH);
            base.PickUp();
        }
    }
}
