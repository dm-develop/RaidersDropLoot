using MelonLoader;

namespace dm.ffmods.raidersdroploot
{
    public class LootSettingsManager
    {
        #region Fields

        // pref category for the mod
        public static MelonPreferences_Category LootPrefs;

        public static MelonPreferences_Category LootScalerPrefs;
        public static MelonPreferences_Category SetupPrefs;

        public uint DropChanceAdjustmentIntervalInSeconds = 300;
        public bool IsVerbose = false;
        public int LostBonusInPercent = 30;
        public uint LostThreshold = 10;

        // list of prefs to ignore
        public List<MelonPreferences_Entry> PrefEntriesToIgnore;

        public int ProducedPenaltyInPercent = -20;
        public uint ProducedThreshold = 10;
        public int UnusedPenaltyInPercent = -80;
        public uint UnusedThreshold = 50;
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
            // set up categories
            LootScalerPrefs = MelonPreferences.CreateCategory("LootScaling");
            LootScalerPrefs.SetFilePath(prefsPath);
            LootPrefs = MelonPreferences.CreateCategory("LootTables");
            LootPrefs.SetFilePath(prefsPath);
            SetupPrefs = MelonPreferences.CreateCategory("Setup");
            SetupPrefs.SetFilePath(prefsPath);

            // set verbosity
            isVerboseEntry = SetupPrefs.CreateEntry<bool>("verboseLogging", IsVerbose);

            // set loot update interval
            lootUpdateIntervalEntry = LootScalerPrefs.CreateEntry<uint>("dropChanceAdjustmentIntervalInSeconds", DropChanceAdjustmentIntervalInSeconds);

            // set lost items entries
            lostBonusEntry = LootScalerPrefs.CreateEntry<int>("lostItemsBonusInPercentPoints", LostBonusInPercent);
            lostThresholdEntry = LootScalerPrefs.CreateEntry<uint>("lostItemsThreshold", LostThreshold);

            // set produced items entries
            producedPenaltyEntry = LootScalerPrefs.CreateEntry<int>("producedItemsPenaltyInPercentPoints", ProducedPenaltyInPercent);
            producedThresholdEntry = LootScalerPrefs.CreateEntry<uint>("producedItemsThreshold", ProducedThreshold);

            // set unused entries
            unusedPenaltyEntry = LootScalerPrefs.CreateEntry<int>("unusedItemsPenaltyInPercentPoints", UnusedPenaltyInPercent);
            unusedThresholdEntry = LootScalerPrefs.CreateEntry<uint>("unusedItemsThreshold", UnusedThreshold);

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