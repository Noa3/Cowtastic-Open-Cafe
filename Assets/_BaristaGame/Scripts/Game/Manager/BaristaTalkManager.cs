using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/// <summary>
/// Manages barista dialogue events and timing, integrating with the localization system.
/// Handles various dialogue triggers including idle chat, reactions to player actions, and mood-based conversations.
/// Optimized for Unity 6.1 with Burst compilation where applicable.
/// 
/// Usage:
/// - Automatically manages dialogue timing and event triggers
/// - Responds to game state changes (happiness, bust size, money)
/// - Integrates with localization system for multi-language support
/// </summary>
public class BaristaTalkManager : MonoBehaviour
{
    #region Localized Strings
    [Header("Localized Strings")]

    [Space]
    [Header("Barista Talk Arrays")]
    public LocalizedString StringBaristaTalkStartGameArcade;
    public LocalizedString StringBaristaTalkCookie;
    public LocalizedString StringBaristaTalkCookieBuyed;
    public LocalizedString StringBaristaTalkApronLimit;
    public LocalizedString StringBaristaTalkTooFull;
    public LocalizedString StringBaristaTalkBuyUpgrade;
    public LocalizedString StringBaristaTalkWelcomeNewCustomer;
    public LocalizedString StringBaristaTalkBadEnd;
    public LocalizedString StringBaristaTalkFinishCup;
    public LocalizedString StringBaristaTalk_ResetCup;
    public LocalizedString StringBaristaTalk_AddMilk;
    public LocalizedString StringBaristaTalk_PatHead;

    [Space]
    [Header("Idle Mood")]
    public LocalizedString StringBaristaTalk_Idle_Mood_20;
    public LocalizedString StringBaristaTalk_Idle_Mood_40;
    public LocalizedString StringBaristaTalk_Idle_Mood_60;
    public LocalizedString StringBaristaTalk_Idle_Mood_80;
    public LocalizedString StringBaristaTalk_Idle_Mood_100;

    [Space]
    [Header("Idle Bust")]
    public LocalizedString StringBaristaTalk_Idle_Bust_20;
    public LocalizedString StringBaristaTalk_Idle_Bust_50;
    public LocalizedString StringBaristaTalk_Idle_Bust_80;
    public LocalizedString StringBaristaTalk_Idle_Bust_100;

    [Space]
    [Header("Idle Money Above")]
    public LocalizedString BaristaTalk_Idle_Money_Above_25;
    public LocalizedString BaristaTalk_Idle_Money_Above_100;
    #endregion

    [Space(5)]
    [Header("References")]
    public DialogueManager DialogueManager;
    public SoundEffectVariation SoundVariationAngry;
    public SoundEffectVariation SoundVariationSigh;
    public SoundEffectVariation SoundVariationThinking;

    [Header("Unified Dialogue System")]
    [Tooltip("Enable the greeting message at game start")]
    public bool EventGreeting = true;
    [Tooltip("How much time needs to pass until the greet message appears")]
    public float EventGreetingOffsetStart = Consts.BaristaTalk.DefaultGreetOffsetStart;
    [Tooltip("Duration after greeting completion")]
    public float EventGreetingOffsetEnd = Consts.BaristaTalk.DefaultGreetOffsetEnd;
    
    [Space]
    [Header("Main Dialogue Timing")]
    [Tooltip("Minimum time between any dialogue events")]
    public float EventDialogueMinTime = Consts.BaristaTalk.DefaultIdleMinTime;
    [Tooltip("Maximum time between any dialogue events")]
    public float EventDialogueMaxTime = Consts.BaristaTalk.DefaultIdleMaxTime;
    [Tooltip("How long to wait after dialogue ends before next can start")]
    public float EventDialogueOffsetEnd = Consts.BaristaTalk.DefaultEventOffsetEnd;

    [Space]
    [Header("Event Toggles")]
    [Tooltip("Allow cookie-related dialogue when upgrades are locked")]
    public bool AllowCookieDialogue = true;
    [Tooltip("Allow apron dialogue when bust reaches limit")]
    public bool AllowApronDialogue = true;
    [Range(Consts.BaristaTalk.ApronLimitMin, Consts.BaristaTalk.ApronLimitMax)]
    public float EventApronLimit = Consts.BaristaTalk.DefaultApronLimit;
    [Tooltip("Allow too full dialogue")]
    public bool AllowTooFullDialogue = true;
    public float EventTooFullLimit = Consts.BaristaTalk.DefaultTooFullLimit;
    [Tooltip("Allow upgrade purchase dialogue")]
    public bool AllowUpgradeDialogue = true;
    [Range(Consts.BaristaTalk.TalkChanceMin, Consts.BaristaTalk.TalkChanceMax)]
    public float EventUpgradeTalkChance = Consts.BaristaTalk.DefaultBuyUpgradeTalkChance;

