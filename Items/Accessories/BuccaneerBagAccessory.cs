using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Accessories
{
    public class BuccaneerBagAccessory : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Buccaneer's Bag");
            Tooltip.SetDefault("Increases velocity of cannonballs by 25%" +
                "\nIncreases damage and knockback of cannonballs by 15%" +
                "\n15% chance not to consume cannonballs" +
                "\nIncreases handcannon fire speed by 15%" +
                "\nDoes not stack with its ingredients");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 23;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 1, silver: 33, copper: 33);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //We dont really need to do anything here, but it needs to exist in order for the stuff to
            //GlobalProjectileModifications.cs to function... for some reason
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<BlackPowderAccessory>())
                .AddIngredient(ModContent.ItemType<AnnihilatorRoundsAccessory>())
                .AddIngredient(ModContent.ItemType<MagicPouchAccessory>())
                .AddIngredient(ItemID.Cannonball, 150)
                .AddIngredient(ItemID.HallowedBar, 3)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
