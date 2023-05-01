namespace dm.ffmods.raidersdroploot
{
    public struct LootTable
    {
        #region Public Constructors

        public LootTable(RaiderType raiderType, Dictionary<LootItem, TableEntry> drops)
        {
            RaiderType = raiderType;
            DropTable = drops;
        }

        #endregion Public Constructors

        #region Properties

        public Dictionary<LootItem, TableEntry> DropTable { get; set; }
        public RaiderType RaiderType { get; private set; }

        #endregion Properties

        #region Public Methods

        public override string ToString()
        {
            string str = "";
            str += ($"'{this.RaiderType}': ");
            if (!DropTable.Any())
            {
                str += "[]";
                return str;
            }
            str += "[";
            foreach (var item in this.DropTable)
            {
                TableEntry entry = item.Value;
                str += $"('{item.Key}': '{entry.ItemID}', {entry.DropRateInPercent}, {entry.AmountInBundle})";
            }
            str += "]";
            return str;
        }

        #endregion Public Methods
    }
}