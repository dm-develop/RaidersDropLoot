namespace dm.ffmods.raidersdroploot
{
    public struct LootTable
    {
        #region Public Constructors

        public LootTable(RaiderType raiderType, Dictionary<LootItem, byte> drops)
        {
            RaiderType = raiderType;
            Drops = drops;
            ScaleDroprates();
        }

        #endregion Public Constructors

        #region Properties

        public Dictionary<LootItem, byte> Drops { get; private set; }
        public RaiderType RaiderType { get; private set; }

        #endregion Properties

        #region Public Methods

        public SerialisiableLootTable MakeSerialisable()
        {
            var serialisiableLootTable = new SerialisiableLootTable();
            serialisiableLootTable.RaiderType = RaiderType.ToString();

            var drops = new Dictionary<string, int>();
            foreach (var item in Drops)
            {
                drops.Add(item.Key.ToString(), item.Value);
            }
            serialisiableLootTable.Drops = drops;
            return serialisiableLootTable;
        }

        #endregion Public Methods

        #region Private Methods

        private void ScaleDroprates()
        {
            foreach (var item in Drops)
            {
                if (item.Value > 100)
                {
                    Drops[item.Key] = 100;
                }
            }
        }

        #endregion Private Methods
    }

    public class SerialisiableLootTable
    {
        #region Fields

        public Dictionary<string, int> Drops;
        public string RaiderType;

        #endregion Fields

        #region Public Methods

        public LootTable Deserialise()
        {
            var drops = new Dictionary<LootItem, byte>();
            foreach (var item in Drops)
            {
                var newKey = Enum.Parse<LootItem>(item.Key);
                drops.Add(newKey, (byte)item.Value);
            }
            var type = Enum.Parse<RaiderType>(RaiderType);
            return new LootTable(type, drops);
        }

        #endregion Public Methods
    }
}