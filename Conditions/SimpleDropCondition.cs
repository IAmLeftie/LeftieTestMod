using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace LeftieTestMod.Conditions
{
    public class SimpleDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (!info.IsInSimulation)
            {
                return true;
            }
            return false;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Can always drop";
        }
    }
}
