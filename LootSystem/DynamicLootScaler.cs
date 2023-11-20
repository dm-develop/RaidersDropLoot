using Il2Cpp;
using MelonLoader;
using System.Diagnostics;
using System.Reflection;
using static Il2Cpp.ResourceManager;

namespace dm.ffmods.raidersdroploot
{
    public class DynamicLootScaler
    {
        #region Fields

        public Dictionary<LootItem, float> LootMultipliers = new Dictionary<LootItem, float>();

        private GameManager gameManager;
        private LootSettingsManager lootSettingsManager;
        private Stopwatch timer = new Stopwatch();

        #endregion Fields

        #region Public Constructors

        public DynamicLootScaler(GameManager gameManager, LootSettingsManager lootSettingsManager)
        {
            this.gameManager = gameManager;
            this.lootSettingsManager = lootSettingsManager;

            // init factor with 1
            foreach (LootItem item in Enum.GetValues(typeof(LootItem)))
            {
                LootMultipliers.Add(item, 1);
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public void CalculateAdjustedLootChances()
        {
            timer.Start();
            foreach (LootItem item in Enum.GetValues(typeof(LootItem)))
            {
                UpdateFactorForItem(item);
            }
            timer.Stop();
            if (Melon<RaidersDropLootMelon>.Instance.Verbose)
            {
                Melon<RaidersDropLootMelon>.Logger.Msg($"updating drop chances took {timer.Elapsed} seconds.");
            }
            timer.Reset();
        }

        #endregion Public Methods

        #region Private Methods

        private float UpdateFactorForItem(LootItem item)
        {
            ResourceManager resManager = gameManager.resourceManager;
            var test = resManager.ironOreItemInfo.minQuota;

            string propName = ItemManager.GetItemInfoName(item);

            // Get the property info using reflection
            PropertyInfo propInfo = resManager.GetType().GetProperty(propName);
            if (propInfo == null)
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"could not find PropertyInfo for: {propName}");
                return 1;
            }

            // Get the value of the property using reflection
            ItemInfo info = (ItemInfo)propInfo.GetValue(resManager);
            if (info == null)
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"could not find ItemInfo for: {propName}");
                return 1;
            }

            var numProduced = info.numProducedThisYear;
            var numLost = info.numLostThisYear;
            var numUnused = info.unusedCount;

            float factor = 1;
            if (numProduced >= lootSettingsManager.ProducedThreshold)
            {
                factor += lootSettingsManager.ProducedPenalty;
            }

            if (numLost >= lootSettingsManager.LostThreshold)
            {
                factor += lootSettingsManager.LostBonus;
            }

            if (numUnused >= lootSettingsManager.UnusedThreshold)
            {
                factor += lootSettingsManager.UnusedPenalty;
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