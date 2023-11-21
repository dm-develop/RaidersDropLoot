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
            ResourceManager resManager = gameManager.resourceManager;

            if (resManager == null)
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"could not find Resourcemanager, skipping loot adjustment");
                return;
            }

            timer.Start();

            foreach (LootItem item in Enum.GetValues(typeof(LootItem)))
            {
                UpdateFactorForItem(item, resManager);
            }
            timer.Stop();
            if (Melon<RaidersDropLootMelon>.Instance.Verbose)
            {
                Melon<RaidersDropLootMelon>.Logger.Msg($"updating drop rate adjustments took {timer.ElapsedMilliseconds} milliseconds.");
            }
            timer.Reset();
        }

        #endregion Public Methods

        #region Private Methods

        private void UpdateFactorForItem(LootItem item, ResourceManager resManager)
        {
            string propName = ItemManager.GetItemInfoName(item);

            // Get the property info using reflection
            PropertyInfo propInfo = resManager.GetType().GetProperty(propName);
            if (propInfo == null)
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"could not find PropertyInfo for: {propName}");
                return;
            }

            // Get the value of the property using reflection
            ItemInfo info = (ItemInfo)propInfo.GetValue(resManager);
            if (info == null)
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"could not find ItemInfo for: {propName}");
                return;
            }

            var numProduced = info.numProducedThisYear;
            var numLost = info.numLostThisYear;
            var numUnused = info.unusedCount;

            float factor = 1f;
            if (numProduced >= lootSettingsManager.ProducedThreshold)
            {
                factor += (float)lootSettingsManager.ProducedPenaltyInPercent / 100;
                //Melon<RaidersDropLootMelon>.Logger.Warning($"applying production penalty of {lootSettingsManager.ProducedPenaltyInPercent}%. new factor is: {factor}");
            }

            if (numLost >= lootSettingsManager.LostThreshold)
            {
                factor += (float)lootSettingsManager.LostBonusInPercent / 100;
                //Melon<RaidersDropLootMelon>.Logger.Warning($"applying lost bonus of {lootSettingsManager.LostBonusInPercent}%. new factor is: {factor}");
            }

            if (numUnused >= lootSettingsManager.UnusedThreshold)
            {
                factor += (float)lootSettingsManager.UnusedPenaltyInPercent / 100;
                //Melon<RaidersDropLootMelon>.Logger.Warning($"applying unused penalty of {lootSettingsManager.UnusedPenaltyInPercent}%. new factor is: {factor}");
            }

            if (factor < 0f) { factor = 0f; }

            var oldFactor = LootMultipliers[item];
            LootMultipliers[item] = factor;

            if (Melon<RaidersDropLootMelon>.Instance.Verbose)
            {
                Melon<RaidersDropLootMelon>.Logger.Msg($"updating drop chance adjustments for '{item}': old factor was {oldFactor}, new factor is {LootMultipliers[item]}");
            }
        }

        #endregion Private Methods
    }
}