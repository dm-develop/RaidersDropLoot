using Il2Cpp;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace dm.ffmods.raidersdroploot
{
    public class SpawnManager
    {
        #region Fields

        public static Dictionary<LootItem, Type> LootResourceTypes = new Dictionary<LootItem, Type>()
        {
            { LootItem.crudeWeapon, typeof(SimpleWeaponResource) },
            { LootItem.hauberk, typeof(HauberkResource) },
            { LootItem.heavyWeapon , typeof(HeavyWeaponResource) },
            { LootItem.leatherCoat , typeof(HideCoatResource) },
            { LootItem.plateMail , typeof(PlatemailResource) },
            { LootItem.weapon , typeof(WeaponResource) },
            { LootItem.shield, typeof(ShieldResource) },
            { LootItem.bow , typeof(BowResource) },
            { LootItem.crossbow , typeof(CrossbowResource) },
            { LootItem.arrows, typeof(ArrowResource) },
            { LootItem.smokedMeat , typeof(SmokedMeatResource) },
            { LootItem.bread , typeof(BreadResource) },
            { LootItem.gold, typeof(GoldIngotResource) }
        };

        public Dictionary<LootItem, LootItemSpawnPackage> SpawnPackages = new Dictionary<LootItem, LootItemSpawnPackage>();
        private GameManager gameManager;

        #endregion Fields

        #region Public Constructors

        public SpawnManager(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        #endregion Public Constructors

        #region Public Methods

        public void PrepAllPackages(Dictionary<LootItem, GameObject> ItemPrefabs)
        {
            foreach (var item in ItemPrefabs)
            {
                // todo: get amount from config
                PrepPackage(item.Key, item.Value, 1);
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

        private void AddResourceInstanceToManager(LootItem itemType, DroppedResource instance)
        {
            Type type = LookUpResourceType(itemType);

            var resName = LookUpItemName(itemType);

            string methodName = $"AddOrRemove{resName}Resource";

            // Create an instance of the class that contains the method
            var resManager = gameManager.resourceManager;

            // Get the method info using reflection
            MethodInfo methodInfo = resManager.GetType().GetMethod(methodName);

            // Call the method using reflection
            var paramArray = new object[] { Convert.ChangeType(instance, type), false };
            methodInfo.Invoke(resManager, paramArray);
        }

        private void InitInstance(LootItemSpawnPackage package, DroppedResource instance)
        {
            ItemStorage itemStorage = instance.GetComponent<ReservableItemStorage>().itemStorage;
            ItemBundle val = new ItemBundle(package.Item, package.Amount, 100u);
            itemStorage.AddItems(val);
            package.Action(package.Name, instance);
            package.WorkBucketIdentifiers.ForEach(delegate (WorkBucketIdentifier i)
            {
                instance.AddToWorkBucket(gameManager.workBucketManager.Cast<IOwnerOfWorkBuckets>(), i, 0f);
            });
            instance.CheckWorkAvailability();
        }

        private string LookUpItemName(LootItem itemType)
        {
            Type type = LookUpResourceType(itemType);
            // extract resource name from type
            var typeName = type.Name;
            string suffix = "Resource";
            if (!typeName.EndsWith(suffix))
            {
                Melon<RaidersDropLootMelon>.Logger.Error($"cannot find Item name for '{itemType}'!");
            }
            // strip suffix from name
            var itemName = typeName.Substring(0, typeName.Length - suffix.Length);
            return itemName;
        }

        private Type LookUpResourceType(LootItem itemType)
        {
            if (!LootResourceTypes.ContainsKey(itemType))
            {
                Melon<RaidersDropLootMelon>.Logger.Error($"cannot find Resource Type for '{itemType}'!");
            }
            return LootResourceTypes[itemType];
        }

        private void PrepPackage(LootItem itemType, GameObject prefab, uint amount)
        {
            if (amount < 1)
            {
                amount = 1;
                // write warning message
            }

            Type type = LookUpResourceType(itemType);

            string propName = $"item{LookUpItemName(itemType)}";

            WorkBucketManager workBucketManager = gameManager.workBucketManager;
            // Get the property info using reflection
            PropertyInfo propInfo = workBucketManager.GetType().GetProperty(propName);

            // Get the value of the property using reflection
            Item item = (Item)propInfo.GetValue(workBucketManager);

            SpawnPackages.Add(itemType,
            new LootItemSpawnPackage(
                itemType,
                prefab,
                AddResourceInstanceToManager,
                item, amount));
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
            if (Melon<RaidersDropLootMelon>.Instance.Verbose)
            {
                Melon<RaidersDropLootMelon>.Logger.Msg($"Spawned loot item of '{itemType}' at {position}.");
            }
        }

        #endregion Private Methods
    }
}