    [Space]
    [Header("Action-Based Events")]
    [Tooltip("Allow dialogue when cup is reset")]
    public bool AllowResetCupDialogue = true;
    [Tooltip("Allow dialogue when head is patted")]
    public bool AllowPatHeadDialogue = true;
    [Tooltip("Allow dialogue when breast milk is added")]
    public bool AllowBreastMilkDialogue = true;
    [Tooltip("Minimum time between breast milk dialogue")]
    public int EventBreastMilkAddMin = Consts.BaristaTalk.DefaultBreastMilkAddMin;
    [Tooltip("Maximum time between breast milk dialogue")]
    public int EventBreastMilkAddMax = Consts.BaristaTalk.DefaultBreastMilkAddMax;

    [Header("Runtime State")]
    [ReadOnly]
    public float TimeLastDialogue = 0;
    [ReadOnly]
    private float TimeScinceLevelLoad = 0;
    [ReadOnly]
    private float NextDialogueTime = 0;

    // Private state
    private bool hasGreeted = false;
    private bool EventAppronDone = false;
    private BaseGameMode gameMode;
    private OrderManager orderManager;
    private float EventBreastMilkAddCanSpeakAgain = 0;

    public static BaristaTalkManager instance;

    #region Unity Lifecycle

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeLocalizedStrings();
        InitializeReferences();
        
