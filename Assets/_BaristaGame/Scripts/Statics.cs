/// <summary>
/// Static utility class containing shared methods, constants, and helper functions for the Unity Barista Game.
/// Optimized for Unity 6.1 with Burst compilation where applicable.
/// Contains RNG management, time utilities, dialogue helpers, and common operations.
/// </summary>
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Object = UnityEngine.Object;

public static class Statics
{
    private static uint ranSeed = 0;

    #region Display Strings

    public static string ml = "ml";
    public static string CurrencySymbol = "$";
    public static string LevelPreText = "Lv. ";

    #region Animation Triggers

    public static string AnimationAddMoneyTrigger = "AddMoney";
    public static string AnimationSubMoneyTrigger = "SubMoney";
    public static string CupFadeAwayTrigger = "FinishDrink";
    public static string CaveVisualsStatsLightning = "Stats Lightning";
    public static string CustomerDialogMiling = "Milking";
    public static string CustomerDialogFlusterIncrease = "Fluster";

    #endregion

    #region Ingredient Names

    public static string Espresso = "None";
    public static string Coffee = "None";
    public static string Chocolate = "None";
    public static string Tea = "None";
    public static string Milk = "None";
    public static string BreastMilk = "None";
    public static string Cream = "None";
    public static string Sugar = "None";
    public static string Ice = "None";
    public static string Boba = "None";

    public static string WhippedCream = "Whipped Cream";
    public static string CaramelSauce = "Caramel Sauce";
    public static string ChocolateSauce = "Cocoa Powder";
    public static string Sprinkles = "Candies";

    #endregion

    #region Customer Dialogs

    public static string[] CustomerDialogDrinkWithNoIngredient = { "None" };

    public static string CustomerDialogDrinkTypeCoffee = "None";
    public static string CustomerDialogDrinkTypeMilk = "None";
    public static string CustomerDialogDrinkTypeEspresso = "None";
    public static string CustomerDialogDrinkTypeTea = "None";
    public static string CustomerDialogDrinkTypeCream = "None";
    public static string CustomerDialogDrinkTypeChocolate = "None";

    public static string CustomerDialogNoSecondIngreedient = "Regular ";

    public static string CustomerDialogDrinkModiferEspresso = "None ";
    public static string CustomerDialogDrinkModiferCoffee = "None ";
    public static string CustomerDialogDrinkModiferChocolate = "None ";
    public static string CustomerDialogDrinkModiferTea = "None ";
    public static string CustomerDialogDrinkModiferMilk = "None ";
    public static string CustomerDialogDrinkModiferBreastMilk = "None ";
    public static string CustomerDialogDrinkModiferCream = "None ";
    public static string CustomerDialogDrinkModiferSugar = "None ";
    public static string CustomerDialogDrinkModiferIce = "None ";
    public static string CustomerDialogDrinkModiferBoba = "None ";

    public static string CustomerDialogDrinkModiferWhippedCream = "None ";
    public static string CustomerDialogDrinkModiferCaramelSauce = "None ";
    public static string CustomerDialogDrinkModiferChocolateSauce = "None ";
    public static string CustomerDialogDrinkModiferSprinkles = "None ";

    public static string[] CustomerDialogStartGreetings = { "None" };
    public static string[] CustomerDialogStartPre = { "None" };
    public static string CustomerDialogStartSeperator = ", ";
    public static string CustomerDialogStartEnd = " .";

    public static string[] CustomerDialogSucces = new string[] { "None" };
    public static string[] CustomerDialogFailed = new string[] { "None" };

    public static string TooltipFlusteredNormal = "Feeling Normal";
    public static string TooltipFlusteredLevel1 = "Attention Grabbed";
    public static string TooltipFlusteredLevel2 = "A Bit Flustered";
    public static string TooltipFlusteredLevel3 = "Very Flustered";
    public static string TooltipFlusteredLevel4 = "Super Into It!";

    #endregion

    #region Barista Dialogs

    public static DialogSentence[] BaristaTalk_StartGame_Arcade = { };
    public static DialogSentence[] BaristaTalk_Cookie = { };
    public static DialogSentence[] BaristaTalk_CookieBuyed = { };
    public static DialogSentence[] BaristaTalk_ApronLimit = { };
    public static DialogSentence[] BaristaTalk_TooFull = { };
    public static DialogSentence[] BaristaTalk_BuyUpgrade = { };

