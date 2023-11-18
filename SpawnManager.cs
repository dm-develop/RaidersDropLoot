using Il2Cpp;
using MelonLoader;
using System.Reflection;

namespace dm.ffmods.raidersdroploot
{
    public static class SpawnManager
    {
        #region Fields

        public static Dictionary<LootItem, Type> LootItemTypes = new Dictionary<LootItem, Type>()
        {
            { LootItem.crudeWeapon, typeof(ItemSimpleWeapon) },
            { LootItem.hauberk, typeof(ItemHauberk) },
            { LootItem.heavyWeapon , typeof(ItemHeavyWeapon) },
            { LootItem.leatherCoat , typeof(ItemHideCoat) },
            { LootItem.plateMail , typeof(ItemPlatemail) },
            { LootItem.weapon , typeof(ItemWeapon) },
            { LootItem.shield, typeof(ItemShield) },
            { LootItem.bow , typeof(ItemBow) },
            { LootItem.crossbow , typeof(ItemCrossbow) },
            { LootItem.arrows, typeof(ItemArrow) },
            { LootItem.smokedMeat , typeof(ItemSmokedMeat) },
            { LootItem.bread , typeof(ItemBread) },
            { LootItem.gold, typeof(ItemGoldIngot) },
            { LootItem.shoes, typeof(ItemShoes) },
            { LootItem.linenClothes, typeof(ItemLinenClothes) },
        };

        #endregion Fields

        #region Public Methods

        public static void AddBundleToRaider(LootItem itemType, uint amount, Raider raider)
        {
            // check if tyoe is valid
            if (!LootItemTypes.ContainsKey(itemType))
            {
                Melon<RaidersDropLootMelon>.Logger.Error($"cannot find Item Type for '{itemType}'!");
                return;
            }

            // check if type has matching constructor
            Type type = LootItemTypes[itemType];
            ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                Melon<RaidersDropLootMelon>.Logger.Error($"cannot find constructor for '{itemType}'!");
                return;
            }

            // create instance
            Item item = (Item)ctor.Invoke(null);
            if (item == null)
            {
                Melon<RaidersDropLootMelon>.Logger.Error($"cannot create instance of '{itemType}' for bundle!");
                return;
            }

            raider.AddStolenItem(new ItemBundle(item, amount, 100U), null);

            if (Melon<RaidersDropLootMelon>.Instance.Verbose)
            {
                Melon<RaidersDropLootMelon>.Logger.Msg($"added bundle of {amount} units of {itemType} to raider.");
            }
        }

        #endregion Public Methods
    }
}