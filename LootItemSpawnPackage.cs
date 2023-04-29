using Il2Cpp;
using UnityEngine;

namespace dm.ffmods.raidersdroploot
{
    public class LootItemSpawnPackage
    {
        #region Fields

        public readonly Action<DroppedResource> Action;
        public readonly uint Amount;
        public readonly Item Item;
        public readonly LootItem Name;
        public readonly GameObject Prefab;
        public readonly DroppedResource Resource;

        #endregion Fields

        #region Public Constructors

        public LootItemSpawnPackage(LootItem name, GameObject prefab, Action<DroppedResource> action, Item item, uint amount)
        {
            Name = name;
            Prefab = prefab;
            Action = action;
            Item = item;
            Amount = amount;
            Resource = prefab.GetComponent<DroppedResource>();
        }

        #endregion Public Constructors

        #region Properties

        public List<WorkBucketIdentifier> WorkBucketIdentifiers => new List<WorkBucketIdentifier>
        {
            Resource.bucket_itemToCollect,
            Resource.bucket_hasItem,
            Resource.bucket_hasItemIgnoreWC
        };

        #endregion Properties
    }
}