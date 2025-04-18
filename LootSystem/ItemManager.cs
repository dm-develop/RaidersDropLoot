using MelonLoader;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace dm.ffmods.raidersdroploot
{
    public static class ItemManager
    {
        #region Fields

        public static Dictionary<LootItem, Type> LootItemTypes = new Dictionary<LootItem, Type>()
        {
            //{ LootItem.animalTrap, typeof(ItemAnimalTrap) },
            { LootItem.arrow, typeof(ItemArrow) },
            { LootItem.basket, typeof(ItemBasket) },
            { LootItem.beans, typeof(ItemBeans) },
            { LootItem.berries, typeof(ItemBerries) },
            { LootItem.boarCarcass, typeof(ItemBoarCarcass) },
            //{ LootItem.books, typeof(ItemBooks) },
            { LootItem.bow, typeof(ItemBow) },
            { LootItem.bread, typeof(ItemBread) },
            { LootItem.brick, typeof(ItemBrick) },
            { LootItem.candle, typeof(ItemCandle) },
            { LootItem.carcass, typeof(ItemCarcass) },
            { LootItem.cheese, typeof(ItemCheese) },
            { LootItem.clay, typeof(ItemClay) },
            //{ LootItem.clover, typeof(ItemClover) },
            { LootItem.coal, typeof(ItemCoal) },
            { LootItem.crossbow, typeof(ItemCrossbow) },
            { LootItem.eggs, typeof(ItemEggs) },
            { LootItem.fireWood, typeof(ItemFirewood) },
            { LootItem.fish, typeof(ItemFish) },
            { LootItem.flax, typeof(ItemFlax) },
            { LootItem.flour, typeof(ItemFlour) },
            { LootItem.fruit, typeof(ItemFruit) },
            { LootItem.furniture, typeof(ItemFurniture) },
            { LootItem.glass, typeof(ItemGlass) },
            { LootItem.goldIngot, typeof(ItemGoldIngot) },
            { LootItem.goldOre, typeof(ItemGoldOre) },
            { LootItem.grain, typeof(ItemGrain) },
            { LootItem.greens, typeof(ItemGreens) },
            { LootItem.hauberk, typeof(ItemHauberk) },
            { LootItem.heavyWeapon, typeof(ItemHeavyWeapon) },
            { LootItem.herbs, typeof(ItemHerbs) },
            { LootItem.hideCoat, typeof(ItemHideCoat) },
            { LootItem.hide, typeof(ItemHide) },
            { LootItem.honey, typeof(ItemHoney) },
            { LootItem.ironOre, typeof(ItemIronOre) },
            { LootItem.linenClothes, typeof(ItemLinenClothes) },
            { LootItem.logs, typeof(ItemLogs) },
            { LootItem.meat, typeof(ItemMeat) },
            { LootItem.medicine, typeof(ItemMedicine) },
            { LootItem.milk, typeof(ItemMilk) },
            { LootItem.mushroom, typeof(ItemMushroom) },
            { LootItem.nuts, typeof(ItemNuts) },
            //{ LootItem.paper, typeof(ItemPaper) },
            { LootItem.planks, typeof(ItemPlanks) },
            { LootItem.platemail, typeof(ItemPlatemail) },
            { LootItem.pottery, typeof(ItemPottery) },
            { LootItem.preservedVeg, typeof(ItemPreservedVeg) },
            { LootItem.preserves, typeof(ItemPreserves) },
            { LootItem.roots, typeof(ItemRoots) },
            { LootItem.sand, typeof(ItemSand) },
            { LootItem.shield, typeof(ItemShield) },
            { LootItem.shoes, typeof(ItemShoes) },
            { LootItem.simpleWeapon, typeof(ItemSimpleWeapon) },
            { LootItem.smallCarcass, typeof(ItemSmallCarcass) },
            { LootItem.smokedFish, typeof(ItemSmokedFish) },
            { LootItem.smokedMeat, typeof(ItemSmokedMeat) },
            { LootItem.soap, typeof(ItemSoap) },
            { LootItem.stone, typeof(ItemStone) },
            { LootItem.tallow, typeof(ItemTallow) },
            { LootItem.tool, typeof(ItemTool) },
            { LootItem.water, typeof(ItemWater) },
            { LootItem.wax, typeof(ItemWax) },
            { LootItem.weapon, typeof(ItemWeapon) },
            { LootItem.wheatBeer, typeof(ItemWheatBeer) },
            { LootItem.willow, typeof(ItemWillow) },
            { LootItem.wolfCarcass, typeof(ItemWolfCarcass) },
        };

        #endregion Fields

        #region Public Methods

        public static void AddBundleToRaider(LootItem itemType, uint amount, Raider raider)
        {
            // check if type is valid
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

        public static string GetItemInfoName(LootItem item)
        {
            string suffix = "ItemInfo";
            string infoName;
            switch (item)
            {
                case LootItem.berries:
                    infoName = "berry" + suffix;
                    break;

                case LootItem.fireWood:
                    infoName = "firewood" + suffix;
                    break;

                case LootItem.planks:
                    infoName = "plank" + suffix;
                    break;

                default:
                    infoName = item.ToString() + suffix;
                    break;
            }
            return infoName;
        }

        #endregion Public Methods
    }
}