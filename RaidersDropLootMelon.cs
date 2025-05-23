﻿using MelonLoader;
using UnityEngine;

namespace dm.ffmods.raidersdroploot
{
    public class RaidersDropLootMelon : MelonMod
    {
        #region Fields

        public const string ConfigPath = "UserData/RaidersDropLootConfig.cfg";
        private uint checkIntervalInSeconds = 3;
        private ConfigManager configManager;
        private bool frontierHasLoaded = false;
        private GameManager gameManager;
        private bool hasParsedConfig = false;
        private LootManager lootManager;
        private LootSettingsManager lootSettingsManager;
        private DynamicLootScaler scaler;
        private bool setupDone0 = false;
        private bool setupDone1 = false;
        private bool setupDone2 = false;
        private float timeSinceLastCheckInSeconds = 0f;
        private float timeSinceLastLootUpdateInSeconds = 0f;
        private bool useFullTableAsDefault = false;
        private bool verbose = false;

        #endregion Fields

        #region Properties

        public GameManager GameManager { get => gameManager; }
        public bool HasInitalised { get; private set; }
        public LootManager LootManager { get => lootManager; }
        public DynamicLootScaler Scaler { get => scaler; }
        public bool UseFullTableAsDefault { get => useFullTableAsDefault; }
        public bool Verbose { get => verbose; }

        #endregion Properties

        #region Public Methods

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Setting up RaidersDropLoot mod ...");
            configManager = new ConfigManager();
            lootSettingsManager = new LootSettingsManager(ConfigPath);

            // set loot settings
            lootSettingsManager.UpdateLootSettings();
            verbose = lootSettingsManager.IsVerbose;
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

            // only continue if GameManager can be found
            if (!CanFindGameManager())
            {
                timeSinceLastCheckInSeconds = 0;
                return;
            }
            // find GameManager
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            // create loot scaler
            scaler = new DynamicLootScaler(gameManager, lootSettingsManager);
            lootManager = new LootManager(scaler);

            // print progress
            if (!setupDone2)
            {
                LoggerInstance.Msg("[2/2] Reading config file ...");
                setupDone2 = true;
            }

            if (!hasParsedConfig)
            {
                // parse config file
                configManager.InitConfig(lootManager, lootSettingsManager);
                hasParsedConfig = true;
            }

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
            if (sceneName == "Frontier")
            {
                frontierHasLoaded = true;
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Frontier")
            {
                Reset();
                LoggerInstance.Warning($"'Frontier' Scene unloaded, resetting RaidersDropLoot mod ...");
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

            if (timeSinceLastLootUpdateInSeconds > lootSettingsManager.DropChanceAdjustmentIntervalInSeconds)
            {
                if (Melon<RaidersDropLootMelon>.Instance.Verbose)
                {
                    Melon<RaidersDropLootMelon>.Logger.Msg($"updating loot chances based on player wealth ...");
                }
                scaler.CalculateAdjustedLootChances();
                timeSinceLastLootUpdateInSeconds = 0f;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private bool CanFindGameManager()
        {
            if ((System.Object)(object)GameObject.Find("GameManager") == (System.Object)null)
            {
                if (verbose)
                {
                    LoggerInstance.Warning($"could not find gameManager instance, will try again in {checkIntervalInSeconds} seconds ...");
                }
                return false;
            }
            return true;
        }

        private void Reset()
        {
            HasInitalised = false;
            setupDone0 = false;
            setupDone1 = false;
        }

        #endregion Private Methods
    }
}