using MelonLoader;

namespace dm.ffmods.raidersdroploot
{
    public class ConfigManager
    {
        #region Fields

        private bool isInitialised = false;
        private MelonPreferences_Category lootPrefs;
        private LootManager lootRoller;

        #endregion Fields

        #region Properties

        public bool IsInitialised { get => isInitialised; }

        #endregion Properties

        #region Public Methods

        public void InitConfig(LootManager lootRoller, MelonPreferences_Category lootPrefs)
        {
            this.lootRoller = lootRoller;
            this.lootPrefs = lootPrefs;

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
            var toIgnore = Melon<RaidersDropLootMelon>.Instance.GetPrefEntriesToIgnore();
            var entries = lootPrefs.Entries.Except(toIgnore);
            // extract table from each entry
            foreach (var entry in entries)
            {
                var table = ((SerialisiableLootTable)entry.BoxedValue).Deserialise();
                UpdateTable(table);
            }
        }

        #endregion Private Methods
    }
}