    public static DialogSentence[] BaristaTalk_UpgradeBuyed = {
        new DialogSentence("Oh!  Thank you!~", "Spoken_Ah"),
        new DialogSentence("I love this, thank you!~", "Spoken_Ah"),
        new DialogSentence("Just what I needed!", "Spoken_Ah"),
        new DialogSentence("Wonderful!~", "Spoken_Ah"),
        new DialogSentence("Keep it coming~", "Spoken_Ah"),
        new DialogSentence("More please~", "Spoken_Ah"),
        new DialogSentence("What a great addition! Thank you!", "Spoken_Ah"),
        new DialogSentence("This upgrade is a game-changer! Thanks!", "Spoken_Ah"),
        new DialogSentence("I can already feel the benefits of this upgrade!", "Spoken_Ah"),
        new DialogSentence("Our cafe just got even better with this upgrade!", "Spoken_Ah"),
        new DialogSentence("I appreciate the support! More upgrades, please!", "Spoken_Ah"),
        new DialogSentence("Thanks for investing in our success!", "Spoken_Ah"),
        new DialogSentence("One step closer to cafe perfection! Thank you!", "Spoken_Ah"),
        new DialogSentence("The future looks brighter with this upgrade!", "Spoken_Ah"),
        new DialogSentence("Fantastic choice! Let's keep improving!", "Spoken_Ah"),
        new DialogSentence("I'm thrilled with this upgrade! More, please!", "Spoken_Ah"),
        new DialogSentence("The cafe just leveled up! Thanks for the upgrade!", "Spoken_Ah"),
        new DialogSentence("I'm loving the upgrades! Keep 'em coming!", "Spoken_Ah"),
        new DialogSentence("Upgrade acquired! Let's make our cafe even better!", "Spoken_Ah"),
        new DialogSentence("Thanks for making our cafe even more awesome!", "Spoken_Ah"),
        new DialogSentence("This upgrade is like a breath of fresh air! Thank you!", "Spoken_Ah"),
        new DialogSentence("I'm feeling inspired by this upgrade! More, please!", "Spoken_Ah"),
        new DialogSentence("Kudos on the upgrade choice! Let's make magic happen!", "Spoken_Ah"),
        new DialogSentence("Our cafe just got a boost! Thanks for the upgrade!", "Spoken_Ah"),
        new DialogSentence("This upgrade is a real game-changer! Thank you!", "Spoken_Ah"),
        new DialogSentence("The upgrade train has left the station! Choo choo!", "Spoken_Ah"),
        new DialogSentence("Upgrade achieved! The cafe's future is looking bright!", "Spoken_Ah"),
        new DialogSentence("With this upgrade, the sky's the limit! Thank you!", "Spoken_Ah"),
        new DialogSentence("This upgrade is like music to my ears! Thanks a bunch!", "Spoken_Ah"),
        new DialogSentence("You've got a knack for picking upgrades! Thanks!", "Spoken_Ah"),
        new DialogSentence("I'm over the moon about this upgrade! Let's keep 'em coming!", "Spoken_Ah"),
        new DialogSentence("Upgrade unlocked! Our cafe's potential just went up!", "Spoken_Ah"),
        new DialogSentence("I'm feeling energized by this upgrade! Let's do this!", "Spoken_Ah"),
        new DialogSentence("Upgrade approved! Thanks for investing in our success!", "Spoken_Ah"),
        new DialogSentence("This upgrade is exactly what we needed! Thank you!", "Spoken_Ah"),
        new DialogSentence("I've got a good feeling about this upgrade! Thanks!", "Spoken_Ah"),
        new DialogSentence("Thanks~", "Spoken_Ah")
    };

    public static DialogSentence[] BaristaTalk_WelcomingNewCustomer = { };
    public static DialogSentence[] BaristaTalk_BadEnd = { };

    public static DialogSentence[] BaristaTalk_Idle_Mood_20 = { };
    public static DialogSentence[] BaristaTalk_Idle_Mood_40 = { };
    public static DialogSentence[] BaristaTalk_Idle_Mood_60 = { };
    public static DialogSentence[] BaristaTalk_Idle_Mood_80 = { };
    public static DialogSentence[] BaristaTalk_Idle_Mood_100 = { };

    public static DialogSentence[] BaristaTalk_Idle_Bust_20 = { };
    public static DialogSentence[] BaristaTalk_Idle_Bust_50 = { };
    public static DialogSentence[] BaristaTalk_Idle_Bust_80 = { };
    public static DialogSentence[] BaristaTalk_Idle_Bust_100 = { };

    public static DialogSentence[] BaristaTalk_Idle_Money_Above_25 = { };
    public static DialogSentence[] BaristaTalk_Idle_Money_Above_100 = { };

    public static DialogSentence[] BaristaTalk_ResetCup = { };
    public static DialogSentence[] BaristaTalk_FinishCup = { };
    public static DialogSentence[] BaristaTalk_AddMilk = { };
    public static DialogSentence[] BaristaTalk_PatHead = { };

    #endregion

    #region UI Text Constants

    public static string ButtonMaxUpgrades = "Max";

    public static string TextBestTime = "Best Time";
    public static string TextNoRecord = "No Record";
    public static string TextTime = "Time";
    public static string TextNewRecord = "New Record";
    public static string TextActualTime = "Your Time";

    public static string TextMoneyEarned = "Money Earned";
    public static string TextMostEarned = "Most Earned";

    public static string TextCustomerServed = "Customers Served";
    public static string TextMostServed = "Most Served";

    public static string TextMilkCreated = "Milk Created";
    public static string TextMostMilk = "Most Milk";

    public static string TextOverallMilkProduced = "Milk Produced: ";
    public static string TextCupsSold = "Cups Sold: ";
    public static string TextTimePlayed = "Time Played: ";
    public static string TextEarnedMoney = "Money Earned: ";

    #endregion

    #endregion

    #region Color Constants

    public static Color MilkColor_Thick = new Color(0.9433962f, 0.8751631f, 0.8410466f);
    public static Color MilkColor_Creamy = new Color(0.9716981f, 0.884612f, 0.9268206f);
    public static Color MilkColor_Chocolate = new Color(0.4716981f, 0.1941721f, 0.06452475f);
    public static Color MilkColor_Blue = new Color(0.2971698f, 0.8429658f, 1f);
    public static Color MilkColor_Green = new Color(0.5585459f, 1f, 0.3915094f);
    public static Color MilkColor_Raspberry = new Color(1.0f, 0.375f, 0.7f);
    public static Color MilkColor_Void = Color.black;

    #endregion

    #region Core Utility Methods

