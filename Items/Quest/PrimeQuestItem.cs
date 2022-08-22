using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Quest
{
    public class PrimeQuestItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            //TODO: Make an actual name for the item
            DisplayName.SetDefault("Prime Quest Item");
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 32;
            Item.height = 28;
            Item.rare = ItemRarityID.Quest;
            Item.maxStack = 99;
            Item.questItem = true;
        }
    }
}
