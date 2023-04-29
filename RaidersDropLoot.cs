using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace dm.ffmods.raidersdroploot
{
    public class RaidersDropLoot : MelonMod
    {
        #region Fields

        public GameManager GameManager;
        private float checkIntervalInSeconds = 1f;
        private bool frontierHasLoaded = false;
        private ModSetup modSetup;
        private float timeSinceLastCheckInSeconds = 0f;

        #endregion Fields

        #region Properties

        public bool HasInitalised { get; private set; }

        #endregion Properties

        #region Public Methods

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Raiders drop loot mod loaded!");
            modSetup = new ModSetup();
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
                LoggerInstance.Msg($"could not find gameManager instance, will try again in {checkIntervalInSeconds} ...");
                timeSinceLastCheckInSeconds = 0;
                return;
            }
            // find GaneManager
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            // only continue if sword can be found
            if (!modSetup.TryFindItemPrefabs())
            {
                LoggerInstance.Msg($"could not find all item prefabs, will try again in {checkIntervalInSeconds} ...");
                timeSinceLastCheckInSeconds = 0;
                return;
            }

            if (!modSetup.ArePrefabsMissing)
            {
                LoggerInstance.Msg($"all prefabs found, creating SpawnConfigs ...");
                Config = modSetup.CreateConfigs(GameManager);
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
    }
}