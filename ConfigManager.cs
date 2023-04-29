using MelonLoader;

namespace dm.ffmods.raidersdroploot
{
    public class ConfigManager
    {
        #region Fields

        public static MelonPreferences_Category lootPrefs;
        private const string configPath = "UserData/RaidersDropLootConfig.cfg";
        private bool isInitialised = false;
        private LootRoller lootRoller;

        #endregion Fields

        #region Properties

        public bool IsInitialised { get => isInitialised; }

        #endregion Properties

        #region Public Methods

        public void InitConfig(LootRoller lootRoller)
        {
            this.lootRoller = lootRoller;

            lootPrefs = MelonPreferences.CreateCategory("RaidersDropLoot");
            lootPrefs.SetFilePath(configPath, autoload: false);

            // Manually load the category data
            lootPrefs.LoadFromFile();

            // check if all prefs exist
            var missingPrefs = CheckForMissingPrefs(lootPrefs.Entries);

            // create missing
            if (missingPrefs.Any())
            {
                foreach (var key in missingPrefs)
                {
                    Melon<RaidersDropLootMelon>.Logger.Warning($"detected missing Pref for '{key}'," +
                        $" creating new one from default loot table.");
                    lootPrefs.CreateEntry<SerialisiableLootTable>(
                        key.ToString(),
                        lootRoller.LootTables[key].MakeSerialisable(),
                        key.ToString(),
                        false);
                }
                // save updated config
                lootPrefs.SaveToFile(true);
            }
            isInitialised = true;
        }

        public void UpdateLootTablesfromConfig()
        {
            if (!isInitialised)
            {
                return;
            }
            // load prefs from file
            lootPrefs.LoadFromFile(true);

            // update all tables
            foreach (var entry in lootPrefs.Entries)
            {
                RaiderType type = Enum.Parse<RaiderType>(entry.Identifier);
                var newTable = (SerialisiableLootTable)entry.BoxedValue;
                Melon<RaidersDropLootMelon>.Logger.Msg($"read pref for '{newTable.RaiderType}' with {newTable.Drops.Count} drops!");
                lootRoller.UpdateLootTable(type, newTable.Deserialise());
            }
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<RaiderType> CheckForMissingPrefs(List<MelonPreferences_Entry> entries)
        {
            var prefNames = entries.Select(p => Enum.Parse<RaiderType>(p.Identifier)).ToList();
            foreach (var pref in prefNames) { }
            return lootRoller.LootTables.Keys.Except(prefNames);
        }

        #endregion Private Methods
    }
}