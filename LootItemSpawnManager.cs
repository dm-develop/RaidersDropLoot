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

                    default:
                        Melon<RaidersDropLoot>.Logger.Msg($" skipping prep of '{item.Key}', not implemented yet!");
                        break;
                }
            }
        }

        public void SpawnLootItem(LootItem item, Vector3 position)
        {
            switch (item)
            {
                case LootItem.crudeWeapon:
                    SpawnSimpleWeapon(position);
                    break;

                default:
                    Melon<RaidersDropLoot>.Logger.Msg($" cannot spawn '{item}', not implemented yet!");
                    break;
            }
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

        private void PrepSimpleWeaponPackage(GameObject prefab)
        {
            SpawnPackages.Add(LootItem.crudeWeapon,
                new LootItemSpawnPackage(
                    LootItem.crudeWeapon,
                    prefab,
                    AddSimpleWeaponResource,
                    gameManager.workBucketManager.itemSimpleWeapon, 1));
        }

        private void SpawnSimpleWeapon(Vector3 position)
        {
            var package = SpawnPackages[LootItem.crudeWeapon];

            //create instance
            DroppedResource instance = UnityEngine.Object.Instantiate(package.Resource);

            InitInstance(package, instance);
        }

        #endregion Private Methods
    }
}