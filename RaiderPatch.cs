using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace dm.ffmods.raidersdroploot
{
    [HarmonyPatch(typeof(Raider), "OnDeath", new Type[] { typeof(float), typeof(GameObject), typeof(DamageType) })]
    public static class RaiderPatch
    {
        #region Private Methods

        private static void CreateSword(Vector3 position)
        {
            if (!Melon<RaidersDropLoot>.Instance.HasInitalised)
            {
                Melon<RaidersDropLoot>.Logger.Msg($"mod not initialised, ignoring spawn call ...");
                return;
            }
            var gameManager = Melon<RaidersDropLoot>.Instance.GameManager;
            var config = Melon<RaidersDropLoot>.Instance.Config;

            // create prefab instance
            DroppedResource instance = UnityEngine.Object.Instantiate(config.Prefab);

            // set its positions to where raider died
            instance.transform.localPosition = Vector3.zero;
            instance.transform.position = position;

            // make it useable (?)
            ItemStorage itemStorage = instance.GetComponent<ReservableItemStorage>().itemStorage;
            ItemBundle val = new ItemBundle(config.Item, 1u, 100u);
            itemStorage.AddItems(val);
            config.Action(instance);
            config.WorkBucketIdentifiers.ForEach(delegate (WorkBucketIdentifier i)
            {
                instance.AddToWorkBucket(gameManager.workBucketManager.Cast<IOwnerOfWorkBuckets>(), i, 0f);
            });
            instance.CheckWorkAvailability();

            // print success msg
            Melon<RaidersDropLoot>.Logger.Msg($"spawned sword at {position}");
        }

        //todo: check if it also works with postfix
        // link to harmony docs: https://harmony.pardeike.net/articles/patching-prefix.html
        private static bool Prefix(float damageTaken, GameObject damageCauser, Raider __instance)
        {
            // __instance gets us the instance of the Raider class
            Raider instance = __instance;

            // get info from instance
            string name = instance.raiderUnitData.name;
            Vector3 position = instance.pawnInstance.transform.position;

            //create sword where raider died
            CreateSword(position);

            // we want original OnDeath to run, so we set return value to true here for Harmony
            return true;
        }

        #endregion Private Methods
    }
}