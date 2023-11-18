using Il2Cpp;
using MelonLoader;
using System.Reflection;
using static Il2Cpp.ResourceManager;

namespace dm.ffmods.raidersdroploot
{
    public class DynamicLootScaler
    {
        #region Fields

        public Dictionary<LootItem, float> lootMultipliers = new Dictionary<LootItem, float>();
        public float LostBonus = 0.2f;
        public uint LostThreshold = 10;
        public uint ProducedThreshold = 10;
        public float ProductionPenality = 0.2f;
        public float UnusedPenalty = 0.5f;
        public uint UnusedThreshold = 10;

        private GameManager gameManager;

        #endregion Fields

        #region Public Constructors

        public DynamicLootScaler(GameManager gameManager)
        {
            this.gameManager = gameManager;

            // init factor with 1
            foreach (LootItem item in Enum.GetValues(typeof(LootItem)))
            {
                lootMultipliers.Add(item, 1);
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public void CalculateAdjustedLootChances()
        {
            foreach (LootItem item in Enum.GetValues(typeof(LootItem)))
            {
                UpdateFactorForItem(item);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private float UpdateFactorForItem(LootItem item)
        {
            ResourceManager resManager = gameManager.resourceManager;

            string propName = $"{item}ItemInfo";
            Melon<RaidersDropLootMelon>.Logger.Msg($"working on prop: {propName}");

            // Get the property info using reflection
            PropertyInfo propInfo = resManager.GetType().GetProperty(propName);
            Melon<RaidersDropLootMelon>.Logger.Msg($"prop name: {propInfo.Name}");

            // Get the value of the property using reflection
            ItemInfo info = (ItemInfo)propInfo.GetValue(resManager);
            Melon<RaidersDropLootMelon>.Logger.Msg($"info item name: {info.item.name}");

            var numProduced = info.numProducedThisYear;
            var numLost = info.numLostThisYear;
            var numUnused = info.unusedCount;

            float factor = 1;
            if (numProduced >= ProducedThreshold)
            {
                factor -= ProductionPenality;
            }

            if (numLost >= LostThreshold)
            {
                factor += LostBonus;
            }

            if (numUnused >= UnusedThreshold)
            {
                factor -= UnusedPenalty;
            }

            if (factor < 0f) { factor = 0f; }

            if (Melon<RaidersDropLootMelon>.Instance.Verbose)
            {
                Melon<RaidersDropLootMelon>.Logger.Msg($"updating loot chance multiplier for {item}, new factor is: {factor}");
            }

            return factor;
        }

        #endregion Private Methods
    }
}