    /// <summary>
    /// Performs garbage collection and unloads unused Unity resources
    /// </summary>
    public static void CleanUpGabarge()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// Safely gets a random element from an array with error handling
    /// </summary>
    /// <typeparam name="T">Type of array elements</typeparam>
    /// <param name="fromArray">Source array</param>
    /// <returns>Random element or default if array is null/empty</returns>
    [BurstCompile]
    public static T GetRandomFromArray<T>(T[] fromArray)
    {
        if (IsArrayNullOrEmpty(fromArray))
        {
            LogErrorSafe("Array is empty! Check what you do!");
            return default(T);
        }

        if (fromArray.Length == 1)
        {
            return fromArray[0];
        }

        int randomIndex = GetRandomRange(0, fromArray.Length - 1);
        return fromArray[randomIndex];
    }

    /// <summary>
    /// Shuffles a list using the Fisher-Yates algorithm with main RNG
    /// </summary>
    /// <typeparam name="T">Type of list elements</typeparam>
    /// <param name="array">List to shuffle</param>
    [BurstCompile]
    public static void Shuffle<T>(this IList<T> array)
    {
        Shuffle(array, MainRNG());
    }

    /// <summary>
    /// Shuffles a list using the Fisher-Yates algorithm with specified RNG
    /// </summary>
    /// <typeparam name="T">Type of list elements</typeparam>
    /// <param name="array">List to shuffle</param>
    /// <param name="RNG">Random number generator to use</param>
    [BurstCompile]
    public static void Shuffle<T>(this IList<T> array, System.Random RNG)
    {
        if (IsListNullOrEmpty(array))
        {
            return;
        }

        for (int i = array.Count - 1; i > 0; i--)
        {
            int j = RNG.Next(i + 1);
            (array[j], array[i]) = (array[i], array[j]); // Tuple swap for cleaner code
        }
    }

    #endregion

    #region Random Number Generation

    private static System.Random mainRNG;
    private static System.Random drinkRNG;
    private static System.Random specificDrinkRNG;
    private static System.Random eventTypeRNG;
    private static System.Random eventDurationRNG;
    private static System.Random eventGapRNG;
    private static System.Random milkyRNG;

    /// <summary>
    /// Gets the main RNG instance, creating it if necessary
    /// </summary>
    /// <returns>Main Random number generator</returns>
    [BurstCompile]
    public static System.Random MainRNG()
    {
        mainRNG ??= new System.Random();
        return mainRNG;
    }

    /// <summary>
    /// Gets the drink-specific RNG instance, creating it if necessary
    /// </summary>
    /// <returns>Drink Random number generator</returns>
    [BurstCompile]
    public static System.Random DrinkRNG()
    {
        drinkRNG ??= new System.Random();
        return drinkRNG;
    }

    /// <summary>
    /// Gets the specific drink RNG instance, creating it if necessary
    /// </summary>
    /// <returns>Specific drink Random number generator</returns>
    [BurstCompile]
    public static System.Random SpecificDrinkRNG()
    {
        specificDrinkRNG ??= new System.Random();
        return specificDrinkRNG;
    }

    /// <summary>
    /// Gets the event type RNG instance, creating it if necessary
    /// </summary>
    /// <returns>Event type Random number generator</returns>
    [BurstCompile]
    public static System.Random EventTypeRNG()
    {
        eventTypeRNG ??= new System.Random();
        return eventTypeRNG;
    }

    /// <summary>
    /// Gets the event duration RNG instance, creating it if necessary
    /// </summary>
    /// <returns>Event duration Random number generator</returns>
    [BurstCompile]
    public static System.Random EventDurationRNG()
    {
        eventDurationRNG ??= new System.Random();
        return eventDurationRNG;
    }

    /// <summary>
    /// Gets the event gap RNG instance, creating it if necessary
    /// </summary>
    /// <returns>Event gap Random number generator</returns>
    [BurstCompile]
    public static System.Random EventGapRNG()
    {
        eventGapRNG ??= new System.Random();
        return eventGapRNG;
    }

    /// <summary>
    /// Gets the milky mode RNG instance, creating it if necessary
    /// </summary>
    /// <returns>Milky mode Random number generator</returns>
    [BurstCompile]
    public static System.Random MilkyRNG()
    {
        milkyRNG ??= new System.Random();
        return milkyRNG;
    }

    /// <summary>
    /// Seeds all mechanical RNG instances with a master seed
    /// </summary>
    /// <param name="seed">Master seed value</param>
    [BurstCompile]
    public static void SeedMechanicalRNG(Int32 seed)
    {
        drinkRNG = new System.Random(seed);
        eventDurationRNG = new System.Random(drinkRNG.Next());
        eventTypeRNG = new System.Random(drinkRNG.Next());
        eventGapRNG = new System.Random(drinkRNG.Next());
        milkyRNG = new System.Random(drinkRNG.Next());
    }

    /// <summary>
    /// Seeds the specific drink RNG from the main drink RNG
    /// </summary>
    [BurstCompile]
    public static void SeedSpecificDrinkRNG()
    {
        specificDrinkRNG = new System.Random(DrinkRNG().Next());
    }

    /// <summary>
    /// Generates a random boolean value
    /// </summary>
    /// <returns>Random true or false</returns>
    [BurstCompile]
    public static bool RandomBool()
    {
        return MainRNG().NextDouble() < 0.5;
    }

    /// <summary>
    /// Generates a random integer within the specified range (inclusive)
    /// </summary>
    /// <param name="minValue">Minimum value (inclusive)</param>
    /// <param name="maxValue">Maximum value (inclusive)</param>
    /// <returns>Random integer in range</returns>
    [BurstCompile]
    public static int GetRandomRange(int minValue, int maxValue)
    {
        return GetRandomRange(minValue, maxValue, MainRNG());
    }

