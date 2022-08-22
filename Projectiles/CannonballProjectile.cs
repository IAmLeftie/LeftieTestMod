using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Projectiles
{
    public class CannonballProjectile : GlobalProjectile
    {
        public override void SetDefaults(Projectile projectile)
        {
            if (projectile.type == ProjectileID.CannonballFriendly)
            {
                projectile.penetrate = 2;
            }
        }
    }
}
