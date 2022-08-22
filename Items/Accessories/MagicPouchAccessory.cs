using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Accessories
{
    public class MagicPouchAccessory : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Pouch");
            Tooltip.SetDefault("10% chance not to consume bullets and cannonballs");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1, silver: 33, copper: 33);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //We dont really need to do anything here, but it needs to exist in order for the stuff to
            //GlobalProjectileModifications.cs to function... for some reason
        }
    }
}
