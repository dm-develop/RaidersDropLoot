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
            { LootItem.gold, typeof(GoldIngotResource) },
            { LootItem.shoes, typeof(ShoesResource) },
            { LootItem.linenClothes, typeof(LinenClothesResource) },
        };

        public Dictionary<LootItem, SpawnPackage> SpawnPackages = new Dictionary<LootItem, SpawnPackage>();
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
                PrepPackage(item.Key, item.Value);
            }
        }

        public void SpawnLootItem(LootItem item, Vector3 position, uint amount)
        {
            if (!SpawnPackages.Keys.Contains(item))
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"cannot spawn '{item}', no spawn package found!");
                return;
            }
            SpawnItem(position, item, amount);
        }

        #endregion Public Methods

        #region Private Methods

        private void InitInstance(SpawnPackage package, DroppedResource instance, uint amount)
        {
            ItemStorage itemStorage = instance.GetComponent<ReservableItemStorage>().itemStorage;

            // we could put this into the settings in the future
            var condition = 100u;

            ItemBundle val = new ItemBundle(package.Item, amount, condition);
            itemStorage.AddItems(val);
            package.WorkBucketIdentifiers.ForEach(delegate (WorkBucketIdentifier identifier)
            {
                instance.AddToWorkBucket(gameManager.workBucketManager.Cast<IOwnerOfWorkBuckets>(), identifier, 0f);
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

        private void PrepPackage(LootItem itemType, GameObject prefab)
        {
            Type type = LookUpResourceType(itemType);

            string propName = $"item{LookUpItemName(itemType)}";

            WorkBucketManager workBucketManager = gameManager.workBucketManager;
            // Get the property info using reflection
            PropertyInfo propInfo = workBucketManager.GetType().GetProperty(propName);

            // Get the value of the property using reflection
            Item item = (Item)propInfo.GetValue(workBucketManager);

            SpawnPackages.Add(itemType,
            new SpawnPackage(
                itemType,
                prefab,
                item));
        }

        private void SpawnItem(Vector3 position, LootItem itemType, uint amount)
        {
            var package = SpawnPackages[itemType];

            //create instance
            DroppedResource instance = UnityEngine.Object.Instantiate(package.Resource);
            // set its positions to where raider died
            instance.transform.localPosition = Vector3.zero;
            instance.transform.position = position;

            InitInstance(package, instance, amount);
            if (Melon<RaidersDropLootMelon>.Instance.Verbose)
            {
                Melon<RaidersDropLootMelon>.Logger.Msg($"spawned loot item of type '{itemType}' at {position}.");
            }
        }

        #endregion Private Methods
    }
}