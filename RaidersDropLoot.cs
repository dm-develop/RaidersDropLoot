using Il2Cpp;
using Il2CppInterop.Runtime;
using MelonLoader;
using UnityEngine;
using Type = Il2CppSystem.Type;

namespace dm.ffmods.raidersdroploot
{
    public class RaidersDropLoot : MelonMod
    {
        #region Fields

        public SpawnConfig Config;
        public GameManager GameManager;

        private float checkIntervalInSeconds = 1f;
        private bool frontierHasLoaded = false;
        private float timeSinceLastCheckInSeconds = 0f;

        #endregion Fields

        #region Properties

        public bool HasInitalised { get; private set; }

        #endregion Properties

        #region Public Methods

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Raiders drop loot mod loaded!");
        }

        public override void OnLateUpdate()
        {
            if (HasInitalised)
            {
                return;
            }
            // only continue if timer says so
            timeSinceLastCheckInSeconds += Time.deltaTime;
            if (timeSinceLastCheckInSeconds < checkIntervalInSeconds)
            {
                return;
            }
            // only continue if main scene has loaded
            if (!frontierHasLoaded)
            {
                return;
            }
            // only continue if GaneManager can been found
            if ((System.Object)(object)GameObject.Find("GameManager") == (System.Object)null)
            {
                LoggerInstance.Msg($"could not find gameManager instance, will try again in {checkIntervalInSeconds}...");
                timeSinceLastCheckInSeconds = 0;
                return;
            }
            // find GaneManager
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            // only continue if sword can be found
            if (!TryFindPrefab())
            {
                LoggerInstance.Msg($"could not find sword, will try again in {checkIntervalInSeconds}...");
                timeSinceLastCheckInSeconds = 0;
                return;
            }

            if (TryFindPrefab())
            {
                LoggerInstance.Msg($"sword prefab found, creating config...");
                Config = CreateConfig(GameManager);
            }

            HasInitalised = true;
            LoggerInstance.Msg($"mod initialised!");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
            if (sceneName == "Frontier")
            {
                frontierHasLoaded = true;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static bool TryFindPrefab()
        {
            Type val = Il2CppType.Of<GameObject>();
            var allObjects = Resources.FindObjectsOfTypeAll(val);

            foreach (var obj in allObjects)
            {
                if (obj.Cast<GameObject>().name == "Melee_Sword01A_Resource")
                {
                    return true;
                }
            }
            return false;
        }

        private WeaponResource AssignPrefab()
        {
            Type val = Il2CppType.Of<GameObject>();
            var allObjects = Resources.FindObjectsOfTypeAll(val);

            foreach (var obj in allObjects)
            {
                if (obj.Cast<GameObject>().name == "Melee_Sword01A_Resource")
                {
                    return obj.Cast<GameObject>().GetComponentInChildren<WeaponResource>();
                }
            }
            // should never be reached, because we check before
            return new WeaponResource();
        }

        private SpawnConfig CreateConfig(GameManager gameManager)
        {
            WeaponResource prefab = AssignPrefab();

            //string guid = "4e93b64a-d487-42c6-bcca-c266dfa8370e"; // sword guid maybe;
            return new SpawnConfig(prefab, gameManager.workBucketManager.itemWeapon, action);

            // the action to package into the config
            void action(DroppedResource instance)
            {
                GameManager.resourceManager.AddOrRemoveWeaponResource(instance.Cast<WeaponResource>(), false);
            }
        }

        #endregion Private Methods
    }
}