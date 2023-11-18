using Il2Cpp;
using MelonLoader;
using Random = UnityEngine.Random;

namespace dm.ffmods.raidersdroploot
{
    public enum LootItem
    {
        crudeWeapon,
        weapon,
        heavyWeapon,
        leatherCoat,
        hauberk,
        plateMail,
        shield,
        bow,
        crossbow,
        arrows,
        smokedMeat,
        bread,
        gold,
        shoes,
        linenClothes,
    }

    public enum RaiderType
    {
        Brawler,
        Thief,
        Warrior,
        Elite,
        Champion
    }

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

        private uint currentRaidSize = 0;

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
            if (!LootTables[type].Drops.Any())
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

            if (table.Drops.Any())
            {
                foreach (var item in table.Drops)
                {
                    int roll = Random.Range(1, 100 + 1);

                    if (roll <= item.Value.DropRateInPercent)
                    {
                        toSpawn.Add(item.Key);
                        if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                        {
                            Melon<RaidersDropLootMelon>.Logger.Msg($"rolled {roll} for item '{item.Key}' " +
                                $"with droprate {item.Value.DropRateInPercent}%, adding to loot bucket ...");
                        }
                    }
                }
            }
            return toSpawn;
        }

        public void UpdateLootTable(LootTable table)
        {
            var type = table.RaiderType;
            if (!table.Drops.Any())
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

        public void UpdateRaidSize(uint newRaidSize)
        {
            if (Melon<RaidersDropLootMelon>.Instance.Verbose)
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($" setting new raid size of {newRaidSize}");
            }

            currentRaidSize = newRaidSize;
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
                { LootItem.linenClothes, new TableEntry(LootItem.linenClothes.ToString(), 20, 1 )},
                { LootItem.shoes, new TableEntry(LootItem.shoes.ToString(), 60, 1 )},
            };
            Dictionary<LootItem, TableEntry> thiefLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.arrows, new TableEntry(LootItem.arrows.ToString(), 60, 10 )},
                { LootItem.bow, new TableEntry(LootItem.bow.ToString(), 60, 1 )},
                { LootItem.gold, new TableEntry(LootItem.gold.ToString(), 50, 10 )},
                { LootItem.leatherCoat, new TableEntry(LootItem.leatherCoat.ToString(), 40, 1 )},
                { LootItem.linenClothes, new TableEntry(LootItem.linenClothes.ToString(), 20, 1 )},
                { LootItem.shoes, new TableEntry(LootItem.shoes.ToString(), 60, 1 )},
            };
            Dictionary<LootItem, TableEntry> warriorLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.shoes, new TableEntry(LootItem.shoes.ToString(), 60, 1 )},
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 40, 1 )},
                { LootItem.weapon, new TableEntry(LootItem.weapon.ToString(), 60, 1 )},
                { LootItem.linenClothes, new TableEntry(LootItem.linenClothes.ToString(), 20, 1 )},
                { LootItem.hauberk, new TableEntry(LootItem.hauberk.ToString(), 20, 1 )},
            };
            Dictionary<LootItem, TableEntry> eliteLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.shoes, new TableEntry(LootItem.shoes.ToString(), 60, 1 )},
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 20, 1 )},
                { LootItem.weapon, new TableEntry(LootItem.weapon.ToString(), 60, 1 )},
                { LootItem.linenClothes, new TableEntry(LootItem.linenClothes.ToString(), 20, 1 )},
                { LootItem.hauberk, new TableEntry(LootItem.hauberk.ToString(), 20, 1 )},
                { LootItem.heavyWeapon, new TableEntry(LootItem.heavyWeapon.ToString(), 10, 1 )},
                { LootItem.gold, new TableEntry(LootItem.gold.ToString(), 50, 10 )},
                { LootItem.crossbow, new TableEntry(LootItem.crossbow.ToString(), 30, 1 )},
                { LootItem.arrows, new TableEntry(LootItem.arrows.ToString(), 30, 10 )},
            };
            Dictionary<LootItem, TableEntry> championLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.shoes, new TableEntry(LootItem.shoes.ToString(), 60, 1 )},
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 20, 1 )},
                { LootItem.weapon, new TableEntry(LootItem.weapon.ToString(), 60, 1 )},
                { LootItem.linenClothes, new TableEntry(LootItem.linenClothes.ToString(), 10, 1 )},
                { LootItem.plateMail, new TableEntry(LootItem.plateMail.ToString(), 20, 1 )},
                { LootItem.heavyWeapon, new TableEntry(LootItem.heavyWeapon.ToString(), 20, 1 )},
                { LootItem.gold, new TableEntry(LootItem.gold.ToString(), 50, 10 )},
                { LootItem.crossbow, new TableEntry(LootItem.crossbow.ToString(), 30, 1 )},
                { LootItem.arrows, new TableEntry(LootItem.arrows.ToString(), 30, 10 )},
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