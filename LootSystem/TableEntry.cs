namespace dm.ffmods.raidersdroploot
{
    public struct TableEntry
    {
        #region Fields

        private readonly string itemID;
        private uint amountInBundle;

        private uint baseDropChanceInPercent;

        private bool isDynamic;

        #endregion Fields

        #region Public Constructors

        public TableEntry(string itemID, uint dropRateInPercent, uint amountInBundle, bool isDynamic)
        {
            this.itemID = itemID;
            this.amountInBundle = amountInBundle;
            this.baseDropChanceInPercent = dropRateInPercent;
            this.isDynamic = isDynamic;
            SetDropRate(dropRateInPercent);
            SetAmount(amountInBundle);
            SetDynamic(isDynamic);
        }

        #endregion Public Constructors

        #region Properties

        public uint AmountInBundle { get => amountInBundle; }
        public uint BaseDropChanceInPercent { get => baseDropChanceInPercent; }
        public bool IsDynamic { get => isDynamic; }
        public string ItemID { get => itemID; }

        #endregion Properties

        #region Public Methods

        public void SetAmount(uint amount)
        {
            if (amount < 1)
            {
                amount = 1;
            }
            amountInBundle = amount;
        }

        public void SetDropRate(uint rateInPercent)
        {
            if (rateInPercent > 100)
            {
                rateInPercent = 100;
            }
            baseDropChanceInPercent = rateInPercent;
        }

        public void SetDynamic(bool newState)
        {
            isDynamic = newState;
        }

        #endregion Public Methods
    }
}