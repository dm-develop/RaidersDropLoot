namespace dm.ffmods.raidersdroploot
{
    public class ConfigTableRow
    {
        #region Fields

        public int AmountInBundle = 1;
        public int DropRateInPercent = 0;
        public string ID = "undefined";

        #endregion Fields

        #region Public Methods

        public static ConfigTableRow CreateFromTableEntry(TableEntry entry)
        {
            var row = new ConfigTableRow();
            row.DropRateInPercent = (int)entry.DropRateInPercent;
            row.ID = entry.ItemID;
            row.AmountInBundle = (int)entry.AmountInBundle;
            return row;
        }

        #endregion Public Methods
    }

    public class SerialisiableLootTable
    {
        #region Fields

        public string RaiderType = "unknown";
        public List<ConfigTableRow> Table = new List<ConfigTableRow>();

        #endregion Fields

        #region Public Methods

        public static SerialisiableLootTable CreateFromLootTable(LootTable table)
        {
            var serialisiableLootTable = new SerialisiableLootTable();

            serialisiableLootTable.RaiderType = table.RaiderType.ToString();

            var configTable = new List<ConfigTableRow>();

            if (table.Drops.Any())
            {
                foreach (var item in table.Drops)
                {
                    TableEntry entry = item.Value;
                    configTable.Add(ConfigTableRow.CreateFromTableEntry(entry));
                }
            }
            serialisiableLootTable.Table = configTable;
            return serialisiableLootTable;
        }

        public LootTable Deserialise()
        {
            var type = Enum.Parse<RaiderType>(RaiderType);

            var drops = new Dictionary<LootItem, TableEntry>();

            if (Table.Any())
            {
                foreach (var row in Table)
                {
                    var entry = new TableEntry(row.ID, (uint)row.DropRateInPercent, (uint)row.AmountInBundle);
                    drops.Add(Enum.Parse<LootItem>(row.ID), entry);
                }
            }

            return new LootTable(type, drops);
        }

        public override string ToString()
        {
            string str = "";
            str += ($"'{this.RaiderType}': ");

            if (!Table.Any())
            {
                str += "[]";
                return str;
            }

            str += "[";
            foreach (var row in this.Table)
            {
                str += $"({row.ID}: {row.DropRateInPercent}, {row.AmountInBundle})";
            }
            str += "]";
            return str;
        }

        #endregion Public Methods
    }
}