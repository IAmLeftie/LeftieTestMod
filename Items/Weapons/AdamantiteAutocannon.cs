using LeftieTestMod.Items.Ammo;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Weapons
{
    public class AdamantiteAutocannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 56;
            Item.height = 24;
            Item.value = Item.sellPrice(gold: 3, silver: 20);
            Item.rare = ItemRarityID.LightRed;

            //Use Properties
            Item.useTime = 39;
            Item.useAnimation = 39;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item14;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 31;
            Item.knockBack = 3.35f;
            Item.noMelee = true;
            Item.crit = -2;

            //Gun Properties
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 17.5f;
            Item.useAmmo = ItemID.Cannonball;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-14f, -5f);
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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
