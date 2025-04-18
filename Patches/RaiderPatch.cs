using HarmonyLib;
using MelonLoader;
using System.Reflection;
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

            // determine loot these names likely do not work!

            // Access the protected raiderUnitData using reflection
            var fieldInfo = typeof(Raider).GetField("raiderUnitData", BindingFlags.NonPublic | BindingFlags.Instance);
            RaidIncursionUnit raiderUnitData = new RaidIncursionUnit();
            if (fieldInfo != null)
            {
                var nullableRaiderUnitData = fieldInfo.GetValue(__instance) as RaidIncursionUnit;
                if (nullableRaiderUnitData == null)
                {
                    Melon<RaidersDropLootMelon>.Logger.Warning($"could not retrieve raider unit data, no loot will be spawned.");
                    return true;
                }

                // Proceed with the non-nullable `raiderUnitData` safely
                raiderUnitData = nullableRaiderUnitData;
            }
            else
            {
                Melon<RaidersDropLootMelon>.Logger.Warning($"could not retrieve raider unit data, no loot will be spawned.");
                return true;
            }

            // get raider type based on name
            RaiderType type = LootManager.DetermineRaiderTypeFromUnitName(raiderUnitData.name);

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