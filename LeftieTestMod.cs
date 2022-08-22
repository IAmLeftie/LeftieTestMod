using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace LeftieTestMod
{
	public class LeftieTestMod : Mod
	{
		public static int ReclaimerScripsCurrencyId;

        public override void Load()
        {
            //Register the Reclaimer Scrips as a new currency
            ReclaimerScripsCurrencyId = CustomCurrencyManager.RegisterCurrency(
                new ReclaimerScripsCurrency(ModContent.ItemType<Items.ReclaimerScrip>(), 999L,
                "Reclaimer Scrips"));
        }
    }
}