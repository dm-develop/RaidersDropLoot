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

        private static void CreateSimpleWeapon(Vector3 position)
        {
            if (!Melon<RaidersDropLoot>.Instance.HasInitalised)
            {
                Melon<RaidersDropLoot>.Logger.Msg($"mod not initialised, ignoring spawn call ...");
                return;
            }

            Melon<RaidersDropLoot>.Instance.SpawnManager.SpawnLootItem(LootItem.crudeWeapon, position);
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
            CreateSimpleWeapon(position);

            // we want original OnDeath to run, so we set return value to true here for Harmony
            return true;
        }

        #endregion Private Methods
    }
}