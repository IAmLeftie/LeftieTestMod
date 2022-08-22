using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.Localization;

namespace LeftieTestMod
{
    public class ReclaimerScripsCurrency : CustomCurrencySingleCoin
    {
        public ReclaimerScripsCurrency(int coinItemID, long currencyCap, string CurrencyTextKey) 
            : base(coinItemID, currencyCap)
        {
            this.CurrencyTextKey = CurrencyTextKey;
            CurrencyTextColor = Color.Violet;
        }
    }
}