    /// <summary>
    /// Generates a random integer within the specified range using a specific RNG
    /// </summary>
    /// <param name="minValue">Minimum value (inclusive)</param>
    /// <param name="maxValue">Maximum value (inclusive)</param>
    /// <param name="RNG">Random number generator to use</param>
    /// <returns>Random integer in range</returns>
    [BurstCompile]
    public static int GetRandomRange(int minValue, int maxValue, System.Random RNG)
    {
        if (minValue > maxValue)
        {
            (minValue, maxValue) = (maxValue, minValue); // Swap if order is wrong
        }

        int difference = maxValue - minValue;
        if (difference < int.MaxValue)
        {
            difference += 1;
        }

        return RNG.Next(difference) + minValue;
    }

    /// <summary>
    /// Generates a random float within the specified range
    /// </summary>
    /// <param name="minValue">Minimum value</param>
    /// <param name="maxValue">Maximum value</param>
    /// <returns>Random float in range</returns>
    [BurstCompile]
    public static float GetRandomRange(float minValue, float maxValue)
    {
        return GetRandomRange(minValue, maxValue, MainRNG());
    }

    /// <summary>
    /// Generates a random float within the specified range using a specific RNG
    /// </summary>
    /// <param name="minValue">Minimum value</param>
    /// <param name="maxValue">Maximum value</param>
    /// <param name="RNG">Random number generator to use</param>
    /// <returns>Random float in range</returns>
    [BurstCompile]
    public static float GetRandomRange(float minValue, float maxValue, System.Random RNG)
    {
        if (minValue > maxValue)
        {
            (minValue, maxValue) = (maxValue, minValue); // Swap if order is wrong
        }

        float difference = maxValue - minValue;
        return (float)RNG.NextDouble() * difference + minValue;
    }

    /// <summary>
    /// Safely converts a double to float with bounds checking
    /// </summary>
    /// <param name="value">Double value to convert</param>
    /// <returns>Float value within bounds</returns>
    [BurstCompile]
    public static float DoubleToFloat(double value)
    {
        return value switch
        {
            > float.MaxValue => float.MaxValue,
            < float.MinValue => float.MinValue,
            _ => (float)value
        };
    }

    #endregion

    #region Game Management

    /// <summary>
    /// Unlocks an item by ID and saves to PlayerPrefs
    /// </summary>
    /// <param name="UnlockId">Unique identifier for the item to unlock</param>
    public static void UnlockItem(string UnlockId)
    {
        if (string.IsNullOrEmpty(UnlockId))
        {
            LogErrorSafe("UnlockId cannot be null or empty");
            return;
        }

        PermanentUnlock[] unlockables = Object.FindObjectsOfType<PermanentUnlock>();
        if (!IsArrayNullOrEmpty(unlockables))
        {
            foreach (var unlockable in unlockables)
            {
                if (unlockable.UnlockId == UnlockId)
                {
                    unlockable.Unlock(true);
                    break;
                }
            }
        }

        PlayerPrefs.SetInt($"unlocked_{UnlockId}", 1);
        PlayerPrefs.Save();
    }

    #endregion

    #region Time Management Utilities

    /// <summary>
    /// Formats time as MM:SS string with proper padding
    /// </summary>
    /// <param name="time">Time in seconds</param>
    /// <returns>Formatted time string</returns>
    [BurstCompile]
    public static string FormatTimeAsString(float time)
    {
        int minutes = (int)time / Consts.TimeManagement.SecondsPerMinute;
        int seconds = (int)time % Consts.TimeManagement.SecondsPerMinute;
        return $"{minutes}{Consts.TimeManagement.TimeFormatSeparator}{seconds:00}";
    }

