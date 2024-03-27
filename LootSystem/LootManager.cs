using Il2Cpp;
using MelonLoader;
using Random = UnityEngine.Random;

namespace dm.ffmods.raidersdroploot
{
    public class LootManager
    {
        #region Fields

        private static Dictionary<string, RaiderType> unitNames = new Dictionary<string, RaiderType>
        {
            { "RaiderUnit_Archer", RaiderType.Archer },
            { "RaiderUnit_Brawler", RaiderType.Brawler },
            { "RaiderUnit_Thief", RaiderType.Thief },
            { "RaiderUnit_Warrior", RaiderType.Warrior },
            { "RaiderUnit_Elite", RaiderType.Elite },
            { "RaiderUnit_Champion", RaiderType.Champion},
            { "RaiderUnit_Horseman", RaiderType.Horseman},
            { "RaiderUnit_RaidCamp_Archer", RaiderType.Archer },
            { "RaiderUnit_RaidCamp_Arbalest", RaiderType.Arbalest },
            { "RaiderUnit_RaidCamp_Brawler", RaiderType.Brawler },
            { "RaiderUnit_RaidCamp_Warrior", RaiderType.Warrior },
            { "RaiderUnit_RaidCamp_Elite", RaiderType.Elite },
            { "RaiderUnit_RaidCamp_Champion", RaiderType.Champion},
            { "RaiderUnit_RaidCamp_Horseman", RaiderType.Horseman},
            { "RaiderUnit_RelicBrawler", RaiderType.Brawler },
            { "RaiderUnit_RelicWarrior", RaiderType.Warrior },
        };

        private DynamicLootScaler scaler;

        #endregion Fields

        #region Public Constructors

        public LootManager(DynamicLootScaler scaler)
        {
            this.scaler = scaler;
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

                    uint droprate = item.Value.BaseDropChanceInPercent;
                    if (item.Value.IsDynamic)
                    {
                        float multiplier = scaler.LootMultipliers[item.Key];
                        droprate = (uint)((float)droprate * multiplier);
                        if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                        {
                            Melon<RaidersDropLootMelon>.Logger.Msg($"using adjusted droprate for '{item.Key}', current droprate is {droprate}%");
                        }
                    }

                    if (roll <= droprate)
                    {
                        toSpawn.Add(item.Key);
                        if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                        {
                            Melon<RaidersDropLootMelon>.Logger.Msg($"rolled {roll} for item '{item.Key}' " +
                                $"with droprate {droprate}%, adding to loot bucket of '{type}' ...");
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

        #endregion Public Methods

        #region Private Methods

        private void CreateDefaultLootTables()
        {
            if (Melon<RaidersDropLootMelon>.Instance.UseFullTableAsDefault)
            {
                foreach (RaiderType raider in Enum.GetValues(typeof(RaiderType)))
                {
                    LootTables.Add(raider, new LootTable(raider, DefaultLootTables.GetFullTable()));
                    Melon<RaidersDropLootMelon>.Logger.Warning($" added FULL loot table for {raider}");
                }
                return;
            }
            LootTables.Add(RaiderType.Archer, new LootTable(RaiderType.Archer, DefaultLootTables.DefaulArcherLoot));
            LootTables.Add(RaiderType.Arbalest, new LootTable(RaiderType.Arbalest, DefaultLootTables.DefaulArbalestLoot));
            LootTables.Add(RaiderType.Brawler, new LootTable(RaiderType.Brawler, DefaultLootTables.DefaultBrawlerLoot));
            LootTables.Add(RaiderType.Thief, new LootTable(RaiderType.Thief, DefaultLootTables.DefaultThiefLoot));
            LootTables.Add(RaiderType.Warrior, new LootTable(RaiderType.Warrior, DefaultLootTables.DefaultWarriorLoot));
            LootTables.Add(RaiderType.Horseman, new LootTable(RaiderType.Horseman, DefaultLootTables.DefaultHorsemanLoot));
            LootTables.Add(RaiderType.Elite, new LootTable(RaiderType.Elite, DefaultLootTables.DefaultEliteLoot));
            LootTables.Add(RaiderType.Champion, new LootTable(RaiderType.Champion, DefaultLootTables.DefaultChampionLoot));
        }

        #endregion Private Methods
    }
}