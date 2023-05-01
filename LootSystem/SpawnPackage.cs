using Il2Cpp;
using UnityEngine;

namespace dm.ffmods.raidersdroploot
{
    public class SpawnPackage
    {
        #region Fields

        public readonly Item Item;
        public readonly LootItem Name;
        public readonly GameObject Prefab;
        public readonly DroppedResource Resource;

        #endregion Fields

        #region Public Constructors

        public SpawnPackage(LootItem name, GameObject prefab, Item item)
        {
            Name = name;
            Prefab = prefab;
            Item = item;
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