using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace dm.ffmods.raidersdroploot
{
    [HarmonyPatch(typeof(NewYearEvent))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(CEDateTime), typeof(Weather) })]
    public static class NewYearEventPatch
    {
        #region Private Methods

        private static bool Prefix()
        {
            if (!Melon<RaidersDropLootMelon>.Instance.HasInitalised)
            {
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Warning($"mod not initialised, skipping NewYearEvent hook ...");
                }
                return true;
            }

            if (Melon<RaidersDropLootMelon>.Instance.Verbose)
            {
                Melon<RaidersDropLootMelon>.Logger.Msg($"updating loot chances based on player wealth ...");
            }
            Melon<RaidersDropLootMelon>.Instance.Scaler.CalculateAdjustedLootChances();

            return true;
        }

        #endregion Private Methods
    }
}