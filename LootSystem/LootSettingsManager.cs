using MelonLoader;

namespace dm.ffmods.raidersdroploot
{
    public class LootSettingsManager
    {
        #region Fields

        public static uint DefaultDropChanceAdjustmentIntervalInSeconds = 300;
        public static float DefaultLostBonus = 0.3f;
        public static uint DefaultLostThreshold = 10;
        public static float DefaultProducedPenality = -0.2f;
        public static uint DefaultProducedThreshold = 10;
        public static float DefaultUnusedPenalty = -0.8f;
        public static uint DefaultUnusedThreshold = 50;
        public static bool DefaultVerbosity = true;

        // pref category for the mod
        public static MelonPreferences_Category RaidersDropLootPrefs;

        public uint DropChanceAdjustmentIntervalInSeconds = DefaultDropChanceAdjustmentIntervalInSeconds;

        public bool IsVerbose = DefaultVerbosity;
        public float LostBonus = DefaultLostBonus;

        public uint LostThreshold = DefaultLostThreshold;

        // list of prefs to ignore
        public List<MelonPreferences_Entry> PrefEntriesToIgnore;

        public float ProducedPenalty = DefaultProducedPenality;
        public uint ProducedThreshold = DefaultProducedThreshold;
        public float UnusedPenalty = DefaultUnusedPenalty;
        public uint UnusedThreshold = DefaultUnusedThreshold;

        private MelonPreferences_Entry<bool> isVerboseEntry;
        private MelonPreferences_Entry<uint> lootUpdateIntervalEntry;
        private MelonPreferences_Entry<float> lostBonusEntry;
        private MelonPreferences_Entry<uint> lostThresholdEntry;
        private MelonPreferences_Entry<float> producedPenaltyEntry;
        private MelonPreferences_Entry<uint> producedThresholdEntry;
        private MelonPreferences_Entry<float> unusedPenaltyEntry;
        private MelonPreferences_Entry<uint> unusedThresholdEntry;

        #endregion Fields

        #region Public Constructors

        public LootSettingsManager(string prefsPath)
        {
            RaidersDropLootPrefs = MelonPreferences.CreateCategory("RaidersDropLoot");
            RaidersDropLootPrefs.SetFilePath(prefsPath);

            var prefs = RaidersDropLootPrefs;

            // set verbosity
            isVerboseEntry = prefs.CreateEntry<bool>("verboseLogging", DefaultVerbosity);

            // set loot update interval
            lootUpdateIntervalEntry = prefs.CreateEntry<uint>("dropChanceAdjustmentIntervalInSeconds", DefaultDropChanceAdjustmentIntervalInSeconds);

            // set lost items entries
            lostBonusEntry = prefs.CreateEntry<float>("lostItemsBonus", DefaultLostBonus);
            lostThresholdEntry = prefs.CreateEntry<uint>("lostItemsThreshold", DefaultLostThreshold);

            // set produced items entries
            producedPenaltyEntry = prefs.CreateEntry<float>("producedItemsPenalty", DefaultProducedPenality);
            producedThresholdEntry = prefs.CreateEntry<uint>("producedItemsThreshold", DefaultProducedThreshold);

            // set unused entries
            unusedPenaltyEntry = prefs.CreateEntry<float>("unusedItemsPenalty", DefaultUnusedPenalty);
            unusedThresholdEntry = prefs.CreateEntry<uint>("unusedItemsThreshold", DefaultUnusedThreshold);

            // add entries to ignore list
            PrefEntriesToIgnore = new List<MelonPreferences_Entry>
            {
              isVerboseEntry,
              lootUpdateIntervalEntry,
              lostBonusEntry,
              lostThresholdEntry,
              producedPenaltyEntry,
              producedThresholdEntry,
              unusedPenaltyEntry,
              unusedThresholdEntry,
            };
        }

        #endregion Public Constructors

        #region Public Methods

        public void UpdateLootSettings()
        {
            // set verbosity
            IsVerbose = isVerboseEntry.Value;

            // set loot update interval
            DropChanceAdjustmentIntervalInSeconds = lootUpdateIntervalEntry.Value;

            // set lost items entries
            LostBonus = lostBonusEntry.Value;
            LostThreshold = lostThresholdEntry.Value;

            // set produced items entries
            ProducedPenalty = producedPenaltyEntry.Value;
            ProducedThreshold = producedThresholdEntry.Value;

            // set unused entries
            UnusedPenalty = unusedPenaltyEntry.Value;
            UnusedThreshold = unusedThresholdEntry.Value;
        }

        #endregion Public Methods
    }
}