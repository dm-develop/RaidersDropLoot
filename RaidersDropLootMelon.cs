using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace dm.ffmods.raidersdroploot
{
    public class RaidersDropLootMelon : MelonMod
    {
        #region Fields

        public const string ConfigPath = "UserData/RaidersDropLootConfig.cfg";
        public static MelonPreferences_Category LootPrefs;
        private float checkIntervalInSeconds = 3f;
        private ConfigManager configManager;
        private bool frontierHasLoaded = false;
        private GameManager gameManager;
        private MelonPreferences_Entry<bool> isVerboseEntry;
        private LootManager lootManager;
        private PrefabManager prefabManager;
        private bool setupDone0 = false;
        private bool setupDone1 = false;
        private bool setupDone2 = false;
        private bool setupDone3 = false;
        private bool setupDone4 = false;
        private SpawnManager spawnManager;
        private float timeSinceLastCheckInSeconds = 0f;
        private bool verbose = true;

        #endregion Fields

        #region Properties

        public bool HasInitalised { get; private set; }
        public LootManager LootManager { get => lootManager; }
        public SpawnManager SpawnManager { get => spawnManager; }
        public bool Verbose { get => verbose; set => verbose = value; }

        #endregion Properties

        #region Public Methods

        public List<MelonPreferences_Entry> GetPrefEntriesToIgnore()
        {
            return new List<MelonPreferences_Entry> { isVerboseEntry };
        }

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Setting up RaidersDropLoot mod ...");
            prefabManager = new PrefabManager();
            lootManager = new LootManager();
            configManager = new ConfigManager();

            LootPrefs = MelonPreferences.CreateCategory("RaidersDropLoot");
            LootPrefs.SetFilePath(ConfigPath);

            // set debug flag based on config file
            isVerboseEntry = LootPrefs.CreateEntry<bool>("isVerbose", false);
            verbose = isVerboseEntry.Value;
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

            // print progress
            if (!setupDone0)
            {
                LoggerInstance.Msg("[0/4] Waiting for Frontier scene to load ...");
                setupDone0 = true;
            }

            // only continue if main scene has loaded
            if (!frontierHasLoaded)
            {
                return;
            }

            // print progress
            if (!setupDone1)
            {
                LoggerInstance.Msg("[1/4] Fetching GameManager  ...");
                setupDone1 = true;
            }

            // only continue if GameManager can been found
            if ((System.Object)(object)GameObject.Find("GameManager") == (System.Object)null)
            {
                if (verbose)
                {
                    LoggerInstance.Warning($"could not find gameManager instance, will try again in {checkIntervalInSeconds} seconds ...");
                }
                timeSinceLastCheckInSeconds = 0;
                return;
            }
            // find GaneManager
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            spawnManager = new SpawnManager(gameManager);

            // print progress
            if (!setupDone2)
            {
                LoggerInstance.Msg("[2/4] Fetching loot item prefabs ...");
                setupDone2 = true;
            }

            // fetch prefabs
            if (!prefabManager.TryFindItemPrefabs())
            {
                if (verbose)
                {
                    LoggerInstance.Warning($"could not find all item prefabs, will try again in {checkIntervalInSeconds} seconds ...");
                }

                timeSinceLastCheckInSeconds = 0;
                return;
            }

            // print progress
            if (!setupDone3)
            {
                LoggerInstance.Msg("[3/4] Prepping prefabs for use ...");
                setupDone3 = true;
            }

            // prep loot for spawning
            if (!prefabManager.ArePrefabsMissing)
            {
                spawnManager.PrepAllPackages(prefabManager.ItemPrefabs);
            }

            // print progress
            if (!setupDone4)
            {
                LoggerInstance.Msg("[4/4] Reading config file ...");
                setupDone4 = true;
            }

            // parse config file
            configManager.InitConfig(lootManager, LootPrefs);

            if (!configManager.IsInitialised)
            {
                if (verbose)
                {
                    LoggerInstance.Warning($"could not initialise config, will try again in {checkIntervalInSeconds} seconds ...");
                }
                return;
            }

            // done!
            HasInitalised = true;
            LoggerInstance.Msg($"RaidersDropLoot mod initialised!");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (verbose)
            {
                LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
            }
            if (sceneName == "Frontier")
            {
                frontierHasLoaded = true;
            }
        }

        #endregion Public Methods
    }
}