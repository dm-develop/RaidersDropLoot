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
        private uint checkIntervalInSeconds = 3;
        private ConfigManager configManager;
        private bool frontierHasLoaded = false;
        private GameManager gameManager;
        private MelonPreferences_Entry<bool> isVerboseEntry;
        private LootManager lootManager;
        private uint lootUpdateIntervalInSeconds = 10;
        private DynamicLootScaler scaler;
        private bool setupDone0 = false;
        private bool setupDone1 = false;
        private bool setupDone2 = false;
        private float timeSinceLastCheckInSeconds = 0f;
        private float timeSinceLastLootUpdateInSeconds = 0f;
        private bool verbose = false;

        #endregion Fields

        #region Properties

        public GameManager GameManager { get => gameManager; }
        public bool HasInitalised { get; private set; }
        public LootManager LootManager { get => lootManager; }
        public DynamicLootScaler Scaler { get => scaler; }
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
                LoggerInstance.Msg("[0/2] Waiting for Frontier scene to load ...");
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
                LoggerInstance.Msg("[1/2] Fetching GameManager  ...");
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
            // find GameManager
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            // create loot scaler
            scaler = new DynamicLootScaler(gameManager);

            // print progress
            if (!setupDone2)
            {
                LoggerInstance.Msg("[2/2] Reading config file ...");
                setupDone2 = true;
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

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!HasInitalised)
            {
                return;
            }

            // counter for loot updates
            timeSinceLastLootUpdateInSeconds += Time.deltaTime;

            if (timeSinceLastLootUpdateInSeconds > lootUpdateIntervalInSeconds)
            {
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Msg($"updating loot chances based on player wealth ...");
                }
                scaler.CalculateAdjustedLootChances();
                timeSinceLastCheckInSeconds = 0f;
            }
        }

        #endregion Public Methods
    }
}