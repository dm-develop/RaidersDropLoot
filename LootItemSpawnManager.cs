using Il2Cpp;
using MelonLoader;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace dm.ffmods.raidersdroploot
{
    public class LootItemSpawnManager
    {
        #region Fields

        public Dictionary<LootItem, LootItemSpawnPackage> SpawnPackages = new Dictionary<LootItem, LootItemSpawnPackage>();
        private GameManager gameManager;

        #endregion Fields

        #region Public Constructors

        public LootItemSpawnManager(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        #endregion Public Constructors

        #region Public Methods

        public void PrepAllPackages(Dictionary<LootItem, GameObject> ItemPrefabs)
        {
            foreach (var item in ItemPrefabs)
            {
                switch (item.Key)
                {
                    case LootItem.crudeWeapon:
                        PrepSimpleWeaponPackage(item.Value);
                        break;

                    case LootItem.weapon:
                        PrepWeaponPackage(item.Value);
                        break;

                    case LootItem.heavyWeapon:
                        PrepHeavyWeaponPackage(item.Value);
                        break;

                    case LootItem.bow:
                        PrepBowPackage(item.Value);
                        break;

                    case LootItem.crossbow:
                        PrepCrossbowPackage(item.Value);
                        break;

                    case LootItem.arrows:
                        PrepArrowsPackage(item.Value);
                        break;

                    case LootItem.shield:
                        PrepShieldPackage(item.Value);
                        break;

                    case LootItem.leatherCoat:
                        PrepHideCoatPackage(item.Value);
                        break;

                    case LootItem.hauberk:
                        PrepHauberkPackage(item.Value);
                        break;

                    case LootItem.plateMail:
                        PrepPlateMailPackage(item.Value);
                        break;

                    case LootItem.smokedMeat:
                        PrepSmokedMeatPackage(item.Value);
                        break;

                    case LootItem.bread:
                        PrepBreadPackage(item.Value);
                        break;

                    case LootItem.gold:
                        PrepGoldPackage(item.Value);
                        break;

                    default:
                        Melon<RaidersDropLootMelon>.Logger.Msg($"skipping prep of '{item.Key}', not implemented yet!");
                        break;
                }
            }
        }

        public void SpawnLootItem(LootItem item, Vector3 position)
        {
            if (!SpawnPackages.Keys.Contains(item))
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"cannot spawn '{item}', no spawn package found!");
                return;
            }
            SpawnItem(position, item);
        }

        #endregion Public Methods

        #region Private Methods

        private void AddArrowResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveArrowResource(instance.Cast<ArrowResource>(), false);
        }

        private void AddBowResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveBowResource(instance.Cast<BowResource>(), false);
        }

        private void AddBreadResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveBreadResource(instance.Cast<BreadResource>(), false);
        }

        private void AddCrossbowResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveCrossbowResource(instance.Cast<CrossbowResource>(), false);
        }

        private void AddGoldResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveGoldIngotResource(instance.Cast<GoldIngotResource>(), false);
        }

        private void AddHauberkResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveHauberkResource(instance.Cast<HauberkResource>(), false);
        }

        private void AddHeavyWeaponResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveHeavyWeaponResource(instance.Cast<HeavyWeaponResource>(), false);
        }

        private void AddHideCoatResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveHideCoatResource(instance.Cast<HideCoatResource>(), false);
        }

        private void AddPlatemailResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemovePlatemailResource(instance.Cast<PlatemailResource>(), false);
        }

        private void AddShieldResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveShieldResource(instance.Cast<ShieldResource>(), false);
        }

        private void AddSimpleWeaponResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveSimpleWeaponResource(instance.Cast<SimpleWeaponResource>(), false);
        }

        private void AddSmokedMeatResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveSmokedMeatResource(instance.Cast<SmokedMeatResource>(), false);
        }

        private void AddWeaponResource(DroppedResource instance)
        {
            gameManager.resourceManager.AddOrRemoveWeaponResource(instance.Cast<WeaponResource>(), false);
        }

        private void InitInstance(LootItemSpawnPackage package, DroppedResource instance)
        {
            ItemStorage itemStorage = instance.GetComponent<ReservableItemStorage>().itemStorage;
            ItemBundle val = new ItemBundle(package.Item, package.Amount, 100u);
            itemStorage.AddItems(val);
            package.Action(instance);
            package.WorkBucketIdentifiers.ForEach(delegate (WorkBucketIdentifier i)
            {
                instance.AddToWorkBucket(gameManager.workBucketManager.Cast<IOwnerOfWorkBuckets>(), i, 0f);
            });
            instance.CheckWorkAvailability();
        }

        private void PrepArrowsPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.arrows,
                new LootItemSpawnPackage(
                    LootItem.arrows,
                    prefab,
                    AddArrowResource,
                    gameManager.workBucketManager.itemArrow, 10));
        }

        private void PrepBowPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.bow,
                new LootItemSpawnPackage(
                    LootItem.bow,
                    prefab,
                    AddBowResource,
                    gameManager.workBucketManager.itemBow, 1));
        }

        private void PrepBreadPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.bread,
                new LootItemSpawnPackage(
                    LootItem.bread,
                    prefab,
                    AddBreadResource,
                    gameManager.workBucketManager.itemBread, 1));
        }

        private void PrepCrossbowPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.crossbow,
                new LootItemSpawnPackage(
                    LootItem.crossbow,
                    prefab,
                    AddCrossbowResource,
                    gameManager.workBucketManager.itemCrossbow, 1));
        }

        private void PrepGoldPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.gold,
                new LootItemSpawnPackage(
                    LootItem.gold,
                    prefab,
                    AddGoldResource,
                    gameManager.workBucketManager.itemGoldIngot, 3));
        }

        private void PrepHauberkPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.hauberk,
                new LootItemSpawnPackage(
                    LootItem.hauberk,
                    prefab,
                    AddHauberkResource,
                    gameManager.workBucketManager.itemHauberk, 1));
        }

        private void PrepHeavyWeaponPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.heavyWeapon,
                new LootItemSpawnPackage(
                    LootItem.heavyWeapon,
                    prefab,
                    AddHeavyWeaponResource,
                    gameManager.workBucketManager.itemHeavyWeapon, 1));
        }

        private void PrepHideCoatPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.leatherCoat,
                new LootItemSpawnPackage(
                    LootItem.leatherCoat,
                    prefab,
                    AddHideCoatResource,
                    gameManager.workBucketManager.itemHideCoat, 1));
        }

        private void PrepPlateMailPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.plateMail,
                new LootItemSpawnPackage(
                    LootItem.plateMail,
                    prefab,
                    AddPlatemailResource,
                    gameManager.workBucketManager.itemPlatemail, 1));
        }

        private void PrepShieldPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.shield,
                new LootItemSpawnPackage(
                    LootItem.shield,
                    prefab,
                    AddShieldResource,
                    gameManager.workBucketManager.itemShield, 1));
        }

        private void PrepSimpleWeaponPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.crudeWeapon,
                new LootItemSpawnPackage(
                    LootItem.crudeWeapon,
                    prefab,
                    AddSimpleWeaponResource,
                    gameManager.workBucketManager.itemSimpleWeapon, 1));
        }

        private void PrepSmokedMeatPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.smokedMeat,
                new LootItemSpawnPackage(
                    LootItem.smokedMeat,
                    prefab,
                    AddSmokedMeatResource,
                    gameManager.workBucketManager.itemSmokedMeat, 1));
        }

        private void PrepWeaponPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.weapon,
                new LootItemSpawnPackage(
                    LootItem.weapon,
                    prefab,
                    AddWeaponResource,
                    gameManager.workBucketManager.itemWeapon, 1));
        }

        private void SpawnItem(Vector3 position, LootItem itemType)
        {
            var package = SpawnPackages[itemType];

            //create instance
            DroppedResource instance = UnityEngine.Object.Instantiate(package.Resource);
            // set its positions to where raider died
            instance.transform.localPosition = Vector3.zero;
            instance.transform.position = position;

            InitInstance(package, instance);
            Melon<RaidersDropLootMelon>.Logger.Msg($"created '{itemType}' at {position}");
        }

        /// <summary>
        /// deprecated
        /// </summary>
        /// <param name="position"> </param>
        private void SpawnSimpleWeapon(Vector3 position)
        {
            var package = SpawnPackages[LootItem.crudeWeapon];

            //create instance
            DroppedResource instance = UnityEngine.Object.Instantiate(package.Resource);
            // set its positions to where raider died
            instance.transform.localPosition = Vector3.zero;
            instance.transform.position = position;

            InitInstance(package, instance);
            Melon<RaidersDropLootMelon>.Logger.Msg($"created '{LootItem.crudeWeapon}' at {position}");
        }

        #endregion Private Methods
    }
}