using Il2Cpp;

namespace dm.ffmods.raidersdroploot
{
    public class SpawnConfig
    {
        #region Fields

        private readonly Action<DroppedResource> action;
        private readonly Item item;
        private readonly DroppedResource prefab;

        #endregion Fields

        #region Public Constructors

        public SpawnConfig(DroppedResource prefab, Item item, Action<DroppedResource> action)
        {
            this.prefab = prefab;
            this.item = item;
            this.action = action;
        }

        #endregion Public Constructors

        #region Properties

        public Action<DroppedResource> Action => action;
        public Item Item => item;
        public DroppedResource Prefab => prefab;

        public List<WorkBucketIdentifier> WorkBucketIdentifiers => new List<WorkBucketIdentifier>
    {
        prefab.bucket_itemToCollect,
        prefab.bucket_hasItem,
        prefab.bucket_hasItemIgnoreWC
    };

        #endregion Properties
    }
}