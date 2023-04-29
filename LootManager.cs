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
            return LootTables.ContainsKey(type);
        }

        public List<LootItem> RollLoot(RaiderType type)
        {
            if (!IsLootable(type))
            {
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Warning($"'{type}' is not lootable, cannot roll loot!");
                }
                return new List<LootItem>();
            }

            LootTable table = LootTables[type];
            List<LootItem> toSpawn = new List<LootItem>();
            foreach (var item in table.Drops)
            {
                int roll = Random.Range(0, 100 + 1);
                if (roll <= item.Value)
                {
                    toSpawn.Add(item.Key);
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

        #endregion Public Methods

        #region Private Methods

        private void CreateDefaultLootTables()
        {
            Dictionary<LootItem, byte> brawlerLoot = new Dictionary<LootItem, byte>()
            {
                { LootItem.crudeWeapon, 50 },
                { LootItem.weapon, 5 },
                { LootItem.heavyWeapon, 0 },
                { LootItem.shield, 5 },
                { LootItem.leatherCoat, 30 },
                { LootItem.hauberk, 0 },
                { LootItem.plateMail, 0 },
                { LootItem.bow, 0 },
                { LootItem.crossbow, 0 },
                { LootItem.arrows, 0 },
                { LootItem.bread, 5},
                { LootItem.smokedMeat, 5},
                { LootItem.gold, 0}
            };
            Dictionary<LootItem, byte> thiefLoot = new Dictionary<LootItem, byte>()
            {
                { LootItem.crudeWeapon, 0 },
                { LootItem.weapon, 5 },
                { LootItem.heavyWeapon, 0 },
                { LootItem.shield, 0 },
                { LootItem.leatherCoat, 30 },
                { LootItem.hauberk, 0 },
                { LootItem.plateMail, 0 },
                { LootItem.bow, 40 },
                { LootItem.crossbow, 0 },
                { LootItem.arrows, 60 },
                { LootItem.bread, 5},
                { LootItem.smokedMeat, 5},
                { LootItem.gold, 50}
            };
            Dictionary<LootItem, byte> warriorLoot = new Dictionary<LootItem, byte>()
            {
                { LootItem.crudeWeapon, 0 },
                { LootItem.weapon, 60 },
                { LootItem.heavyWeapon, 0 },
                { LootItem.shield, 20 },
                { LootItem.leatherCoat, 30 },
                { LootItem.hauberk, 30 },
                { LootItem.plateMail, 0 },
                { LootItem.bow, 0 },
                { LootItem.crossbow, 10 },
                { LootItem.arrows, 10 },
                { LootItem.bread, 5},
                { LootItem.smokedMeat, 5},
                { LootItem.gold, 5}
            };
            Dictionary<LootItem, byte> eliteLoot = new Dictionary<LootItem, byte>()
            {
                { LootItem.crudeWeapon, 0 },
                { LootItem.weapon, 30 },
                { LootItem.heavyWeapon, 10 },
                { LootItem.shield, 20 },
                { LootItem.leatherCoat, 0 },
                { LootItem.hauberk, 10 },
                { LootItem.plateMail, 10 },
                { LootItem.bow, 0 },
                { LootItem.crossbow, 10 },
                { LootItem.arrows, 10 },
                { LootItem.bread, 5},
                { LootItem.smokedMeat, 5},
                { LootItem.gold, 10}
            };
            Dictionary<LootItem, byte> championLoot = new Dictionary<LootItem, byte>()
            {
                { LootItem.crudeWeapon, 0 },
                { LootItem.weapon, 30 },
                { LootItem.heavyWeapon, 20 },
                { LootItem.shield, 20 },
                { LootItem.leatherCoat, 0 },
                { LootItem.hauberk, 10 },
                { LootItem.plateMail, 20 },
                { LootItem.bow, 0 },
                { LootItem.crossbow, 10 },
                { LootItem.arrows, 10 },
                { LootItem.bread, 5},
                { LootItem.smokedMeat, 5},
                { LootItem.gold, 20}
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