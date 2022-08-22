using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Accessories
{
    public class AnnihilatorRoundsAccessory : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Annihilator Rounds");
            Tooltip.SetDefault("Increases damage and knockback of\nbullets and cannonballs by 10%\n\"Raze Hell\"");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1, silver: 33, copper: 33);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //We dont really need to do anything here, but it needs to exist in order for the stuff to
            //GlobalProjectileModifications.cs to function... for some reason
        }
    }
}
