namespace dm.ffmods.raidersdroploot
{
    public static class DefaultLootTables
    {
        #region Fields

        public static Dictionary<LootItem, TableEntry> DefaultBrawlerLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 5, 1, true)},
                { LootItem.smokedMeat, new TableEntry(LootItem.smokedMeat.ToString(), 5, 1, true)},
                { LootItem.hideCoat, new TableEntry(LootItem.hideCoat.ToString(), 40, 1, true)},
                { LootItem.linenClothes, new TableEntry(LootItem.linenClothes.ToString(), 20, 1, true)},
                { LootItem.shoes, new TableEntry(LootItem.shoes.ToString(), 60, 1, true)},
            };

        public static Dictionary<LootItem, TableEntry> DefaultChampionLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.shoes, new TableEntry(LootItem.shoes.ToString(), 60, 1 ,true)},
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 20, 1, true)},
                { LootItem.weapon, new TableEntry(LootItem.weapon.ToString(), 60, 1, true)},
                { LootItem.linenClothes, new TableEntry(LootItem.linenClothes.ToString(), 10, 1, true)},
                { LootItem.platemail, new TableEntry(LootItem.platemail.ToString(), 20, 1, true)},
                { LootItem.heavyWeapon, new TableEntry(LootItem.heavyWeapon.ToString(), 20, 1, true)},
                { LootItem.goldIngot, new TableEntry(LootItem.goldIngot.ToString(), 50, 10, true)},
                { LootItem.crossbow, new TableEntry(LootItem.crossbow.ToString(), 30, 1, true)},
                { LootItem.arrow, new TableEntry(LootItem.arrow.ToString(), 30, 10, true)},
            };

        public static Dictionary<LootItem, TableEntry> DefaultEliteLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.shoes, new TableEntry(LootItem.shoes.ToString(), 60, 1, true)},
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 20, 1, true)},
                { LootItem.weapon, new TableEntry(LootItem.weapon.ToString(), 60, 1, true)},
                { LootItem.linenClothes, new TableEntry(LootItem.linenClothes.ToString(), 20, 1, true)},
                { LootItem.hauberk, new TableEntry(LootItem.hauberk.ToString(), 20, 1, true)},
                { LootItem.heavyWeapon, new TableEntry(LootItem.heavyWeapon.ToString(), 10, 1, true )},
                { LootItem.goldIngot , new TableEntry(LootItem.goldIngot.ToString(), 50, 10, true)},
                { LootItem.crossbow, new TableEntry(LootItem.crossbow.ToString(), 30, 1, true )},
                { LootItem.arrow, new TableEntry(LootItem.arrow.ToString(), 30, 10, true)},
            };

        public static Dictionary<LootItem, TableEntry> DefaultThiefLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.arrow, new TableEntry(LootItem.arrow.ToString(), 60, 10, true )},
                { LootItem.bow, new TableEntry(LootItem.bow.ToString(), 60, 1, true )},
                { LootItem.goldIngot , new TableEntry(LootItem.goldIngot.ToString(), 50, 10, true)},
                { LootItem.hideCoat, new TableEntry(LootItem.hideCoat.ToString(), 40, 1, true )},
                { LootItem.linenClothes, new TableEntry(LootItem.linenClothes.ToString(), 20, 1, true )},
                { LootItem.shoes, new TableEntry(LootItem.shoes.ToString(), 60, 1, true )},
            };

        public static Dictionary<LootItem, TableEntry> DefaultWarriorLoot = new Dictionary<LootItem, TableEntry>()
            {
                { LootItem.shoes, new TableEntry(LootItem.shoes.ToString(), 60, 1, true )},
                { LootItem.shield, new TableEntry(LootItem.shield.ToString(), 40, 1, true )},
                { LootItem.weapon, new TableEntry(LootItem.weapon.ToString(), 60, 1, true )},
                { LootItem.linenClothes, new TableEntry(LootItem.linenClothes.ToString(), 20, 1, true )},
                { LootItem.hauberk, new TableEntry(LootItem.hauberk.ToString(), 20, 1, true )},
            };

        #endregion Fields

        #region Public Methods

        public static Dictionary<LootItem, TableEntry> GetFullTable()
        {
            var fullTable = new Dictionary<LootItem, TableEntry>();

            foreach (LootItem item in Enum.GetValues(typeof(LootItem)))
            {
                fullTable.Add(item, new TableEntry(item.ToString(), 100, 1, true));
            }
            return fullTable;
        }

        #endregion Public Methods
    }
}