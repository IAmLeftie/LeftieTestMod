using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Quest
{
    public class WoFQuestItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            //TODO: Make an actual name for the item
            DisplayName.SetDefault("WoF Quest Item");
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Quest;
            Item.maxStack = 99;
            Item.questItem = true;
        }
    }
}
