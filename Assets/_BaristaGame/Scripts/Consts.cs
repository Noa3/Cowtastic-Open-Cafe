public static class Consts
{
    public const string PrefixNewGameVersion = "1.3.0.24b"; //This will be used to reset all gamedata, change if new data should be generated

    #region Addressable Keys

    /// <summary>
    /// Addressable asset keys used throughout the application
    /// </summary>
    public static class AddressableKeys
    {
        public const string LoadingScreen = "LoadingScreen";
    }

    #endregion

    #region Error Messages

    /// <summary>
    /// Standardized error messages for consistent logging
    /// </summary>
    public static class ErrorMessages
    {
        public const string InvalidSceneName = "Scene name cannot be null or empty";
        public const string InvalidAssetArray = "Asset array cannot be null or empty";
        public const string SceneLoadFailed = "Failed to load scene";
        public const string ResourceLocationLoadFailed = "Failed to load resource locations";
        public const string LoadingScreenNotFound = "Loading screen prefab not found in Addressables";
        public const string SliderComponentNotFound = "Slider component not found in loading screen prefab";
        public const string SliderCreationFailed = "Failed to create loading slider";
        public const string OperationCancelled = "Loading operation was cancelled";
        public const string AssetLoadCancelled = "Asset loading operation was cancelled";
        public const string UnexpectedError = "Unexpected error occurred while";
        public const string ComponentNotFound = "Component not found during initialization";
    }

    #endregion

    #region Scene Names

    /// <summary>
    /// Game scene names used throughout the application
    /// </summary>
    public static class SceneNames
    {
        public const string GameArcadeNormal = "Game_Arcade";
        public const string GameArcadeHard = "Game_Arcade_Hard";
        public const string GameArcadeCasual = "Game_Arcade_Casual";
        public const string GameArcadeChaos = "Game_Arcade_Chaos";
        public const string GameArcadeUltraChaos = "Game_Arcade_UltraChaos";
        public const string GameArcadeHoliday = "Game_Arcade_Holiday";
    }

    #endregion

    #region Time Management

    /// <summary>
    /// Constants related to time management and best time tracking
    /// </summary>
    public static class TimeManagement
    {
        public const float DefaultBestTime = 99999999f;
        public const int SecondsPerMinute = 60;
        public const string TimeFormatSecondsPlaceholder = "00";
        public const string TimeFormatSeparator = ":";
        public const string TimeDisplaySeparator = " ";
    }

    #endregion

    #region Game Mode Arcade

    /// <summary>
    /// Constants specific to Arcade game mode
    /// </summary>
    public static class GameModeArcade
    {
        public const int MilkyModeUpgradeAmount = 1;
        public const float DefaultMilkyModeMinUpgradeTime = 60f;
        public const float DefaultMilkyModeMaxUpgradeTime = 120f;
        public const float DefaultMilkyModeProductionRate = 1f;
        public const float DefaultMilkyModeUpgradeValue = 2.5f;
        public const float DefaultProductionRateMultiplier = 1.05f;
    }

    #endregion

    #region Dialogue System

    /// <summary>
    /// Constants related to dialogue system functionality
    /// </summary>
    public static class DialogueSystem
    {
        public const int DefaultFlusteredLevel = -1;
        public const int ExpectedOrderValuesCount = 14;
        public const byte MaxAudioClipSearchAttempts = 100;
        public const float MinimumIngredientThreshold = 0.01f;

        /// <summary>
        /// Indices for ingredient values in order arrays
        /// </summary>
        public static class IngredientIndices
        {
            public const int Chocolate = 0;
            public const int Milk = 1;
            public const int Tea = 2;
            public const int Cream = 3;
            public const int Espresso = 4;
            public const int Sugar = 5;
            public const int Coffee = 6;
            public const int Boba = 7;
            public const int Ice = 8;
            public const int WhippedCream = 9;
            public const int ChocolateSauce = 10;
            public const int CaramelSauce = 11;
            public const int Sprinkles = 12;
            public const int BreastMilk = 13;
        }

        /// <summary>
        /// Flustered level ranges and validation
        /// </summary>
        public static class FlusteredLevels
        {
            public const int MinLevel = -1;
            public const int MaxLevel = 4;
            public const int NormalLevel = 0;
            public const int Level1 = 1;
            public const int Level2 = 2;
            public const int Level3 = 3;
            public const int Level4 = 4;
        }
    }

    #endregion

    #region Order Manager

    /// <summary>
    /// Constants specific to Order Manager functionality
    /// </summary>
    public static class OrderManager
    {
        /// <summary>
        /// Dialogue probability configuration for different flustered levels
        /// </summary>
        public static class DialogueProbabilities
        {
            public const float GreetingBase = 0.3f;
            public const float GreetingIncrement = 0.15f;
            public const float PreBase = 0.8f;
            public const float PreDecrement = 0.2f;
            public const float ModifierBase = 0.2f;
            public const float ModifierVariation = 0.1f;
            public const float DrinkTypeDecrement = 0.2f;
            public const float WithoutIngredientBase = 0.1f;
            public const float WithoutIngredientIncrement = 0.1f;
            // Added precomputed per-level probability arrays used in OrderManager dialogue generation
            public static readonly float[] GreetingPerLevel = { 0.3f, 0.5f, 0.6f, 0.8f, 0.95f };
            public static readonly float[] PrePerLevel = { 0.8f, 0.6f, 0.4f, 0.15f, 0f }; // previously Pre probability logic
            public static readonly float[] ModifierPerLevel = { 0.2f, 0.35f, 0.4f, 0.1f, 0f };
            public static readonly float[] TypePerLevel = { 1f, 1f, 1f, 0.5f, 0.2f };
            public static readonly float[] WithoutIngredientPerLevel = { 0.1f, 0.25f, 0.4f, 0.5f, 0.05f };
        }

        /// <summary>
        /// Drink modifier type identifiers
        /// </summary>
        public static class DrinkModifierTypes
        {
            public const string Strong = "Strong";
            public const string Coffee = "Coffee";
            public const string Dark = "Dark";
            public const string Tea = "Tea";
            public const string Milk = "Milk";
            public const string BreastMilk = "Breast Milk";
            public const string Thick = "Thick";
            public const string Sweet = "Sweet";
            public const string Iced = "Iced";
            public const string Boba = "Boba";
            public const string Fluffy = "Fluffy";
            public const string Fancy = "Fancy";
            public const string Chocolate = "Chocolate";
            public const string Fun = "Fun";
        }

        /// <summary>
        /// Drink type identifiers
        /// </summary>
        public static class DrinkTypes
        {
            public const string Coffee = "Coffee";
            public const string Milk = "Milk";
            public const string Latte = "Latte";
            public const string Tea = "Tea";
            public const string Shake = "Shake";
            public const string ChocolateShake = "Chocolate Shake";
        }

        /// <summary>
        /// Filling type identifiers
        /// </summary>
        public static class FillingTypes
        {
            public const string Espresso = "Espresso";
            public const string Coffee = "Coffee";
            public const string Chocolate = "Chocolate";
            public const string Tea = "Tea";
            public const string Milk = "Milk";
            public const string BreastMilk = "BreastMilk";
            public const string Cream = "Cream";
            public const string Sugar = "Sugar";
            public const string Ice = "Ice";
            public const string Boba = "Boba";
        }

        /// <summary>
        /// Ingredient accuracy validation constants
        /// </summary>
        public static class AccuracyValidation
        {
            public const float ToleranceThreshold = 0.01f;
            public const float MinimumFillLevel = 0.7f;
        }

        /// <summary>
        /// Fallback ingredient for "not included" dialogue
        /// </summary>
        public const string FallbackIngredient = "Sawdust";
    }

    #endregion

    #region Barista Talk System

    /// <summary>
    /// Constants for barista dialogue system configuration
    /// </summary>
    public static class BaristaTalk
    {
        // Default timing values
        public const float DefaultEventOffsetEnd = 2f;
        public const float DefaultGreetOffsetStart = 6f;
        public const float DefaultGreetOffsetEnd = 2f;
        public const float DefaultIdleMinTime = 60f;
        public const float DefaultIdleMaxTime = 180f;
        public const float DefaultApronLimit = 23f;
        public const float DefaultTooFullMinTime = 30f;
        public const float DefaultTooFullLimit = 90f;
        public const float DefaultBuyUpgradeMinTime = 30f;
        public const float DefaultBuyUpgradeTalkChance = 20f;
        public const float DefaultResetCupMinTime = 30f;
        public const float DefaultPatHeadMinTime = 10f;
        public const int DefaultBreastMilkAddMin = 30;
        public const int DefaultBreastMilkAddMax = 180;

        // Range constraints
        public const float ApronLimitMin = 10f;
        public const float ApronLimitMax = 70f;
        public const float TooFullMinTimeMin = 50f;
        public const float TooFullMinTimeMax = 100f;
        public const float TalkChanceMin = 1f;
        public const float TalkChanceMax = 100f;

        /// <summary>
        /// Dialogue key identifiers for localization mapping
        /// </summary>
        public static class DialogueKeys
        {
            public const string StartGameArcade = "StartGame_Arcade";
            public const string Cookie = "Cookie";
            public const string CookieBuyed = "CookieBuyed";
            public const string ApronLimit = "ApronLimit";
            public const string TooFull = "TooFull";
            public const string BuyUpgrade = "BuyUpgrade";
            public const string WelcomingNewCustomer = "WelcomingNewCustomer";
            public const string BadEnd = "BadEnd";
            public const string FinishCup = "FinishCup";
            public const string ResetCup = "BaristaTalk_ResetCup";
            public const string AddMilk = "BaristaTalk_AddMilk";
            public const string PatHead = "BaristaTalk_PatHead";

            // Mood dialogues
            public const string IdleMood20 = "IdleMood_20";
            public const string IdleMood40 = "IdleMood_40";
            public const string IdleMood60 = "IdleMood_60";
            public const string IdleMood80 = "IdleMood_80";
            public const string IdleMood100 = "IdleMood_100";

            // Bust dialogues
            public const string IdleBust20 = "IdleBust_20";
            public const string IdleBust50 = "IdleBust_50";
            public const string IdleBust80 = "IdleBust_80";
            public const string IdleBust100 = "IdleBust_100";

            // Money dialogues
            public const string MoneyAbove25 = "MoneyAbove_25";
            public const string MoneyAbove100 = "MoneyAbove_100";
        }

        /// <summary>
        /// Thresholds for mood-based dialogue selection
        /// </summary>
        public static class MoodThresholds
        {
            public const float VeryHappy = 80f;
            public const float Happy = 55f;
            public const float Neutral = 25f;
            public const float Sad = 10f;
        }

        /// <summary>
        /// Thresholds for bust size-based dialogue selection
        /// </summary>
        public static class BustThresholds
        {
            public const float VeryLarge = 75f;
            public const float Large = 40f;
            public const float Medium = 10f;
        }

        /// <summary>
        /// Thresholds for money-based dialogue selection
        /// </summary>
        public static class MoneyThresholds
        {
            public const float HighTier = 100f;
            public const float MidTier = 25f;
        }

        /// <summary>
        /// Mood variation range for randomization
        /// </summary>
        public static class MoodVariation
        {
            public const int Min = -15;
            public const int Max = 15;
        }

        /// <summary>
        /// Localization string separator constants
        /// </summary>
        public static class Localization
        {
            public const char DialogueSeparator = ';';
            public const char AudioSeparator = '@';
            public const string EnglishLocaleCode = "en";
        }
    }

    #endregion

    #region Game Settings

    /// <summary>
    /// Constants for game settings and configuration
    /// </summary>
    public static class GameSettings
    {
        // Time scale constants
        public const float PausedTimeScale = 0f;
        public const float NormalTimeScale = 1f;

        // Default Discord URL
        public const string DefaultDiscordURL = "https://discord.gg/VCm2WYhG";

        // Volume defaults and constraints
        public const float DefaultVolumeSoundFx = 1f;
        public const float DefaultVolumeMusic = 0.7f;
        public const float DefaultVolumeTalk = 0.7f;
        public const float MinVolumeValue = 0f;
        public const float MaxVolumeValue = 1f;

        // Graphics quality defaults and constraints
        public const int DefaultTextureQuality = 0;
        public const int MinTextureQuality = 0;
        public const int MaxTextureQuality = 3;

        // Audio mixer conversion constants
        public const float DecibelMultiplier = 20f;
        public const float MinAudioMixerValue = -80f;

        // Platform defaults
        public const bool DefaultCameraMoveAndroid = false;
        public const bool DefaultCameraMoveDesktop = true;
    }

    #endregion

    //Barista Controller
    public const string BustSize = "BustSize";
    public const string BaristaFullness = "Fullness";
    public const string Happiness = "Happiness";
    public const string BeingMilked = "BeingMilked";
    public const string Clothed = "Clothed";
    public const string Talking = "Talking";
    public const string SurpriseGrowth = "SurpriseGrowth";
    public const string MiniSurpriseGrowth = "MiniSurpriseGrowth";
    public const string Random = "Random";
    public const string BaristaFixApron = "FixOutfit";
    public const string BaristaReset = "Reset";
    public const string BaristaGoodEnd = "GoodEnd";
    public const string BaristaBadEnd = "BadEnd";
    public const string BaristaMouseX = "MouseX";
    public const string BaristaMouseY = "MouseY";
    public const string BaristaHeadPat = "HeadPat";
    public const string BaristaAutoFixOutfit = "AutoFixOutfit";

    // Animation Triggers
    public const string CustomerDialogFlusterIncrease = "Fluster";

    #region CupShader
    public const string CupShader_Fullness = "_CupFullness";
    public const string CupShader_MilkType = "_Milk_Type";
    public const string CupShader_MilkTypeColor = "_Custom_Milk_Color";

    public const string CupShader_Espresso = "_FillerEspresso";
    public const string CupShader_Coffee = "_FillerCoffee";
    public const string CupShader_Chocolate = "_FillerChocolate";
    public const string CupShader_Tea = "_FillerTea";
    public const string CupShader_Milk = "_FillerMilk";
    public const string CupShader_BreastMilk = "_FillerBreastMilk";
    public const string CupShader_Cream = "_FillerCream";
    public const string CupShader_Sugar = "_FillerSugar";
    public const string CupShader_Ice = "_ExtraIce";
    public const string CupShader_Boba = "_ExtraBoba";

    public const string CupShader_WhippedCream = "_ToppingCream";
    public const string CupShader_CaramelSauce = "_ToppingCaramel";
    public const string CupShader_ChocolateSauce = "_ToppingChocolate";
    public const string CupShader_Sprinkles = "_ToppingSprinkles";
    #endregion

    #region PlayerPrefs

    public const string PlayerPrefPrefix = "PlayerPref";
    public const string PlayerPrefNextIsTutorial = "NextIsTutorial";
    public const string PlayerPrefNextIsMilkyMode = "NextIsMilkyMode";
    public const string PlayerPrefSoundFx = "SoundFx";
    public const string PlayerPrefMusic = "Music";
    public const string PlayerPrefTalk = "Talk";
    public const string PlayerPrefMilkBarista = "MilkBarista";
    public const string PlayerPrefMilkCup = "MilkCup";

    public const string PlayerPrefMilkColorR = "MilkColorR";
    public const string PlayerPrefMilkColorG = "MilkColorG";
    public const string PlayerPrefMilkColorB = "MilkColorB";

    public const string PlayerPrefCanMoveCamera = "MoveCamera";
    public const string PlayerPrefShowBestTimes = "ShowBestTimes";

    public const string PlayerPrefShowIntroPopup = "PlayerPrefShowIntroPopup";

    //BestTimes
    public const string PlayerPrefBestTimeNormal = "BestTimeNormal";
    public const string PlayerPrefBestTimeHard = "BestTimeHard";
    public const string PlayerPrefBestTimeMilkymodeSuffix = "_Milkymode";

    public const string PlayerPrefBestTimeCasual = "BestTimeCasual";
    public const string PlayerPrefBestTimeChaos = "BestTimeChaos";
    public const string PlayerPrefBestTimeUltraChaos = "BestTimeUltraChaos";
    public const string PlayerPrefBestTimeNoasMod = "BestTimeNoasMod";

    public const string PlayerPrefMostEarned = "MostEarned";
    public const string PlayerPrefMostServed = "MostServed";
    public const string PlayerPrefMostMilk = "MostMilk";

    public const string PlayerPrefTextureQuality = "TextureQuality";
    public const string PlayerPrefVsync = "Vsync";
    public const string PlayerPrefAntiAlaising = "AntiAliasing";
    public const string PlayerPrefAutoFixClothes = "AutoFixClothes";
    public const string PlayerPrefFullscreen = "Fullscreen";
    public const string PlayerPrefResolution = "Resolution";

    public const string PlayerPrefSawAvatarAdventurer = "SawAvatarAdventurer";
    public const string PlayerPrefSawAvatarCat = "SawAvatarCat";
    public const string PlayerPrefSawAvatarFairy = "SawAvatarFairy";
    public const string PlayerPrefSawAvatarHero = "SawAvatarAdventurer";
    public const string PlayerPrefSawAvatarKnight = "SawAvatarKnight";

    //Stats
    public const string PlayerPrefMilkProducedOverall = "OverallMilkProduced";
    public const string PlayerPrefTimePlayedOverall = "OverallTimePlayed";
    public const string PlayerPrefCupsSoldOverall = "OverallCupsSold";
    public const string PlayerPrefCustomersOverall = "OverallCustomers";
    public const string PlayerPrefMoneyEarnedOverall = "OverallMoneyEarned";

    //Etc Prefs
    public const string PlayerPrefRandomSeed = "RandomSeed";
    public const string PlayerPrefCurrentSeed = "CurrentSeed";
    public const string PlayerPrefSceneWon = "SceneWon_";

    //Achievement (fixed spelling)
    public const string AchievementAvatarAdventurer = "Avatar Adventurer";
    public const string AchievementAvatarCat = "Avatar Cat";
    public const string AchievementAvatarFairy = "Avatar Fairy";
    public const string AchievementAvatarHero = "Avatar Hero";
    public const string AchievementAvatarKnight = "Avatar Knight";

    #endregion

    public const string AudioVolumeMusic = "Volume_Music";
    public const string AudioVolumeTalk = "Volume_Talking";
    public const string AudioVolumeEffects = "Volume_Effects";

    public const string ARGSandboxUnlocked = "-SandboxUnlocked";

    /// <summary>
    /// Ask if there are commandline arguments
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name)
            {
                return args[i + 1];
            }
        }
        return null;
    }

    #region Open-Cafe specific (preserved from open-source variant)

    // MilkTypeController preset key (open-source milk-type refactor)
    public const string PlayerPrefMilkPreset = "MilkPreset";

    // Legacy (misspelled) avatar achievement keys kept for backwards compatibility with
    // the open-source Archievements system. Same values as the AchievementAvatar* constants.
    public const string ArchievementAvatarAdventurer = "Avatar Adventurer";
    public const string ArchievementAvatarCat = "Avatar Cat";
    public const string ArchievementAvatarFairy = "Avatar Fairy";
    public const string ArchievementAvatarHero = "Avatar Hero";
    public const string ArchievementAvatarKnight = "Avatar Knight";

    #endregion
}