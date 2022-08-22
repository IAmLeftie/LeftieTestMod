using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Weapons
{
    public class ShroomDragoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("10% chance to not consume ammo");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 46;
            Item.height = 22;
            Item.value = 34500;
            Item.rare = ItemRarityID.Yellow;

            //Use Properties
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item14;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 43;
            Item.knockBack = 4.75f;
            Item.noMelee = true;
            Item.crit = -2;

            //Gun Properties
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 18.85f;
            Item.useAmmo = ItemID.Cannonball;
        }

        //public override Vector2? HoldoutOffset()
        //{
        //    return new Vector2(-24f, -5f);
        //}

        //Use this to apply the Buccaneer's Bag Attack Speed Buff
        public override float UseSpeedMultiplier(Player player)
        {
            if (player.GetModPlayer<ModPlayerCostume>().hasBuccaneerBag)
            {
                return 1.15f;
            }
            else { return 1f; }
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShroomiteBar, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
