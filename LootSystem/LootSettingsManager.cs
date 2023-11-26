using MelonLoader;

namespace dm.ffmods.raidersdroploot
{
    public class LootSettingsManager
    {
        #region Fields

        public static uint DefaultDropChanceAdjustmentIntervalInSeconds = 300;
        public static int DefaultLostBonus = 30;
        public static uint DefaultLostThreshold = 10;
        public static int DefaultProducedPenality = -20;
        public static uint DefaultProducedThreshold = 10;
        public static int DefaultUnusedPenalty = -80;
        public static uint DefaultUnusedThreshold = 50;
        public static bool DefaultVerbosity = false;

        // pref category for the mod
        public static MelonPreferences_Category RaidersDropLootPrefs;

        public uint DropChanceAdjustmentIntervalInSeconds = DefaultDropChanceAdjustmentIntervalInSeconds;

        public bool IsVerbose = DefaultVerbosity;
        public int LostBonusInPercent = DefaultLostBonus;

        public uint LostThreshold = DefaultLostThreshold;

        // list of prefs to ignore
        public List<MelonPreferences_Entry> PrefEntriesToIgnore;

        public int ProducedPenaltyInPercent = DefaultProducedPenality;
        public uint ProducedThreshold = DefaultProducedThreshold;
        public int UnusedPenaltyInPercent = DefaultUnusedPenalty;
        public uint UnusedThreshold = DefaultUnusedThreshold;

        private MelonPreferences_Entry<bool> isVerboseEntry;
        private MelonPreferences_Entry<uint> lootUpdateIntervalEntry;
        private MelonPreferences_Entry<int> lostBonusEntry;
        private MelonPreferences_Entry<uint> lostThresholdEntry;
        private MelonPreferences_Entry<int> producedPenaltyEntry;
        private MelonPreferences_Entry<uint> producedThresholdEntry;
        private MelonPreferences_Entry<int> unusedPenaltyEntry;
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
            lostBonusEntry = prefs.CreateEntry<int>("lostItemsBonusInPercentPoints", DefaultLostBonus);
            lostThresholdEntry = prefs.CreateEntry<uint>("lostItemsThreshold", DefaultLostThreshold);

            // set produced items entries
            producedPenaltyEntry = prefs.CreateEntry<int>("producedItemsPenaltyInPercentPoints", DefaultProducedPenality);
            producedThresholdEntry = prefs.CreateEntry<uint>("producedItemsThreshold", DefaultProducedThreshold);

            // set unused entries
            unusedPenaltyEntry = prefs.CreateEntry<int>("unusedItemsPenaltyInPercentPoints", DefaultUnusedPenalty);
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
            LostBonusInPercent = lostBonusEntry.Value;
            LostThreshold = lostThresholdEntry.Value;

            // set produced items entries
            ProducedPenaltyInPercent = producedPenaltyEntry.Value;
            ProducedThreshold = producedThresholdEntry.Value;

            // set unused entries
            UnusedPenaltyInPercent = unusedPenaltyEntry.Value;
            UnusedThreshold = unusedThresholdEntry.Value;
        }

        #endregion Public Methods
    }
}