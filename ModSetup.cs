using Il2Cpp;
using Il2CppInterop.Runtime;
using MelonLoader;
using UnityEngine;
using Type = Il2CppSystem.Type;

namespace dm.ffmods.raidersdroploot
{
    public enum LootItem
    {
        crudeWeapon,
        weapon,
        heavyWeapon,
        leatherCoat,
        hauberk,
        plateMail,
        shield,
        bow,
        crossbow,
        arrows,
        smokedMeat,
        bread
    }

    public class ModSetup
    {
        #region Fields

        public static Dictionary<LootItem, string> ItemNames = new Dictionary<LootItem, string>()
        {
            { LootItem.crudeWeapon, "Melee_Club01A_Resource" },
            { LootItem.hauberk, "Resource_Villager_Hauberk01A" },
            { LootItem.heavyWeapon , "Melee_Mace01A_Resource" },
            { LootItem.leatherCoat , "Resource_Villager_HideCoat01A"},
            { LootItem.plateMail , "Resource_Villager_Platemail01A"},
            { LootItem.weapon , "Melee_Sword01A_Resource"},
            { LootItem.shield, "Resource_Villager_Shield01A"},
            { LootItem.bow , "Ranged_Bow01A_Resource"},
            { LootItem.crossbow , "Ranged_Crossbow01A_Resource"},
            { LootItem.arrows, "Ranged_Bow01A_Ammo_Resource"},
            { LootItem.smokedMeat , "Resource_Villager_SmokedMeat01A"},
            { LootItem.bread , "Resource_Villager_Bread01A"}
        };

        public bool ArePrefabsMissing = true;

        public Dictionary<LootItem, GameObject> ItemPrefabs = new Dictionary<LootItem, GameObject>();

        #endregion Fields

        #region Public Methods

        public bool TryFindItemPrefabs()
        {
            // get all objects (also inactive and in "hidden scenes")
            Type val = Il2CppType.Of<GameObject>();
            var allObjects = Resources.FindObjectsOfTypeAll(val); // this is expensive!
            List<LootItem> itemsMissing = ItemNames.Keys.Except(ItemPrefabs.Keys).ToList();

            // check for all objects
            foreach (var obj in allObjects)
            {
                // is current object one of the prefabs we are still looking for
                foreach (var item in ItemNames)
                {
                    if (!itemsMissing.Contains(item.Key))
                    {
                        continue;
                    }
                    if (obj.Cast<GameObject>().name == item.Value)
                    {
                        ItemPrefabs.Add(item.Key, obj.Cast<GameObject>());
                        itemsMissing.Remove(item.Key);
                        Melon<RaidersDropLoot>.Logger.Msg($"found prefab for '{item.Key}!'");
                    }
                }
            }
            // report whether we found all prefabs
            if (itemsMissing.Count > 0)
            {
                ArePrefabsMissing = true;
            }
            if (itemsMissing.Count == 0)
            {
                ArePrefabsMissing = false;
            }
            return !ArePrefabsMissing;
        }

        #endregion Public Methods
    }
}