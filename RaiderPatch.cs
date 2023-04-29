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

        //todo: check if it also works with postfix
        // link to harmony docs: https://harmony.pardeike.net/articles/patching-prefix.html
        private static bool Prefix(float damageTaken, GameObject damageCauser, Raider __instance)
        {
            if (!Melon<RaidersDropLootMelon>.Instance.HasInitalised)
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"mod not initialised, skipping Raider OnDeath hook ...");
                return true;
            }

            // __instance gets us the instance of the Raider class
            Raider instance = __instance;

            // get position from instance
            Vector3 position = instance.pawnInstance.transform.position;

            // get LootRoller instance
            var lootRoller = Melon<RaidersDropLootMelon>.Instance.LootRoller;

            // determine loot
            RaiderType type = lootRoller.DetermineRaiderType(instance.raiderUnitData.name);
            var loot = lootRoller.RollLoot(type);

            // spawn loot
            var spawner = Melon<RaidersDropLootMelon>.Instance.SpawnManager;
            foreach (var item in loot)
            {
                spawner.SpawnLootItem(item, position);
            }

            // we want original OnDeath to run, so we set return value to true here for Harmony
            return true;
        }

        #endregion Private Methods
    }
}