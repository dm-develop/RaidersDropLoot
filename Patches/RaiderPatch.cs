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

            // __instance gets us the instance of the Raider class
            Raider instance = __instance;

            // get raid group size
            int size = instance.raiderGroup.groupSize;
            Melon<RaidersDropLootMelon>.Logger.Warning($"groupsize was: {size}");

            // get LootRoller instance
            var lootManager = Melon<RaidersDropLootMelon>.Instance.LootManager;

            // determine loot
            RaiderType type = LootManager.DetermineRaiderTypeFromUnitName(instance.raiderUnitData.name);

            // check if there is a loot table for this raider type
            if (!lootManager.IsLootable(type))
            {
                return true;
            }

            // get loor for raider typer
            var loot = lootManager.RollLoot(type);
            // check if there really is loot
            if (!loot.Any())
            {
                return true;
            }

            // spawn it
            if (loot.Any())
            {
                foreach (var item in loot)
                {
                    var amount = lootManager.LootTables[type].Drops[item].AmountInBundle;
                    ItemManager.AddBundleToRaider(item, amount, __instance);
                }
            }

            // we want original OnDeath to run, so we set return value to true here for Harmony
            return true;
        }

        #endregion Private Methods
    }
}