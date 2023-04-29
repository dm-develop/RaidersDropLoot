using MelonLoader;
using Random = UnityEngine.Random;

namespace dm.ffmods.raidersdroploot
{
    public enum RaiderType
    { Brawler, Thief, Warrior, Elite, Champion }

    public class LootRoller
    {
        #region Fields

        public static Dictionary<string, RaiderType> UnitNames = new Dictionary<string, RaiderType>
        {
            { "RaiderUnit_Brawler", RaiderType.Brawler },
            { "RaiderUnit_Thief", RaiderType.Thief },
            { "RaiderUnit_Warrior", RaiderType.Warrior },
            { "RaiderUnit_Elite", RaiderType.Elite },
            { "RaiderUnit_Champion", RaiderType.Champion}
        };

        #endregion Fields

        #region Public Constructors

        public LootRoller()
        {
            LootTables = new Dictionary<RaiderType, LootTable>();
            CreateDefaultLootTables();
        }

        #endregion Public Constructors

        #region Properties

        public Dictionary<RaiderType, LootTable> LootTables { get; private set; }

        #endregion Properties

        #region Public Methods

        public static RaiderType DetermineRaiderType(string unitName)
        {
            foreach (string key in UnitNames.Keys.OrderByDescending(s => s.Length))
            {
                if (unitName.StartsWith(key))
                {
                    return UnitNames[key];
                }
            }
            // if we can't find the name, return default
            Melon<RaidersDropLootMelon>.Logger.Warning($"Raider type {unitName} unknown, treating as '{RaiderType.Brawler}'.");
            return RaiderType.Brawler;
        }

        public List<LootItem> RollLoot(RaiderType type)
        {
            if (!LootTables.ContainsKey(type))
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"No LootTable for {type} found, using table for '{RaiderType.Brawler}'.");
                type = RaiderType.Brawler;
            }

            LootTable table = LootTables[type];
            List<LootItem> toSpawn = new List<LootItem>();
            foreach (var item in table.DropRates)
            {
                int roll = Random.Range(0, 100 + 1);
                if (roll <= item.Value)
                {
                    toSpawn.Add(item.Key);
                }
            }
            return toSpawn;
        }

        #endregion Public Methods

        #region Private Methods

        private void CreateDefaultLootTables()
        {
            Dictionary<LootItem, byte> brawlerLoot = new Dictionary<LootItem, byte>()
            {
                { LootItem.crudeWeapon, 40 },
                { LootItem.leatherCoat, 30 },
                { LootItem.bread, 5},
                { LootItem.smokedMeat, 5},
            };
            Dictionary<LootItem, byte> thiefLoot = new Dictionary<LootItem, byte>()
            {
                { LootItem.bow, 30 },
                { LootItem.arrows, 40 },
                { LootItem.leatherCoat, 30 },
                { LootItem.bread, 5},
                { LootItem.smokedMeat, 5},
                { LootItem.gold, 10}
            };
            Dictionary<LootItem, byte> warriorLoot = new Dictionary<LootItem, byte>()
            {
                { LootItem.weapon, 30 },
                { LootItem.shield, 30 },
                { LootItem.leatherCoat, 30 },
                { LootItem.bread, 5},
                { LootItem.smokedMeat, 5},
            };
            Dictionary<LootItem, byte> eliteLoot = new Dictionary<LootItem, byte>()
            {
                { LootItem.weapon, 30 },
                { LootItem.shield, 30 },
                { LootItem.hauberk, 20 },
                { LootItem.crossbow, 20 },
                { LootItem.arrows, 5 },
                { LootItem.bread, 5},
                { LootItem.smokedMeat, 5},
                { LootItem.gold, 5}
            };
            Dictionary<LootItem, byte> championLoot = new Dictionary<LootItem, byte>()
            {
                { LootItem.heavyWeapon, 20 },
                { LootItem.plateMail, 10 },
                { LootItem.crossbow, 20 },
                { LootItem.arrows, 10 },
                { LootItem.bread, 5},
                { LootItem.smokedMeat, 5},
                { LootItem.gold, 5}
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