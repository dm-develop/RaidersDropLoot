﻿using Il2Cpp;
using MelonLoader;
using Random = UnityEngine.Random;

namespace dm.ffmods.raidersdroploot
{
    public enum RaiderType
    { Brawler, Thief, Warrior, Elite, Champion }

    public class LootManager
    {
        #region Fields

        private static Dictionary<string, RaiderType> unitNames = new Dictionary<string, RaiderType>
        {
            { "RaiderUnit_Brawler", RaiderType.Brawler },
            { "RaiderUnit_Thief", RaiderType.Thief },
            { "RaiderUnit_Warrior", RaiderType.Warrior },
            { "RaiderUnit_Elite", RaiderType.Elite },
            { "RaiderUnit_Champion", RaiderType.Champion}
        };

        #endregion Fields

        #region Public Constructors

        public LootManager()
        {
            LootTables = new Dictionary<RaiderType, LootTable>();
            CreateDefaultLootTables();
        }

        #endregion Public Constructors

        #region Properties

        public Dictionary<RaiderType, LootTable> LootTables { get; private set; }

        #endregion Properties

        #region Public Methods

        public static RaiderType DetermineRaiderTypeFromUnitName(string unitName)
        {
            foreach (string key in unitNames.Keys.OrderByDescending(s => s.Length))
            {
                if (unitName.StartsWith(key))
                {
                    return unitNames[key];
                }
            }
            // if we can't find the name, return default
            if (Melon<RaidersDropLootMelon>.Instance.Verbose)
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"Raider type '{unitName}' unknown, treating as '{RaiderType.Brawler}'.");
            }
            return RaiderType.Brawler;
        }

        public bool IsLootable(RaiderType type)
        {
            if (!LootTables.ContainsKey(type))
            {
                return false;
            }
            if (!LootTables[type].DropTable.Any())
            {
                return false;
            }
            return true;
        }

        public List<LootItem> RollLoot(RaiderType type)
        {
            List<LootItem> toSpawn = new List<LootItem>();
            if (!IsLootable(type))
            {
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Warning($"'{type}' is not lootable, cannot roll loot!");
                }
                return toSpawn;
            }

            LootTable table = LootTables[type];

            if (table.DropTable.Any())
            {
                foreach (var item in table.DropTable)
                {
                    int roll = Random.Range(1, 100 + 1);

                    if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                    {
                        Melon<RaidersDropLootMelon>.Logger.Warning($"rolled {roll} for item '{item.Key}' " +
                            $"with droprate {item.Value.DropRateInPercent}%");
                    }
                    if (roll <= item.Value.DropRateInPercent)
                    {
                        toSpawn.Add(item.Key);
                    }
                }
            }
            return toSpawn;
        }

        public void UpdateLootTable(LootTable table)
        {
            var type = table.RaiderType;
            if (!table.DropTable.Any())
            {
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Warning($"Received empty loot table for '{type}'," +
                            $" removing entry from loot table list.");
                }
                LootTables.Remove(type);
                return;
            }
            if (!LootTables.Keys.Contains(type))
            {
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Warning($"Raider type '{type}' unknown," +
                        $" creating new loot entry.");
                }

                LootTables.Add(type, table);
                return;
            }
            Melon<RaidersDropLootMelon>.Logger.Msg($"updating loot entry for '{type}' ...");
            LootTables[type] = table;
        }

        #endregion Public Methods

        #region Private Methods

        private void CreateDefaultLootTables()
        {
            Dictionary<LootItem, TableEntry> brawlerLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.crudeWeapon, new TableEntry(LootItem.crudeWeapon.ToString(), 70, 1 )},
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 5, 1 )},
                { LootItem.smokedMeat, new TableEntry(LootItem.smokedMeat.ToString(), 5, 1 )},
                { LootItem.leatherCoat, new TableEntry(LootItem.leatherCoat.ToString(), 40, 1 )},
            };
            Dictionary<LootItem, TableEntry> thiefLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.arrows, new TableEntry(LootItem.arrows.ToString(), 50, 10 )},
                { LootItem.bow, new TableEntry(LootItem.bow.ToString(), 50, 1 )},
                { LootItem.gold, new TableEntry(LootItem.gold.ToString(), 100, 20 )},
            };
            Dictionary<LootItem, TableEntry> warriorLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 5, 1 )},
            };
            Dictionary<LootItem, TableEntry> eliteLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 5, 1 )},
            };
            Dictionary<LootItem, TableEntry> championLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 5, 1 )},
            };

            LootTables.Add(RaiderType.Brawler, new LootTable(RaiderType.Brawler, brawlerLoot));
            LootTables.Add(RaiderType.Thief, new LootTable(RaiderType.Thief, thiefLoot));
            LootTables.Add(RaiderType.Warrior, new LootTable(RaiderType.Warrior, warriorLoot));
            LootTables.Add(RaiderType.Elite, new LootTable(RaiderType.Elite, eliteLoot));
            LootTables.Add(RaiderType.Champion, new LootTable(RaiderType.Champion, championLoot));
        }

        #endregion Private Methods
    }
}