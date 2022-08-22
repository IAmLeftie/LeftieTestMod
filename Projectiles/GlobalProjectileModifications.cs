using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Projectiles
{
    public class GlobalProjectileModifications : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            //We run through a different check for each possible buff

            //Black Powder
            if (Main.player[projectile.owner].GetModPlayer<ModPlayerCostume>().hasBlackPowder 
                && !Main.player[projectile.owner].GetModPlayer<ModPlayerCostume>().hasBuccaneerBag
                && !Main.player[projectile.owner].GetModPlayer<ModPlayerCostume>().hasOperatorKit)
            {
                if (source is EntitySource_ItemUse_WithAmmo && (Main.player[projectile.owner].HeldItem.useAmmo == AmmoID.Bullet
                    || Main.player[projectile.owner].HeldItem.useAmmo == ItemID.Cannonball))
                {
                    //Main.NewText("Black Powder is working!");
                    //Main.NewText("Old velocity is " + projectile.velocity.X.ToString() + ", " + projectile.velocity.Y.ToString());
                    projectile.velocity += (projectile.velocity * 0.2f);
                    //Main.NewText("New velocity is " + projectile.velocity.X.ToString() + ", " + projectile.velocity.Y.ToString());
                }
            }

            //Annihilator Rounds
            if (Main.player[projectile.owner].GetModPlayer<ModPlayerCostume>().hasAnnihilatorRounds
                && !Main.player[projectile.owner].GetModPlayer<ModPlayerCostume>().hasBuccaneerBag
                && !Main.player[projectile.owner].GetModPlayer<ModPlayerCostume>().hasOperatorKit)
            {
                if (source is EntitySource_ItemUse_WithAmmo && (Main.player[projectile.owner].HeldItem.useAmmo == AmmoID.Bullet
                    || Main.player[projectile.owner].HeldItem.useAmmo == ItemID.Cannonball))
                {
                    //Main.NewText("Annihilator Rounds is working!");
                    //Main.NewText("Old damage is " + projectile.damage);
                    //Main.NewText("Old knockback is " + projectile.knockBack);
                    projectile.damage += (int)(projectile.damage * 0.1f);
                    projectile.knockBack += (projectile.knockBack * 0.1f);
                    //Main.NewText("New damage is " + projectile.damage);
                    //Main.NewText("New knockback is " + projectile.knockBack);
                }
            }

            //Buccaneer Bag
            if (Main.player[projectile.owner].GetModPlayer<ModPlayerCostume>().hasBuccaneerBag)
            {
                if (source is EntitySource_ItemUse_WithAmmo && Main.player[projectile.owner].HeldItem.useAmmo == ItemID.Cannonball)
                {
                    projectile.velocity += (projectile.velocity * 0.25f);
                    projectile.damage += (int)(projectile.damage * 0.15f);
                    projectile.knockBack += (projectile.knockBack * 0.15f);
                }
            }

            //Operator Kit
            if (Main.player[projectile.owner].GetModPlayer<ModPlayerCostume>().hasOperatorKit)
            {
                if (source is EntitySource_ItemUse_WithAmmo && Main.player[projectile.owner].HeldItem.useAmmo == AmmoID.Bullet)
                {
                    projectile.velocity += (projectile.velocity * 0.25f);
                    projectile.damage += (int)(projectile.damage * 0.15f);
                    projectile.knockBack += (projectile.knockBack * 0.15f);
                    //If the ammo type is NOT Luminite Bullets (because for some reason that's broken with this)
                    if (source is not EntitySource_ItemUse_WithAmmo { AmmoItemIdUsed: 3567 })
                    {
                        projectile.penetrate += 2;
                        projectile.localNPCHitCooldown = -1;
                        projectile.usesLocalNPCImmunity = true;
                    }
                }
            }
        }
    }
}
