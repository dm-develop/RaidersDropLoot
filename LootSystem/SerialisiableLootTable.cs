using System.Collections.Generic;

namespace dm.ffmods.raidersdroploot
{
    public class ConfigTableRow
    {
        #region Fields

        public int AmountInBundle = 1;
        public int BaseDropChanceInPercent = 0;
        public string ID = "undefined";
        public bool IsDynamic = false;

        #endregion Fields

        #region Public Methods

        public static ConfigTableRow CreateFromTableEntry(TableEntry entry)
        {
            var row = new ConfigTableRow();
            row.BaseDropChanceInPercent = (int)entry.BaseDropChanceInPercent;
            row.ID = entry.ItemID;
            row.AmountInBundle = (int)entry.AmountInBundle;
            row.IsDynamic = (bool)entry.IsDynamic;
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
                    var entry = new TableEntry(row.ID, (uint)row.BaseDropChanceInPercent, (uint)row.AmountInBundle, (bool)row.IsDynamic);
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
                str += $"({row.ID}: {row.BaseDropChanceInPercent}, {row.AmountInBundle}, {row.IsDynamic})";
            }
            str += "]";
            return str;
        }

        #endregion Public Methods
    }
}