    /// <summary>
    /// Determines the game mode based on scene name and milky mode status
    /// </summary>
    /// <param name="sceneName">Current scene name</param>
    /// <param name="isMilkyMode">Whether milky mode is enabled</param>
    /// <returns>Corresponding GameMode enum value</returns>
    [BurstCompile]
    public static GameMode DetermineGameMode(string sceneName, bool isMilkyMode)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            LogErrorSafe("Scene name is null or empty, defaulting to Normal mode");
            return GameMode.Normal;
        }

        var normalModeMap = new Dictionary<string, GameMode>()
        {
            { Consts.SceneNames.GameArcadeNormal, GameMode.Normal },
            { Consts.SceneNames.GameArcadeHard, GameMode.Hard },
            { Consts.SceneNames.GameArcadeCasual, GameMode.Casual },
            { Consts.SceneNames.GameArcadeChaos, GameMode.Chaos },
            { Consts.SceneNames.GameArcadeUltraChaos, GameMode.UltraChaos },
            { Consts.SceneNames.GameArcadeHoliday, GameMode.NoasMod }
        };

        var milkyModeMap = new Dictionary<string, GameMode>()
        {
            { Consts.SceneNames.GameArcadeNormal, GameMode.NormalMilky },
            { Consts.SceneNames.GameArcadeHard, GameMode.HardMilky },
            { Consts.SceneNames.GameArcadeCasual, GameMode.CasualMilky },
            { Consts.SceneNames.GameArcadeChaos, GameMode.ChaosMilky },
            { Consts.SceneNames.GameArcadeUltraChaos, GameMode.UltraChaosMilky }
        };

        var activeMap = isMilkyMode ? milkyModeMap : normalModeMap;

        if (activeMap.TryGetValue(sceneName, out GameMode mode))
        {
            return mode;
        }

        LogWarningSafe($"Unknown scene name: {sceneName}, defaulting to Normal mode");
        return GameMode.Normal;
    }

    /// <summary>
    /// Gets the PlayerPrefs key for best time based on game mode
    /// </summary>
    /// <param name="gameMode">The game mode</param>
    /// <returns>PlayerPrefs key string</returns>
    [BurstCompile]
    public static string GetBestTimeKey(GameMode gameMode)
    {
        return gameMode switch
        {
            GameMode.Normal => Consts.PlayerPrefBestTimeNormal,
            GameMode.Hard => Consts.PlayerPrefBestTimeHard,
            GameMode.Casual => Consts.PlayerPrefBestTimeCasual,
            GameMode.Chaos => Consts.PlayerPrefBestTimeChaos,
            GameMode.UltraChaos => Consts.PlayerPrefBestTimeUltraChaos,
            GameMode.NoasMod => Consts.PlayerPrefBestTimeNoasMod,
            GameMode.NormalMilky => Consts.PlayerPrefBestTimeNormal + Consts.PlayerPrefBestTimeMilkymodeSuffix,
            GameMode.HardMilky => Consts.PlayerPrefBestTimeHard + Consts.PlayerPrefBestTimeMilkymodeSuffix,
            GameMode.CasualMilky => Consts.PlayerPrefBestTimeCasual + Consts.PlayerPrefBestTimeMilkymodeSuffix,
            GameMode.ChaosMilky => Consts.PlayerPrefBestTimeChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix,
            GameMode.UltraChaosMilky => Consts.PlayerPrefBestTimeUltraChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix,
            _ => Consts.PlayerPrefBestTimeNormal
        };
    }

    /// <summary>
    /// Safely loads best time from PlayerPrefs with error handling
    /// </summary>
    /// <param name="gameMode">The game mode</param>
    /// <returns>Best time or default value</returns>
    [BurstCompile]
    public static float LoadBestTime(GameMode gameMode)
    {
        string key = GetBestTimeKey(gameMode);
        return PlayerPrefs.GetFloat(key, Consts.TimeManagement.DefaultBestTime);
    }

    /// <summary>
    /// Safely saves best time to PlayerPrefs with error handling
    /// </summary>
    /// <param name="gameMode">The game mode</param>
    /// <param name="time">Time to save</param>
    [BurstCompile]
    public static void SaveBestTime(GameMode gameMode, float time)
    {
        if (time < 0)
        {
            LogWarningSafe($"Attempted to save negative time: {time}, skipping save");
            return;
        }

        string key = GetBestTimeKey(gameMode);
        PlayerPrefs.SetFloat(key, time);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Checks if a new time is a new best time record
    /// </summary>
    /// <param name="currentTime">Current time achieved</param>
    /// <param name="bestTime">Current best time</param>
    /// <returns>True if current time is a new record</returns>
    [BurstCompile]
    public static bool IsNewBestTime(float currentTime, float bestTime)
    {
        return currentTime > 0 && currentTime < bestTime && bestTime < Consts.TimeManagement.DefaultBestTime;
    }

    /// <summary>
    /// Checks if enough time has passed between events
    /// </summary>
    /// <param name="currentTime">Current game time</param>
    /// <param name="lastEventTime">Time of last event</param>
    /// <param name="requiredInterval">Required time interval</param>
    /// <returns>True if enough time has passed</returns>
    [BurstCompile]
    public static bool HasTimePassed(float currentTime, float lastEventTime, float requiredInterval)
    {
        return currentTime > (lastEventTime + requiredInterval);
    }

    #endregion

    #region Game Mode Utilities

    /// <summary>
    /// Calculates the next upgrade time for milky mode with random variation
    /// </summary>
    /// <param name="currentTime">Current game time</param>
    /// <param name="minUpgradeTime">Minimum time until next upgrade</param>
    /// <param name="maxUpgradeTime">Maximum time until next upgrade</param>
    /// <returns>Next upgrade time</returns>
    [BurstCompile]
    public static float CalculateNextUpgradeTime(float currentTime, float minUpgradeTime, float maxUpgradeTime)
    {
        return currentTime + GetRandomRange(minUpgradeTime, maxUpgradeTime, MilkyRNG());
    }

    /// <summary>
    /// Checks if it's time for the next upgrade based on current time
    /// </summary>
    /// <param name="currentTime">Current game time</param>
    /// <param name="nextUpgradeTime">Time when next upgrade should occur</param>
    /// <returns>True if it's time for upgrade</returns>
    [BurstCompile]
    public static bool IsTimeForNextUpgrade(float currentTime, float nextUpgradeTime)
    {
        return currentTime > nextUpgradeTime;
    }

    /// <summary>
    /// Loads a boolean preference from PlayerPrefs with default fallback
    /// </summary>
    /// <param name="key">PlayerPrefs key</param>
    /// <param name="defaultValue">Default value if key doesn't exist</param>
    /// <returns>Boolean value from PlayerPrefs</returns>
    [BurstCompile]
    public static bool LoadBoolPreference(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    /// <summary>
    /// Safely finds an object of type T with null checking and error handling
    /// </summary>
    /// <typeparam name="T">Type of object to find</typeparam>
    /// <returns>Found object or null if not found</returns>
    public static T FindObjectOfTypeSafe<T>() where T : Object
    {
        T result = Object.FindObjectOfType<T>();
        if (result == null)
        {
            LogWarningSafe($"{typeof(T).Name} not found in scene");
        }
        return result;
    }

    /// <summary>
    /// Determines if a chance-based event should trigger
    /// </summary>
    /// <param name="chancePercentage">Chance percentage (0-100)</param>
    /// <returns>True if event should trigger</returns>
    [BurstCompile]
    public static bool ShouldTriggerChanceEvent(float chancePercentage)
    {
        if (chancePercentage <= 0) return false;
        if (chancePercentage >= 100) return true;

        float randomValue = GetRandomRange(0f, 100f);
        return chancePercentage > randomValue;
    }

    #endregion

    #region Dialogue System Utilities

    /// <summary>
    /// Builds tooltip text from ingredient values list
    /// </summary>
    /// <param name="values">List of ingredient percentages/values</param>
    /// <returns>Formatted tooltip string</returns>
    [BurstCompile]
    public static string BuildTooltipTextFromValues(List<float> values)
    {
        if (IsListNullOrEmpty(values) || values.Count != Consts.DialogueSystem.ExpectedOrderValuesCount)
        {
            LogWarningSafe($"Invalid values list for tooltip generation. Expected {Consts.DialogueSystem.ExpectedOrderValuesCount} items");
            return string.Empty;
        }

        var tooltipBuilder = new StringBuilder();
        var threshold = Consts.DialogueSystem.MinimumIngredientThreshold;

        // Use ingredient indices from constants for clarity
        AddIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.BreastMilk], BreastMilk, threshold);
        AddIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.Chocolate], Chocolate, threshold);
        AddIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.Milk], Milk, threshold);
        AddIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.Tea], Tea, threshold);
        AddIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.Cream], Cream, threshold);
        AddIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.Espresso], Espresso, threshold);
        AddIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.Sugar], Sugar, threshold);
        AddIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.Coffee], Coffee, threshold);

        // Binary ingredients (no percentage)
        AddBinaryIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.Boba], Boba, threshold);
        AddBinaryIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.Ice], Ice, threshold);
        AddBinaryIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.WhippedCream], WhippedCream, threshold);
        AddBinaryIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.ChocolateSauce], ChocolateSauce, threshold);
        AddBinaryIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.CaramelSauce], CaramelSauce, threshold);
        AddBinaryIngredientToTooltip(tooltipBuilder, values[Consts.DialogueSystem.IngredientIndices.Sprinkles], Sprinkles, threshold);

        return tooltipBuilder.ToString();
    }

    /// <summary>
    /// Adds an ingredient with percentage to tooltip if above threshold
    /// </summary>
    /// <param name="builder">StringBuilder to append to</param>
    /// <param name="value">Ingredient value/percentage</param>
    /// <param name="ingredientName">Display name of ingredient</param>
    /// <param name="threshold">Minimum threshold to display</param>
    [BurstCompile]
    private static void AddIngredientToTooltip(StringBuilder builder, float value, string ingredientName, float threshold)
    {
        if (value > threshold)
        {
            builder.AppendLine($"{ingredientName}: {value:F1}%");
        }
    }

    /// <summary>
    /// Adds a binary ingredient (present/absent) to tooltip if above threshold
    /// </summary>
    /// <param name="builder">StringBuilder to append to</param>
    /// <param name="value">Ingredient value (0 or 1 typically)</param>
    /// <param name="ingredientName">Display name of ingredient</param>
    /// <param name="threshold">Minimum threshold to display</param>
    [BurstCompile]
    private static void AddBinaryIngredientToTooltip(StringBuilder builder, float value, string ingredientName, float threshold)
    {
        if (value > threshold)
        {
            builder.AppendLine(ingredientName);
        }
    }

    /// <summary>
    /// Gets the appropriate tooltip text for a flustered level
    /// </summary>
    /// <param name="flusteredLevel">Flustered level (-1 to 4)</param>
    /// <returns>Appropriate tooltip text</returns>
    [BurstCompile]
    public static string GetFlusteredTooltipText(int flusteredLevel)
    {
        return flusteredLevel switch
        {
            <= 0 => TooltipFlusteredNormal,
            1 => TooltipFlusteredLevel1,
            2 => TooltipFlusteredLevel2,
            3 => TooltipFlusteredLevel3,
            >= 4 => TooltipFlusteredLevel4,
        };
    }

    /// <summary>
    /// Creates a fallback dialogue from customer name and dialog array
    /// </summary>
    /// <param name="customerName">Customer name for dialogue</param>
    /// <param name="dialogArray">Array of possible dialog sentences</param>
    /// <returns>New Dialogue object</returns>
    [BurstCompile]
    public static Dialogue CreateFallbackDialogue(string customerName, string[] dialogArray)
    {
        if (IsArrayNullOrEmpty(dialogArray))
        {
            LogWarningSafe("Dialog array is null or empty for fallback dialogue");
            return new Dialogue(new[] { "..." }); // Minimal fallback
        }

        string randomDialog = GetRandomFromArray(dialogArray);
        var dialogue = new Dialogue(new[] { randomDialog });
        dialogue.name = string.IsNullOrWhiteSpace(customerName) ? string.Empty : customerName;

        return dialogue;
    }

    /// <summary>
    /// Processes a localized dialogue string and converts it to DialogSentence array
    /// </summary>
    /// <param name="localizedString">Localized string containing dialogue data</param>
    /// <returns>Array of DialogSentence objects</returns>
    public static DialogSentence[] ProcessLocalizedDialogueString(LocalizedString localizedString)
    {
        if (localizedString == null)
        {
            LogWarningSafe("LocalizedString is null for dialogue processing");
            return new DialogSentence[0];
        }

        try
        {
            // Get current language text
            var currentLanguageText = localizedString.GetLocalizedString();
            if (string.IsNullOrEmpty(currentLanguageText))
            {
                LogWarningSafe("Localized string returned empty text");
                return new DialogSentence[0];
            }

            // Get English fallback text
            var enLocale = LocalizationSettings.AvailableLocales?.GetLocale(Consts.BaristaTalk.Localization.EnglishLocaleCode);
            var englishText = string.Empty;

            if (enLocale != null)
            {
                englishText = LocalizationSettings.StringDatabase.GetLocalizedString(
                    localizedString.TableReference,
                    localizedString.TableEntryReference,
                    enLocale
                );
            }

            // Split and process dialogue segments
            var currentSegments = SplitAndCleanDialogueString(currentLanguageText);
            var englishSegments = SplitAndCleanDialogueString(englishText);

            // Create DialogSentence array
            return CreateDialogueSentenceArray(currentSegments, englishSegments);
        }
        catch (System.Exception ex)
        {
            LogErrorSafe($"Error processing localized dialogue string: {ex.Message}");
            return new DialogSentence[0];
        }
    }

    /// <summary>
    /// Splits and cleans a dialogue string into segments
    /// </summary>
    /// <param name="dialogueText">Raw dialogue text</param>
    /// <returns>Array of cleaned dialogue segments</returns>
    [BurstCompile]
    private static string[] SplitAndCleanDialogueString(string dialogueText)
    {
        if (string.IsNullOrEmpty(dialogueText))
        {
            return new string[0];
        }

        return dialogueText.Split(Consts.BaristaTalk.Localization.DialogueSeparator)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();
    }

    /// <summary>
    /// Creates DialogSentence array from current and English text segments
    /// </summary>
    /// <param name="currentSegments">Current language segments</param>
    /// <param name="englishSegments">English fallback segments</param>
    /// <returns>Array of DialogSentence objects</returns>
    [BurstCompile]
    private static DialogSentence[] CreateDialogueSentenceArray(string[] currentSegments, string[] englishSegments)
    {
        if (IsArrayNullOrEmpty(currentSegments))
        {
            return new DialogSentence[0];
        }

        var sentences = new List<DialogSentence>();

        for (int i = 0; i < currentSegments.Length; i++)
        {
            var currentText = currentSegments[i];
            var englishFallback = (englishSegments != null && i < englishSegments.Length)
                ? englishSegments[i]
                : currentText;

            // Extract audio clip name if present
            var textParts = currentText.Split(Consts.BaristaTalk.Localization.AudioSeparator);
            var displayText = textParts[0];
            var audioClipName = textParts.Length > 1 ? textParts[1] : englishFallback;

            sentences.Add(new DialogSentence(displayText, audioClipName));
        }

        return sentences.ToArray();
    }

    /// <summary>
    /// Updates a static dialogue array based on dialogue key
    /// </summary>
    /// <param name="dialogueKey">Key identifying which dialogue array to update</param>
    /// <param name="sentences">New dialogue sentences</param>
    public static void UpdateStaticDialogueArray(string dialogueKey, DialogSentence[] sentences)
    {
        if (string.IsNullOrEmpty(dialogueKey) || sentences == null)
        {
            LogWarningSafe($"Invalid parameters for updating dialogue array: {dialogueKey}");
            return;
        }

        switch (dialogueKey)
        {
            case Consts.BaristaTalk.DialogueKeys.StartGameArcade:
                BaristaTalk_StartGame_Arcade = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.Cookie:
                BaristaTalk_Cookie = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.CookieBuyed:
                BaristaTalk_CookieBuyed = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.ApronLimit:
                BaristaTalk_ApronLimit = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.TooFull:
                BaristaTalk_TooFull = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.BuyUpgrade:
                BaristaTalk_BuyUpgrade = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.WelcomingNewCustomer:
                BaristaTalk_WelcomingNewCustomer = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.BadEnd:
                BaristaTalk_BadEnd = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.FinishCup:
                BaristaTalk_FinishCup = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.ResetCup:
                BaristaTalk_ResetCup = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.AddMilk:
                BaristaTalk_AddMilk = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.PatHead:
                BaristaTalk_PatHead = sentences;
                break;

            // Mood dialogues
            case Consts.BaristaTalk.DialogueKeys.IdleMood20:
                BaristaTalk_Idle_Mood_20 = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.IdleMood40:
                BaristaTalk_Idle_Mood_40 = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.IdleMood60:
                BaristaTalk_Idle_Mood_60 = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.IdleMood80:
                BaristaTalk_Idle_Mood_80 = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.IdleMood100:
                BaristaTalk_Idle_Mood_100 = sentences;
                break;

            // Bust dialogues
            case Consts.BaristaTalk.DialogueKeys.IdleBust20:
                BaristaTalk_Idle_Bust_20 = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.IdleBust50:
                BaristaTalk_Idle_Bust_50 = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.IdleBust80:
                BaristaTalk_Idle_Bust_80 = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.IdleBust100:
                BaristaTalk_Idle_Bust_100 = sentences;
                break;

            // Money dialogues
            case Consts.BaristaTalk.DialogueKeys.MoneyAbove25:
                BaristaTalk_Idle_Money_Above_25 = sentences;
                break;
            case Consts.BaristaTalk.DialogueKeys.MoneyAbove100:
                BaristaTalk_Idle_Money_Above_100 = sentences;
                break;

            default:
                LogWarningSafe($"Unknown dialogue key: {dialogueKey}");
                break;
        }
    }

    #endregion

    #region Validation and Safety Utilities

    /// <summary>
    /// Checks if an array is null or empty
    /// </summary>
    /// <typeparam name="T">Type of array elements</typeparam>
    /// <param name="array">Array to check</param>
    /// <returns>True if null or empty</returns>
    [BurstCompile]
    public static bool IsArrayNullOrEmpty<T>(T[] array)
    {
        return array == null || array.Length == 0;
    }

    /// <summary>
    /// Checks if a list is null or empty
    /// </summary>
    /// <typeparam name="T">Type of list elements</typeparam>
    /// <param name="list">List to check</param>
    /// <returns>True if null or empty</returns>
    [BurstCompile]
    public static bool IsListNullOrEmpty<T>(IList<T> list)
    {
        return list == null || list.Count == 0;
    }

    /// <summary>
    /// Safely logs an error message if not null or empty
    /// </summary>
    /// <param name="message">Error message to log</param>
    [BurstCompile]
    public static void LogErrorSafe(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            Debug.LogError(message);
        }
    }

    /// <summary>
    /// Safely logs a warning message if not null or empty
    /// </summary>
    /// <param name="message">Warning message to log</param>
    [BurstCompile]
    public static void LogWarningSafe(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            Debug.LogWarning(message);
        }
    }

    /// <summary>
    /// Safely logs an info message if not null or empty
    /// </summary>
    /// <param name="message">Info message to log</param>
    [BurstCompile]
    public static void LogInfoSafe(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log(message);
        }
    }

    /// <summary>
    /// Safely destroys a GameObject with context-appropriate method
    /// </summary>
    /// <param name="obj">GameObject to destroy</param>
    public static void SafeDestroy(GameObject obj)
    {
        if (obj != null)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(obj);
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
        }
    }

    /// <summary>
    /// Safely gets a component from a GameObject with null checking
    /// </summary>
    /// <typeparam name="T">Type of component to get</typeparam>
    /// <param name="gameObject">GameObject to get component from</param>
    /// <returns>Component of type T or null if not found</returns>
    public static T GetComponentSafe<T>(GameObject gameObject) where T : Component
    {
        if (gameObject == null)
        {
            LogWarningSafe($"GameObject is null when trying to get component {typeof(T).Name}");
            return null;
        }

        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            LogWarningSafe($"Component {typeof(T).Name} not found on GameObject {gameObject.name}");
        }

        return component;
    }

    /// <summary>
    /// Safely clamps a value between min and max bounds
    /// </summary>
    /// <param name="value">Value to clamp</param>
    /// <param name="min">Minimum bound</param>
    /// <param name="max">Maximum bound</param>
    /// <returns>Clamped value</returns>
    [BurstCompile]
    public static float ClampSafe(float value, float min, float max)
    {
        if (min > max)
        {
            LogWarningSafe($"ClampSafe: min ({min}) is greater than max ({max}). Swapping values.");
            (min, max) = (max, min);
        }

        return Mathf.Clamp(value, min, max);
    }

    /// <summary>
    /// Safely clamps an integer value between min and max bounds
    /// </summary>
    /// <param name="value">Value to clamp</param>
    /// <param name="min">Minimum bound</param>
    /// <param name="max">Maximum bound</param>
    /// <returns>Clamped value</returns>
    [BurstCompile]
    public static int ClampSafe(int value, int min, int max)
    {
        if (min > max)
        {
            LogWarningSafe($"ClampSafe: min ({min}) is greater than max ({max}). Swapping values.");
            (min, max) = (max, min);
        }

        return Mathf.Clamp(value, min, max);
    }

    #endregion

    #region Performance Utilities

    /// <summary>
    /// Checks if a flustered level is within valid range
    /// </summary>
    /// <param name="level">Flustered level to validate</param>
    /// <returns>True if level is valid</returns>
    [BurstCompile]
    public static bool IsValidFlusteredLevel(int level)
    {
        return level >= Consts.DialogueSystem.FlusteredLevels.MinLevel &&
               level <= Consts.DialogueSystem.FlusteredLevels.MaxLevel;
    }

    /// <summary>
    /// Normalizes a flustered level to valid range
    /// </summary>
    /// <param name="level">Level to normalize</param>
    /// <returns>Normalized flustered level</returns>
    [BurstCompile]
    public static int NormalizeFlusteredLevel(int level)
    {
        return ClampSafe(level,
            Consts.DialogueSystem.FlusteredLevels.MinLevel,
            Consts.DialogueSystem.FlusteredLevels.MaxLevel);
    }

    /// <summary>
    /// Converts a percentage to a display string with formatting
    /// </summary>
    /// <param name="percentage">Percentage value (0-100)</param>
    /// <param name="decimalPlaces">Number of decimal places</param>
    /// <returns>Formatted percentage string</returns>
    [BurstCompile]
    public static string FormatPercentage(float percentage, int decimalPlaces = 1)
    {
        string format = decimalPlaces > 0 ? $"F{decimalPlaces}" : "F0";
        return $"{percentage.ToString(format)}%";
    }

    #endregion

    #region String / Array Helpers (Added)
    /// <summary>
    /// Splits a delimited string into a trimmed, non-empty string array.
    /// Default separator is ';'. Returns empty array if input is null/empty.
    /// </summary>
    /// <param name="value">Raw string value containing separated entries.</param>
    /// <param name="separator">Separator character (default ';').</param>
    [BurstCompile]
    public static string[] SplitToCleanArray(string value, char separator = ';')
    {
        if (string.IsNullOrEmpty(value))
        {
            return Array.Empty<string>();
        }
        return value.Split(separator)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();
    }
    #endregion
}