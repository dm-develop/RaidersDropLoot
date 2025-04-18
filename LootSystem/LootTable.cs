using System.Collections.Generic;

namespace dm.ffmods.raidersdroploot
{
    public struct LootTable
    {
        #region Public Constructors

        public LootTable(RaiderType raiderType, Dictionary<LootItem, TableEntry> drops)
        {
            RaiderType = raiderType;
            Drops = drops;
        }

        #endregion Public Constructors

        #region Properties

        public Dictionary<LootItem, TableEntry> Drops { get; set; }
        public RaiderType RaiderType { get; private set; }

        #endregion Properties

        #region Public Methods

        public override string ToString()
        {
            string str = "";
            str += ($"'{this.RaiderType}': ");
            if (!Drops.Any())
            {
                str += "[]";
                return str;
            }
            str += "[";
            foreach (var item in this.Drops)
            {
                TableEntry entry = item.Value;
                str += $"('{item.Key}': '{entry.ItemID}', {entry.BaseDropChanceInPercent}, {entry.AmountInBundle})";
            }
            str += "]";
            return str;
        }

        #endregion Public Methods
    }
}