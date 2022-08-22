using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Weapons
{
    public class LooseCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Loose Cannon");
            Tooltip.SetDefault("It reeks of scrumpy");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 38;
            Item.height = 14;
            Item.value = 30500;
            Item.rare = ItemRarityID.Pink;
            Item.scale = 1.2f;

            //Use Properties
            Item.useTime = 39;
            Item.useAnimation = 39;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item14;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 35;
            Item.knockBack = 3.75f;
            Item.noMelee = true;
            Item.crit = -2;

            //Gun Properties
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 17.85f;
            Item.useAmmo = ItemID.Cannonball;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8f, -3);
        }

        //Use this to apply the Buccaneer's Bag Attack Speed Buff
        public override float UseSpeedMultiplier(Player player)
        {
            if (player.GetModPlayer<ModPlayerCostume>().hasBuccaneerBag)
            {
                return 1.15f;
            }
            else { return 1f; }
        }
    }
}
