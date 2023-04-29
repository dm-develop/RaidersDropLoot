using MelonLoader;

namespace dm.ffmods.raidersdroploot
{
    public class ConfigManager
    {
        #region Fields

        public static MelonPreferences_Category lootPrefs;
        private const string configPath = "UserData/RaidersDropLootConfig.cfg";
        private bool isInitialised = false;
        private LootManager lootRoller;

        #endregion Fields

        #region Properties

        public bool IsInitialised { get => isInitialised; }

        #endregion Properties

        #region Public Methods

        public void InitConfig(LootManager lootRoller)
        {
            this.lootRoller = lootRoller;

            lootPrefs = MelonPreferences.CreateCategory("RaidersDropLoot");
            lootPrefs.SetFilePath(configPath);

            CreateEntries();

            UpdateTablesFromPrefs();

            isInitialised = true;
        }

        #endregion Public Methods

        #region Private Methods

        private void CreateEntries()
        {
            foreach (var item in lootRoller.LootTables)
            {
                _ = CreateMelonPref(item.Value);
            }
        }

        private MelonPreferences_Entry CreateMelonPref(LootTable table)
        {
            var _table = table.MakeSerialisable();
            return lootPrefs.CreateEntry<SerialisiableLootTable>(
            _table.RaiderType,
            _table,
            _table.RaiderType,
            false);
        }

        private void UpdateTable(LootTable table)
        {
            if (lootRoller.LootTables.Keys.Contains(table.RaiderType))
            {
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Msg($"overwriting loot table for '{table.RaiderType}' ...");
                }
            }
            lootRoller.UpdateLootTable(table);
        }

        private void UpdateTablesFromPrefs()
        {
            // extract table from each entry
            foreach (var entry in lootPrefs.Entries)
            {
                var table = ((SerialisiableLootTable)entry.BoxedValue).Deserialise();
                UpdateTable(table);
            }
        }

        #endregion Private Methods
    }
}