namespace dm.ffmods.raidersdroploot
{
    public struct TableEntry
    {
        #region Fields

        private readonly string itemID = "unknown";
        private uint amountInBundle = 1;

        private uint dropRateInPercent = 0;

        #endregion Fields

        #region Public Constructors

        public TableEntry(string itemID, uint dropRateInPercent, uint amountInBundle)
        {
            this.itemID = itemID;
            SetDropRate(dropRateInPercent);
            SetAmount(amountInBundle);
        }

        #endregion Public Constructors

        #region Properties

        public uint AmountInBundle { get => amountInBundle; }
        public uint DropRateInPercent { get => dropRateInPercent; }
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
            dropRateInPercent = rateInPercent;
        }

        #endregion Public Methods
    }
}