namespace dm.ffmods.raidersdroploot
{
    public struct LootTable
    {
        #region Public Constructors

        public LootTable(RaiderType raiderType, Dictionary<LootItem, byte> dropRates)
        {
            RaiderType = raiderType;
            DropRates = dropRates;
            ScaleDroprates();
        }

        #endregion Public Constructors

        #region Properties

        public Dictionary<LootItem, byte> DropRates { get; private set; }
        public RaiderType RaiderType { get; private set; }

        #endregion Properties

        #region Private Methods

        private void ScaleDroprates()
        {
            foreach (var item in DropRates)
            {
                if (item.Value > 100)
                {
                    DropRates[item.Key] = 100;
                }
            }
        }

        #endregion Private Methods
    }
}