        if (EventGreeting)
        {
            DoBaristaEventStartGame();
        }
        else
        {
            hasGreeted = true;
            SetNextDialogueTime();
        }
    }

    public void FixedUpdate()
    {
        TimeScinceLevelLoad = Time.timeSinceLevelLoad;

        if (!ValidateGameMode()) return;

        ProcessUnifiedDialogueSystem();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes localized string subscriptions and updates static dialogue arrays
    /// </summary>
    private void InitializeLocalizedStrings()
    {
        SubscribeToLocalizedStringChanges();
        UpdateAllStaticDialogues();
    }

    /// <summary>
    /// Initializes component references with fallback safety
    /// </summary>
    private void InitializeReferences()
    {
        if (DialogueManager == null)
        {
            DialogueManager = Statics.FindObjectOfTypeSafe<DialogueManager>();
        }

        gameMode = Statics.FindObjectOfTypeSafe<BaseGameMode>();
        orderManager = Statics.FindObjectOfTypeSafe<OrderManager>();
    }

    /// <summary>
    /// Subscribes to localized string change events
    /// </summary>
    private void SubscribeToLocalizedStringChanges()
    {
        // Main dialogue events
        StringBaristaTalkStartGameArcade.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalkStartGameArcade, Consts.BaristaTalk.DialogueKeys.StartGameArcade);
        StringBaristaTalkCookie.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalkCookie, Consts.BaristaTalk.DialogueKeys.Cookie);
        StringBaristaTalkCookieBuyed.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalkCookieBuyed, Consts.BaristaTalk.DialogueKeys.CookieBuyed);
        StringBaristaTalkApronLimit.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalkApronLimit, Consts.BaristaTalk.DialogueKeys.ApronLimit);
        StringBaristaTalkTooFull.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalkTooFull, Consts.BaristaTalk.DialogueKeys.TooFull);
        StringBaristaTalkBuyUpgrade.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalkBuyUpgrade, Consts.BaristaTalk.DialogueKeys.BuyUpgrade);
        StringBaristaTalkWelcomeNewCustomer.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalkWelcomeNewCustomer, Consts.BaristaTalk.DialogueKeys.WelcomingNewCustomer);
        StringBaristaTalkBadEnd.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalkBadEnd, Consts.BaristaTalk.DialogueKeys.BadEnd);
        StringBaristaTalkFinishCup.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalkFinishCup, Consts.BaristaTalk.DialogueKeys.FinishCup);

        // Action dialogues
        StringBaristaTalk_ResetCup.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_ResetCup, Consts.BaristaTalk.DialogueKeys.ResetCup);
        StringBaristaTalk_AddMilk.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_AddMilk, Consts.BaristaTalk.DialogueKeys.AddMilk);
        StringBaristaTalk_PatHead.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_PatHead, Consts.BaristaTalk.DialogueKeys.PatHead);

        // Mood dialogues
        StringBaristaTalk_Idle_Mood_20.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_Idle_Mood_20, Consts.BaristaTalk.DialogueKeys.IdleMood20);
        StringBaristaTalk_Idle_Mood_40.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_Idle_Mood_40, Consts.BaristaTalk.DialogueKeys.IdleMood40);
        StringBaristaTalk_Idle_Mood_60.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_Idle_Mood_60, Consts.BaristaTalk.DialogueKeys.IdleMood60);
        StringBaristaTalk_Idle_Mood_80.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_Idle_Mood_80, Consts.BaristaTalk.DialogueKeys.IdleMood80);
        StringBaristaTalk_Idle_Mood_100.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_Idle_Mood_100, Consts.BaristaTalk.DialogueKeys.IdleMood100);

        // Bust dialogues
        StringBaristaTalk_Idle_Bust_20.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_Idle_Bust_20, Consts.BaristaTalk.DialogueKeys.IdleBust20);
        StringBaristaTalk_Idle_Bust_50.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_Idle_Bust_50, Consts.BaristaTalk.DialogueKeys.IdleBust50);
        StringBaristaTalk_Idle_Bust_80.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_Idle_Bust_80, Consts.BaristaTalk.DialogueKeys.IdleBust80);
        StringBaristaTalk_Idle_Bust_100.StringChanged += _ => UpdateBaristaTalk(StringBaristaTalk_Idle_Bust_100, Consts.BaristaTalk.DialogueKeys.IdleBust100);

        // Money dialogues
        BaristaTalk_Idle_Money_Above_25.StringChanged += _ => UpdateBaristaTalk(BaristaTalk_Idle_Money_Above_25, Consts.BaristaTalk.DialogueKeys.MoneyAbove25);
        BaristaTalk_Idle_Money_Above_100.StringChanged += _ => UpdateBaristaTalk(BaristaTalk_Idle_Money_Above_100, Consts.BaristaTalk.DialogueKeys.MoneyAbove100);
    }

    /// <summary>
    /// Updates all static dialogue arrays on initialization
    /// </summary>
    private void UpdateAllStaticDialogues()
    {
        UpdateBaristaTalk(StringBaristaTalkStartGameArcade, Consts.BaristaTalk.DialogueKeys.StartGameArcade);
        UpdateBaristaTalk(StringBaristaTalkCookie, Consts.BaristaTalk.DialogueKeys.Cookie);
        UpdateBaristaTalk(StringBaristaTalkCookieBuyed, Consts.BaristaTalk.DialogueKeys.CookieBuyed);
        UpdateBaristaTalk(StringBaristaTalkApronLimit, Consts.BaristaTalk.DialogueKeys.ApronLimit);
        UpdateBaristaTalk(StringBaristaTalkTooFull, Consts.BaristaTalk.DialogueKeys.TooFull);
        UpdateBaristaTalk(StringBaristaTalkBuyUpgrade, Consts.BaristaTalk.DialogueKeys.BuyUpgrade);
        UpdateBaristaTalk(StringBaristaTalkWelcomeNewCustomer, Consts.BaristaTalk.DialogueKeys.WelcomingNewCustomer);
        UpdateBaristaTalk(StringBaristaTalkBadEnd, Consts.BaristaTalk.DialogueKeys.BadEnd);
        UpdateBaristaTalk(StringBaristaTalkFinishCup, Consts.BaristaTalk.DialogueKeys.FinishCup);

        UpdateBaristaTalk(StringBaristaTalk_ResetCup, Consts.BaristaTalk.DialogueKeys.ResetCup);
        UpdateBaristaTalk(StringBaristaTalk_AddMilk, Consts.BaristaTalk.DialogueKeys.AddMilk);
        UpdateBaristaTalk(StringBaristaTalk_PatHead, Consts.BaristaTalk.DialogueKeys.PatHead);

        UpdateBaristaTalk(StringBaristaTalk_Idle_Mood_20, Consts.BaristaTalk.DialogueKeys.IdleMood20);
        UpdateBaristaTalk(StringBaristaTalk_Idle_Mood_40, Consts.BaristaTalk.DialogueKeys.IdleMood40);
        UpdateBaristaTalk(StringBaristaTalk_Idle_Mood_60, Consts.BaristaTalk.DialogueKeys.IdleMood60);
        UpdateBaristaTalk(StringBaristaTalk_Idle_Mood_80, Consts.BaristaTalk.DialogueKeys.IdleMood80);
        UpdateBaristaTalk(StringBaristaTalk_Idle_Mood_100, Consts.BaristaTalk.DialogueKeys.IdleMood100);

        UpdateBaristaTalk(StringBaristaTalk_Idle_Bust_20, Consts.BaristaTalk.DialogueKeys.IdleBust20);
        UpdateBaristaTalk(StringBaristaTalk_Idle_Bust_50, Consts.BaristaTalk.DialogueKeys.IdleBust50);
        UpdateBaristaTalk(StringBaristaTalk_Idle_Bust_80, Consts.BaristaTalk.DialogueKeys.IdleBust80);
        UpdateBaristaTalk(StringBaristaTalk_Idle_Bust_100, Consts.BaristaTalk.DialogueKeys.IdleBust100);

        UpdateBaristaTalk(BaristaTalk_Idle_Money_Above_25, Consts.BaristaTalk.DialogueKeys.MoneyAbove25);
        UpdateBaristaTalk(BaristaTalk_Idle_Money_Above_100, Consts.BaristaTalk.DialogueKeys.MoneyAbove100);
    }

    #endregion

    #region Localization Processing

    /// <summary>
    /// Processes localized string and updates corresponding static dialogue array
    /// </summary>
    /// <param name="localizedString">Source localized string</param>
    /// <param name="dialogueKey">Key identifier for the dialogue type</param>
    private void UpdateBaristaTalk(LocalizedString localizedString, string dialogueKey)
    {
        if (localizedString == null || string.IsNullOrEmpty(dialogueKey))
        {
            Statics.LogWarningSafe($"Invalid parameters for UpdateBaristaTalk: {dialogueKey}");
            return;
        }

        var dialogueSentences = Statics.ProcessLocalizedDialogueString(localizedString);
        Statics.UpdateStaticDialogueArray(dialogueKey, dialogueSentences);
    }

    #endregion

    #region Unified Dialogue System

    /// <summary>
    /// Main dialogue processing system that handles timing and event selection
    /// </summary>
    [BurstCompile]
    private void ProcessUnifiedDialogueSystem()
    {
        // Skip if greeting hasn't happened yet
        if (!hasGreeted) return;

        // Check if it's time for the next dialogue
        if (TimeScinceLevelLoad >= NextDialogueTime)
        {
            TriggerContextualDialogue();
        }
    }

    /// <summary>
    /// Selects and triggers the most appropriate dialogue based on current game state
    /// </summary>
    private void TriggerContextualDialogue()
    {
        Statics.LogInfoSafe("BaristaTalk: Selecting contextual dialogue");

        // Priority 1: Special state dialogues
        if (AllowApronDialogue && !EventAppronDone && gameMode.TargetBustSize >= EventApronLimit)
        {
            DoBaristaEventAppron();
            EventAppronDone = true;
            return;
        }

        if (AllowTooFullDialogue && gameMode.UpgradeCanGrow && gameMode.Fullness > EventTooFullLimit)
        {
            DoBaristaEventTooFull();
            return;
        }

        // Priority 2: Cookie dialogue when upgrades are locked
        if (AllowCookieDialogue && !gameMode.UpgradeCanGrow)
        {
            DoBaristaEventCookie();
            return;
        }

        // Priority 3: Random contextual dialogue based on game state
        DoBaristaEventIdle();
    }

    /// <summary>
    /// Sets the next dialogue time based on min/max range
    /// </summary>
    private void SetNextDialogueTime()
    {
        float randomTime = Statics.GetRandomRange(EventDialogueMinTime, EventDialogueMaxTime);
        NextDialogueTime = TimeScinceLevelLoad + randomTime;
        
        Statics.LogInfoSafe($"BaristaTalk: Next dialogue in {randomTime:F1} seconds");
    }

    /// <summary>
    /// Updates timing state after dialogue completion
    /// </summary>
    public void SetTimeForLastDialogue()
    {
        TimeLastDialogue = Time.timeSinceLevelLoad;
        SetNextDialogueTime();
    }

    #endregion

    #region Public Event Methods (Action-Based)

    /// <summary>
    /// Handles greeting at game start
    /// </summary>
    public void DoBaristaEventStartGame()
    {
        TryStartBaristaTalkFromArrays(
            Statics.BaristaTalk_StartGame_Arcade,
            null,
            "StartGameArcade",
            EventGreetingOffsetStart,
            EventGreetingOffsetEnd
        );
        hasGreeted = true;
    }

    /// <summary>
    /// Tries to trigger head pat dialogue if timing allows
    /// </summary>
    public void TryBaristaEventPatHead()
    {
        if (AllowPatHeadDialogue && CanTriggerActionDialogue())
        {
            Statics.LogInfoSafe("BaristaTalk Pat Head Event");
            DoBaristaEventPatHead();
        }
    }

    public void DoBaristaEventPatHead()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_PatHead, Statics.BaristaTalk_Idle_Mood_60, "PatHead", 0, EventDialogueOffsetEnd);
    }

    /// <summary>
    /// Tries to trigger upgrade dialogue with chance-based logic
    /// </summary>
    public void TryBaristaEventUpgrade()
    {
        if (AllowUpgradeDialogue && CanTriggerActionDialogue() && Statics.ShouldTriggerChanceEvent(EventUpgradeTalkChance))
        {
            Statics.LogInfoSafe("BaristaTalk Upgrade Event");
            DoBaristaEventUpgrade();
        }
    }

    public void DoBaristaEventUpgrade()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_BuyUpgrade, Statics.BaristaTalk_Idle_Mood_80, "BuyUpgrade", 0, EventDialogueOffsetEnd);
    }

    /// <summary>
    /// Handles cookie purchase dialogue
    /// </summary>
    public void TryBaristaEventCookieBuyed()
    {
        if (AllowCookieDialogue)
        {
            DoBaristaEventCookieBuyed();
        }
    }

    public void DoBaristaEventCookieBuyed()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_CookieBuyed, Statics.BaristaTalk_Idle_Mood_60, "CookieBuyed", 0, EventDialogueOffsetEnd);
    }

    /// <summary>
    /// Tries to trigger cup reset dialogue
    /// </summary>
    public void TryBaristaEventCupReset()
    {
        if (orderManager == null)
        {
            orderManager = OrderManager.instance;
        }

        if (AllowResetCupDialogue && orderManager?.orderIsActive == true && CanTriggerActionDialogue())
        {
            DoBaristaEventCupReset();
        }
    }

    public void DoBaristaEventCupReset()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_ResetCup, Statics.BaristaTalk_Idle_Mood_60, "ResetCup", 0, EventDialogueOffsetEnd);
        SoundVariationThinking?.PlayRandomOneShot();
    }

    /// <summary>
    /// Handles cup finished dialogue
    /// </summary>
    public void TryBaristaEventCupFinished()
    {
        if (CanTriggerActionDialogue())
        {
            DoBaristaEventCupFinished();
        }
    }

    public void DoBaristaEventCupFinished()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_FinishCup, Statics.BaristaTalk_Idle_Mood_80, "FinishCup", 0, EventDialogueOffsetEnd);
    }

    /// <summary>
    /// Handles new customer greeting
    /// </summary>
    public void TryBaristaEventGreetNewCustomer()
    {
        if (CanTriggerActionDialogue())
        {
            DoBaristaEventGreetNewCustomer();
        }
    }

    public void DoBaristaEventGreetNewCustomer()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_WelcomingNewCustomer, Statics.BaristaTalk_StartGame_Arcade, "WelcomingNewCustomer", 0, EventDialogueOffsetEnd);
    }

    /// <summary>
    /// Handles breast milk addition dialogue with timing constraints
    /// </summary>
    public void TryBaristaEventCupAddBreastMilk()
    {
        if (AllowBreastMilkDialogue && CanTriggerActionDialogue() && TimeScinceLevelLoad > EventBreastMilkAddCanSpeakAgain)
        {
            SetNewEventBreastMilkTime();
            DoBaristaEventCupAddBreastMilk();
        }
    }

    private void SetNewEventBreastMilkTime()
    {
        EventBreastMilkAddCanSpeakAgain = Time.time + Statics.GetRandomRange(EventBreastMilkAddMin, EventBreastMilkAddMax);
    }

    public void DoBaristaEventCupAddBreastMilk()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_AddMilk, Statics.BaristaTalk_Idle_Bust_50, "AddMilk", 0, EventDialogueOffsetEnd);
    }

    /// <summary>
    /// Checks if action-based dialogue can be triggered (doesn't interfere with main dialogue timing)
    /// </summary>
    private bool CanTriggerActionDialogue()
    {
        // Allow action dialogues if enough time has passed since last dialogue
        return Statics.HasTimePassed(TimeScinceLevelLoad, TimeLastDialogue, EventDialogueMinTime * 0.5f);
    }

    #endregion

    #region Main Event Methods

    public void DoBaristaEventAppron()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_ApronLimit, Statics.BaristaTalk_Idle_Bust_100, "ApronLimit", 0, EventDialogueOffsetEnd);
    }

    public void DoBaristaEventCookie()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Cookie, Statics.BaristaTalk_Idle_Mood_60, "Cookie", 0, EventDialogueOffsetEnd);
    }

    public void DoBaristaEventTooFull()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_TooFull, Statics.BaristaTalk_Idle_Bust_100, "TooFull", 0, EventDialogueOffsetEnd);
        SoundVariationSigh?.PlayRandomOneShot();
    }

    public void DoBaristaEventBadEnd()
    {
        TryStartBaristaTalkFromArrays(Statics.BaristaTalk_BadEnd, Statics.BaristaTalk_Idle_Mood_20, "BadEnd", 0, EventDialogueOffsetEnd);
    }

    #endregion

    #region Idle Dialogue Selection

    /// <summary>
    /// Selects appropriate idle dialogue based on current game state
    /// </summary>
    public void DoBaristaEventIdle()
    {
        if (gameMode.Money >= Consts.BaristaTalk.MoneyThresholds.HighTier)
        {
            int randomChoice = Statics.GetRandomRange(0, 3);
            switch (randomChoice)
            {
                case 0: DoBaristaMoneyTalk(); break;
                case 1: DoBaristaBustTalk(); break;
                default: DoBaristaMoodTalk(); break;
            }
        }
        else
        {
            if (Statics.RandomBool())
            {
                DoBaristaMoodTalk();
            }
            else
            {
                DoBaristaBustTalk();
            }
        }
    }

    /// <summary>
    /// Handles mood-based dialogue selection with randomization
    /// </summary>
    public void DoBaristaMoodTalk()
    {
        int randomVariation = Statics.GetRandomRange(Consts.BaristaTalk.MoodVariation.Min, Consts.BaristaTalk.MoodVariation.Max);
        float adjustedHappiness = gameMode.Happiness + randomVariation;

        if (adjustedHappiness > Consts.BaristaTalk.MoodThresholds.VeryHappy)
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Mood_100, Statics.BaristaTalk_Idle_Mood_80, "IdleMood100", 0, EventDialogueOffsetEnd);
        }
        else if (adjustedHappiness > Consts.BaristaTalk.MoodThresholds.Happy)
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Mood_80, Statics.BaristaTalk_Idle_Mood_60, "IdleMood80", 0, EventDialogueOffsetEnd);
        }
        else if (adjustedHappiness > Consts.BaristaTalk.MoodThresholds.Neutral)
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Mood_60, Statics.BaristaTalk_Idle_Mood_40, "IdleMood60", 0, EventDialogueOffsetEnd);
        }
        else if (adjustedHappiness > Consts.BaristaTalk.MoodThresholds.Sad)
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Mood_40, Statics.BaristaTalk_Idle_Mood_20, "IdleMood40", 0, EventDialogueOffsetEnd);
        }
        else
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Mood_20, Statics.BaristaTalk_StartGame_Arcade, "IdleMood20", 0, EventDialogueOffsetEnd);
        }
    }

    /// <summary>
    /// Handles bust size-based dialogue selection
    /// </summary>
    public void DoBaristaBustTalk()
    {
        if (gameMode.BustSize > Consts.BaristaTalk.BustThresholds.VeryLarge)
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Bust_100, Statics.BaristaTalk_Idle_Bust_80, "IdleBust100", 0, EventDialogueOffsetEnd);
        }
        else if (gameMode.BustSize > Consts.BaristaTalk.BustThresholds.Large)
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Bust_80, Statics.BaristaTalk_Idle_Bust_50, "IdleBust80", 0, EventDialogueOffsetEnd);
        }
        else if (gameMode.BustSize > Consts.BaristaTalk.BustThresholds.Medium)
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Bust_50, Statics.BaristaTalk_Idle_Bust_20, "IdleBust50", 0, EventDialogueOffsetEnd);
        }
        else
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Bust_20, Statics.BaristaTalk_Idle_Mood_60, "IdleBust20", 0, EventDialogueOffsetEnd);
        }
    }

    /// <summary>
    /// Handles money-based dialogue selection
    /// </summary>
    public void DoBaristaMoneyTalk()
    {
        if (gameMode.Money >= Consts.BaristaTalk.MoneyThresholds.HighTier)
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Money_Above_100, Statics.BaristaTalk_Idle_Mood_100, "MoneyAbove100", 0, EventDialogueOffsetEnd);
        }
        else if (gameMode.Money >= Consts.BaristaTalk.MoneyThresholds.MidTier)
        {
            TryStartBaristaTalkFromArrays(Statics.BaristaTalk_Idle_Money_Above_25, Statics.BaristaTalk_Idle_Mood_80, "MoneyAbove25", 0, EventDialogueOffsetEnd);
        }
    }

    private bool TryStartBaristaTalkFromArrays(DialogSentence[] primary, DialogSentence[] fallback, string context, float startOffset, float stopOffset)
    {
        DialogSentence sentence = null;

        if (!Statics.IsArrayNullOrEmpty(primary))
        {
            sentence = Statics.GetRandomFromArray(primary);
        }
        else if (!Statics.IsArrayNullOrEmpty(fallback))
        {
            Statics.LogWarningSafe($"BaristaTalk '{context}' is empty. Using fallback dialogue.");
            sentence = Statics.GetRandomFromArray(fallback);
        }
        else
        {
            Statics.LogErrorSafe($"BaristaTalk '{context}' and fallback are empty. Skipping dialogue.");
        }

        if (sentence == null)
        {
            // Prevent immediate re-trigger loops if a dialogue set is missing.
            SetNextDialogueTime();
            return false;
        }

        StartBaristaTalk(sentence, startOffset, stopOffset);
        return true;
    }

    #endregion

    #region Dialogue Execution

    /// <summary>
    /// Starts barista dialogue with specified timing
    /// </summary>
    /// <param name="sentence">Dialogue sentence to play</param>
    /// <param name="startOffset">Delay before starting dialogue</param>
    /// <param name="stopOffset">Duration after dialogue completion</param>
    public void StartBaristaTalk(DialogSentence sentence, float startOffset, float stopOffset)
    {
        if (sentence == null)
        {
            Statics.LogWarningSafe("Attempted to start barista talk with null sentence");
            return;
        }

        SetTimeForLastDialogue();

        if (startOffset > 0)
        {
            StartCoroutine(StartBaristaTalkCoroutine(sentence, startOffset, stopOffset));
        }
        else
        {
            ExecuteBaristaTalk(sentence, stopOffset);
        }
    }

    /// <summary>
    /// Coroutine for delayed dialogue execution
    /// </summary>
    private IEnumerator StartBaristaTalkCoroutine(DialogSentence sentence, float startOffset, float stopOffset)
    {
        yield return new WaitForSeconds(startOffset);
        ExecuteBaristaTalk(sentence, stopOffset);
    }

    /// <summary>
    /// Executes the actual dialogue through the dialogue manager
    /// </summary>
    private void ExecuteBaristaTalk(DialogSentence sentence, float stopOffset)
    {
        if (DialogueManager != null)
        {
            DialogueManager.StartDialoguebarista(sentence, stopOffset);
        }
        else
        {
            Statics.LogWarningSafe("DialogueManager is null, cannot start barista dialogue");
        }
    }

    #endregion

    #region Validation

    /// <summary>
    /// Validates game mode reference and logs error if missing
    /// </summary>
    /// <returns>True if game mode is valid</returns>
    [BurstCompile]
    private bool ValidateGameMode()
    {
        if (gameMode == null)
        {
            gameMode = BaseGameMode.instance;
            if (gameMode == null)
            {
                Statics.LogErrorSafe("Gamemode not found!");
                return false;
            }
        }
        return true;
    }

    #endregion
}
