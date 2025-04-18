using MelonLoader;
using System.Collections.Generic;

namespace dm.ffmods.raidersdroploot
{
    public class ConfigManager
    {
        #region Fields

        private bool isInitialised = false;
        private LootManager lootManager;
        private MelonPreferences_Category lootPrefs;
        private LootSettingsManager lootSettingsManager;

        #endregion Fields

        #region Properties

        public bool IsInitialised { get => isInitialised; }

        #endregion Properties

        #region Public Methods

        public void InitConfig(LootManager lootManager, LootSettingsManager settingsManager)
        {
            this.lootSettingsManager = settingsManager;
            this.lootManager = lootManager;
            this.lootPrefs = LootSettingsManager.LootPrefs;

            CreateEntries();

            UpdateTablesFromPrefs();

            isInitialised = true;
        }

        #endregion Public Methods

        #region Private Methods

        private void CreateEntries()
        {
            foreach (var item in lootManager.LootTables)
            {
                _ = CreateMelonPref(item.Value);
            }
        }

        private MelonPreferences_Entry CreateMelonPref(LootTable table)
        {
            var _table = SerialisiableLootTable.CreateFromLootTable(table);
            return lootPrefs.CreateEntry<SerialisiableLootTable>(
            _table.RaiderType,
            _table,
            _table.RaiderType,
            false);
        }

        private void UpdateTable(LootTable table)
        {
            if (lootManager.LootTables.ContainsKey(table.RaiderType))
            {
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Msg($"overwriting loot table for '{table.RaiderType}' ...");
                }
            }
            lootManager.UpdateLootTable(table);
        }

        private void UpdateTablesFromPrefs()
        {
            var toIgnore = lootSettingsManager.PrefEntriesToIgnore;
            var entries = new List<MelonPreferences_Entry>();

            // Manually filter out entries that are in the toIgnore list
            foreach (var entry in lootPrefs.Entries)
            {
                bool shouldIgnore = false;
                foreach (var ignoreEntry in toIgnore)
                {
                    if (entry == ignoreEntry)
                    {
                        shouldIgnore = true;
                        break;
                    }
                }
                if (!shouldIgnore)
                {
                    entries.Add(entry);
                }
            }

            // Extract table from each entry
            foreach (var entry in entries)
            {
                var table = ((SerialisiableLootTable)entry.BoxedValue).Deserialise();
                UpdateTable(table);
            }
        }

        #endregion Private Methods
    }
}