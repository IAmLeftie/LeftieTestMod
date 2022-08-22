﻿using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Ammo
{
    public class CopperBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 12;
            Item.height = 12;
            Item.value = Item.sellPrice(copper: 4);
            Item.rare = ItemRarityID.White;
            Item.maxStack = 999;
            Item.consumable = true;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 5;
            Item.knockBack = 1;

            //Gun Properties
            Item.shoot = ModContent.ProjectileType<Projectiles.TinBulletProjectile>();
            Item.shootSpeed = 3;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            Recipe.Create(ModContent.ItemType<CopperBullet>(), 70)
                .AddIngredient(ItemID.CopperBar)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
