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
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Warning($"mod not initialised, skipping Raider OnDeath hook ...");
                }
                return true;
            }

            if (damageCauser.TryGetComponent<AggressiveAnimal>(out _))
            {
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Warning($"raider was killed by animal, skipping loot roll ...");
                }
                return true;
            }

            // __instance gets us the instance of the Raider class
            Raider raider = __instance;

            // get LootRoller instance
            var lootManager = Melon<RaidersDropLootMelon>.Instance.LootManager;

            // determine loot
            RaiderType type = LootManager.DetermineRaiderTypeFromUnitName(raider.raiderUnitData.name);

            // check if there is a loot table for this raider type
            if (!lootManager.IsLootable(type))
            {
                return true;
            }

            // get loot for raider type
            var loot = lootManager.RollLoot(type);
            // check if there really is loot
            if (!loot.Any())
            {
                return true;
            }

            // spawn it
            foreach (var item in loot)
            {
                var amount = lootManager.LootTables[type].Drops[item].AmountInBundle;
                ItemManager.AddBundleToRaider(item, amount, raider);
            }

            // we want original OnDeath to run, so we set return value to true here for Harmony
            return true;
        }

        #endregion Private Methods
    }
}