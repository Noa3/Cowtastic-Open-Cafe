/*
 * BARISTA GAME CHEAT SYSTEM - CROSS-PLATFORM VERSION
 * ==================================================
 * 
 * This cheat manager provides various fun cheats for players to experiment with!
 * Supports multiple input methods across different platforms.
 * 
 * MOBILE GESTURES:
 * ----------------
 * 5-finger triple tap - Trigger debug panel visibility
 * 3-finger tap (different positions) - Various cheats
 * 4-finger tap - God Mode toggle
 * 2-finger gestures - Production/Upgrades
 * 
 * The system automatically detects the platform and enables appropriate input methods.
 * UI logic is handled externally - this manager only provides cheat functionality.
 */

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Cross-Platform Cheat Manager for the Barista Game
/// Supports desktop (F-keys, number keys, combinations) and mobile (gestures, touch sequences)
/// UI handling is done externally - this provides core cheat functionality
/// </summary>
public class cheatManager : MonoBehaviour
{
    [Header("Platform Detection")]
    [SerializeField] private bool isMobilePlatform = false;
    [SerializeField] private bool enableMobileGestures = true;
    [SerializeField] private bool enableDesktopAlternatives = true;
    [SerializeField] private bool enableSecretCombinations = true;
    
    [Header("Cheat Settings")]
    [SerializeField] private bool cheatEnabled = true;
    [SerializeField] private float moneyCheatAmount = 1000f;
    [SerializeField] private float bustSizeCheatAmount = 20f;
    [SerializeField] private int productionRateMultiplier = 5;
    
    [Header("Mobile Touch Settings")]
    [SerializeField] private int tapCountRequired = 3; // Number of taps required
    [SerializeField] private float tapWindow = 1.0f; // Time window for multiple taps
    
    [Header("Statistics Control")]
    [SerializeField] private bool pauseStatisticsWhenCheating = true;
    [SerializeField] private bool statisticsCurrentlyPaused = false;
    
    [Header("External UI Events")]
    // Event that external UI can subscribe to for debug panel toggle
    public System.Action<bool> OnDebugPanelToggleRequested;
    
    [Header("References")]
    private BaseGameMode gameMode;
    private OrderManager orderManager;
    private StatsManager statsManager;
    private EventManager eventManager;
    private CupController cupController;
    private StatisticsHolder statisticsHolder;
    
    [Header("Cheat Status Display")]
    [SerializeField] private bool godModeActive = false;
    [SerializeField] private bool fastProductionActive = false;
    [SerializeField] private bool autoOrderCompleteActive = false;
    
    private bool originalCanLevelUp;
    private float originalProductionRate;
    
    // Mobile gesture tracking
    private int currentTapCount = 0;
    private float lastTapTime = 0f;
    
    // Desktop combination key tracking
    private bool ctrlHeld = false;
    private bool shiftHeld = false;
    
    // Statistics pause tracking
    private Dictionary<string, System.Reflection.MethodInfo> originalStatisticsMethods = new Dictionary<string, System.Reflection.MethodInfo>();

    void Start()
    {
        // Detect platform
        DetectPlatform();
        
        // Get references to game managers
        gameMode = BaseGameMode.instance;
        orderManager = FindFirstObjectByType<OrderManager>();
        statsManager = StatsManager.instance;
        eventManager = FindFirstObjectByType<EventManager>();
        cupController = CupController.instance;
        statisticsHolder = StatisticsHolder.instance;
        
        // Store original values for restoration
        if (gameMode != null)
        {
            originalCanLevelUp = gameMode.CanLevelUp;
            originalProductionRate = (float)gameMode.ProductionRate;
        }
        
        // Display available cheats based on platform
        LogPlatformSpecificBindings();
    }

    private void DetectPlatform()
    {
        #if UNITY_ANDROID || UNITY_IOS
            isMobilePlatform = true;
        #else
            isMobilePlatform = Application.platform == RuntimePlatform.Android ||
                              Application.platform == RuntimePlatform.IPhonePlayer ||
                              Application.isMobilePlatform;
        #endif
        
        #if UNITY_EDITOR
            // Allow testing mobile mode in editor by uncommenting the line below
            // isMobilePlatform = true;
        #endif
        
        Debug.Log($"[CHEAT MANAGER] Platform detected: {(isMobilePlatform ? "Mobile" : "Desktop")}");
    }

    void Update()
    {
        if (!cheatEnabled) return;
        
        // Handle input based on platform
        if (isMobilePlatform && enableMobileGestures)
        {
            HandleMobileInput();
        }
        else
        {
            HandleDesktopInput();
        }
        
        // Handle active cheats that run continuously
        if (autoOrderCompleteActive)
        {
            AutoCompleteOrders();
        }
        
        // Keep god mode active
        if (godModeActive && gameMode != null)
        {
            gameMode.Happiness = 100f;
        }
    }

    private void LogPlatformSpecificBindings()
    {
        if (isMobilePlatform)
        {
            Debug.Log(@"[MOBILE CHEAT GESTURES]
            5-finger triple tap - Request Debug Panel Toggle
            3-finger tap (different positions) - Various cheats
            4-finger tap - God Mode toggle
            2-finger gestures - Production/Upgrades
            Individual gestures work even without external UI");
        }
        else
        {
            Debug.Log(@"[DESKTOP CHEAT BINDINGS]
            F2/1" + (enableSecretCombinations ? "/Ctrl+Shift+1" : "") + " - Add Money (+" + moneyCheatAmount + @")
            F3/2" + (enableSecretCombinations ? "/Ctrl+Shift+2" : "") + " - Max Happiness (100%)" + @"
            F4/3" + (enableSecretCombinations ? "/Ctrl+Shift+3" : "") + " - Increase Bust Size (+" + bustSizeCheatAmount + @")
            F5/4" + (enableSecretCombinations ? "/Ctrl+Shift+4" : "") + " - Toggle God Mode" + @"
            F6/5" + (enableSecretCombinations ? "/Ctrl+Shift+5" : "") + " - Fast Production (x" + productionRateMultiplier + @")
            F7/6" + (enableSecretCombinations ? "/Ctrl+Shift+6" : "") + " - Complete Order" + @"
            F8/7" + (enableSecretCombinations ? "/Ctrl+Shift+7" : "") + " - Auto Order Completion" + @"
            F9/8" + (enableSecretCombinations ? "/Ctrl+Shift+8" : "") + " - Max All Upgrades" + @"
            F10/9" + (enableSecretCombinations ? "/Ctrl+Shift+9" : "") + " - Reset Bust Size" + @"
            F11/0" + (enableSecretCombinations ? "/Ctrl+Shift+0" : "") + " - Toggle Time Scale");
        }
    }

    #region Desktop Input Handling
    
    private void HandleDesktopInput()
    {
        // Track modifier keys for secret combinations
        ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool secretCombo = ctrlHeld && shiftHeld && enableSecretCombinations;
        
        // F-Keys (primary method)
        if (Input.GetKeyDown(KeyCode.F2)) CheatAddMoney();
        else if (Input.GetKeyDown(KeyCode.F3)) CheatMaxHappiness();
        else if (Input.GetKeyDown(KeyCode.F4)) CheatIncreaseBustSize();
        else if (Input.GetKeyDown(KeyCode.F5)) CheatToggleGodMode();
        else if (Input.GetKeyDown(KeyCode.F6)) CheatToggleFastProduction();
        else if (Input.GetKeyDown(KeyCode.F7)) CheatCompleteCurrentOrder();
        else if (Input.GetKeyDown(KeyCode.F8)) CheatToggleAutoOrderCompletion();
        else if (Input.GetKeyDown(KeyCode.F9)) CheatMaxAllUpgrades();
        else if (Input.GetKeyDown(KeyCode.F10)) CheatResetBustSize();
        else if (Input.GetKeyDown(KeyCode.F11)) CheatToggleTimeScale();
        
        // Number keys (alternative method)
        else if (enableDesktopAlternatives)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || (secretCombo && Input.GetKeyDown(KeyCode.Alpha1))) CheatAddMoney();
            else if (Input.GetKeyDown(KeyCode.Alpha2) || (secretCombo && Input.GetKeyDown(KeyCode.Alpha2))) CheatMaxHappiness();
            else if (Input.GetKeyDown(KeyCode.Alpha3) || (secretCombo && Input.GetKeyDown(KeyCode.Alpha3))) CheatIncreaseBustSize();
            else if (Input.GetKeyDown(KeyCode.Alpha4) || (secretCombo && Input.GetKeyDown(KeyCode.Alpha4))) CheatToggleGodMode();
            else if (Input.GetKeyDown(KeyCode.Alpha5) || (secretCombo && Input.GetKeyDown(KeyCode.Alpha5))) CheatToggleFastProduction();
            else if (Input.GetKeyDown(KeyCode.Alpha6) || (secretCombo && Input.GetKeyDown(KeyCode.Alpha6))) CheatCompleteCurrentOrder();
            else if (Input.GetKeyDown(KeyCode.Alpha7) || (secretCombo && Input.GetKeyDown(KeyCode.Alpha7))) CheatToggleAutoOrderCompletion();
            else if (Input.GetKeyDown(KeyCode.Alpha8) || (secretCombo && Input.GetKeyDown(KeyCode.Alpha8))) CheatMaxAllUpgrades();
            else if (Input.GetKeyDown(KeyCode.Alpha9) || (secretCombo && Input.GetKeyDown(KeyCode.Alpha9))) CheatResetBustSize();
            else if (Input.GetKeyDown(KeyCode.Alpha0) || (secretCombo && Input.GetKeyDown(KeyCode.Alpha0))) CheatToggleTimeScale();
        }
    }
    
    #endregion

    #region Mobile Input Handling
    
    private void HandleMobileInput()
    {
        // Handle the special gesture to request debug panel toggle: 5 fingers, 3 taps
        if (Input.touchCount == 5)
        {
            DetectMultiTapGesture();
        }
        
        // Reset tap count if time window expires
        if (Time.time - lastTapTime > tapWindow)
        {
            currentTapCount = 0;
        }
        
        // Handle other touch gestures for direct cheats
        if (Input.touchCount > 0 && Input.touchCount != 5)
        {
            ProcessTouchInput();
        }
    }
    
    private void DetectMultiTapGesture()
    {
        // Check if all 5 fingers just began touching
        bool allFingersJustTouched = true;
        for (int i = 0; i < 5; i++)
        {
            if (i < Input.touchCount)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase != TouchPhase.Began)
                {
                    allFingersJustTouched = false;
                    break;
                }
            }
            else
            {
                allFingersJustTouched = false;
                break;
            }
        }
        
        if (allFingersJustTouched)
        {
            float currentTime = Time.time;
            
            if (currentTime - lastTapTime <= tapWindow)
            {
                currentTapCount++;
            }
            else
            {
                currentTapCount = 1;
            }
            
            lastTapTime = currentTime;
            
            if (currentTapCount >= tapCountRequired)
            {
                RequestDebugPanelToggle();
                currentTapCount = 0;
            }
            
            Debug.Log($"[CHEAT] 5-finger tap detected! Count: {currentTapCount}/{tapCountRequired}");
        }
    }
    
    private void RequestDebugPanelToggle()
    {
        Debug.Log("[CHEAT] Requesting debug panel toggle from external UI");
        OnDebugPanelToggleRequested?.Invoke(true);
    }
    
    private void ProcessTouchInput()
    {
        int touchCount = Input.touchCount;
        Vector2 centerTouch = GetTouchCenter();
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        
        // Simple gesture recognition for direct cheats
        if (touchCount == 3)
        {
            Handle3FingerGestures(centerTouch, screenSize);
        }
        else if (touchCount == 4)
        {
            Handle4FingerGestures();
        }
        else if (touchCount == 2)
        {
            Handle2FingerGestures();
        }
    }
    
    private void Handle3FingerGestures(Vector2 center, Vector2 screenSize)
    {
        bool allBegan = true;
        for (int i = 0; i < 3; i++)
        {
            if (Input.GetTouch(i).phase != TouchPhase.Began)
            {
                allBegan = false;
                break;
            }
        }
        
        if (!allBegan) return;
        
        // Position-based cheats
        if (center.x < screenSize.x * 0.33f && center.y > screenSize.y * 0.66f)
        {
            CheatAddMoney(); // Top-left
        }
        else if (center.x > screenSize.x * 0.66f && center.y > screenSize.y * 0.66f)
        {
            CheatMaxHappiness(); // Top-right
        }
        else
        {
            CheatIncreaseBustSize(); // Center/other
        }
    }
    
    private void Handle4FingerGestures()
    {
        bool allBegan = true;
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetTouch(i).phase != TouchPhase.Began)
            {
                allBegan = false;
                break;
            }
        }
        
        if (allBegan)
        {
            CheatToggleGodMode();
        }
    }
    
    private void Handle2FingerGestures()
    {
        // Simple 2-finger swipe down detection
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            
            if (touch1.deltaPosition.y < -20f && touch2.deltaPosition.y < -20f)
            {
                CheatToggleFastProduction();
            }
        }
    }
    
    private Vector2 GetTouchCenter()
    {
        Vector2 center = Vector2.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            center += Input.GetTouch(i).position;
        }
        return center / Input.touchCount;
    }
    
    #endregion

    #region Statistics Control
    
    /// <summary>
    /// Pauses statistics tracking to prevent cheated gameplay from affecting achievements
    /// </summary>
    public void PauseStatistics()
    {
        if (statisticsHolder == null || statisticsCurrentlyPaused) return;
        
        statisticsCurrentlyPaused = true;
        Debug.Log("[CHEAT] Statistics tracking PAUSED - achievements disabled during cheat usage");
        
        // Note: In a more complex implementation, you could disable specific StatisticsHolder methods
        // For now, we'll use a flag-based approach that external systems can check
    }
    
    /// <summary>
    /// Resumes statistics tracking
    /// </summary>
    public void ResumeStatistics()
    {
        if (statisticsHolder == null || !statisticsCurrentlyPaused) return;
        
        statisticsCurrentlyPaused = false;
        Debug.Log("[CHEAT] Statistics tracking RESUMED");
    }
    
    /// <summary>
    /// Check if any cheats are currently active
    /// </summary>
    public bool AnyCheatsActive()
    {
        return godModeActive || fastProductionActive || autoOrderCompleteActive;
    }
    
    /// <summary>
    /// Updates statistics pause state based on active cheats
    /// </summary>
    private void UpdateStatisticsPauseState()
    {
        if (!pauseStatisticsWhenCheating) return;
        
        if (AnyCheatsActive() && !statisticsCurrentlyPaused)
        {
            PauseStatistics();
        }
        else if (!AnyCheatsActive() && statisticsCurrentlyPaused)
        {
            ResumeStatistics();
        }
    }
    
    /// <summary>
    /// Public property that external systems can check to see if statistics should be tracked
    /// </summary>
    public bool ShouldTrackStatistics => !statisticsCurrentlyPaused;
    
    #endregion

    #region Cheat Methods
    
    private void CheatAddMoney()
    {
        if (gameMode != null)
        {
            gameMode.AddMoney(moneyCheatAmount);
            Debug.Log($"[CHEAT] Added {moneyCheatAmount} money! Current: {gameMode.Money:F2}");
            UpdateStatisticsPauseState();
        }
    }

    private void CheatMaxHappiness()
    {
        if (gameMode != null)
        {
            gameMode.Happiness = 100f;
            Debug.Log("[CHEAT] Happiness set to maximum (100%)!");
            UpdateStatisticsPauseState();
        }
    }

    private void CheatIncreaseBustSize()
    {
        if (gameMode != null)
        {
            float oldSize = gameMode.TargetBustSize;
            gameMode.TargetBustSize += bustSizeCheatAmount;
            if (gameMode.TargetBustSize > gameMode.CurrentMaxSize)
            {
                gameMode.TargetBustSize = gameMode.CurrentMaxSize;
            }
            Debug.Log($"[CHEAT] Bust size: {oldSize:F1} ? {gameMode.TargetBustSize:F1}");
            UpdateStatisticsPauseState();
        }
    }

    private void CheatToggleGodMode()
    {
        godModeActive = !godModeActive;
        
        if (gameMode != null)
        {
            if (godModeActive)
            {
                gameMode.Happiness = 100f;
                gameMode.WhileHeadpatingDontDecreaseHappyness = true;
                Debug.Log("[CHEAT] ?? GOD MODE ACTIVATED!");
            }
            else
            {
                gameMode.WhileHeadpatingDontDecreaseHappyness = false;
                Debug.Log("[CHEAT] God Mode DEACTIVATED");
            }
        }
        UpdateStatisticsPauseState();
    }

    private void CheatToggleFastProduction()
    {
        fastProductionActive = !fastProductionActive;
        
        if (gameMode != null)
        {
            if (fastProductionActive)
            {
                gameMode.ProductionRate = originalProductionRate * productionRateMultiplier;
                gameMode.EventFastMilkFill = true;
                gameMode.EventFastMilkFillMultipler = 5f;
                Debug.Log($"[CHEAT] ? FAST PRODUCTION ACTIVATED!");
            }
            else
            {
                gameMode.ProductionRate = originalProductionRate;
                gameMode.EventFastMilkFill = false;
                gameMode.EventFastMilkFillMultipler = 1f;
                Debug.Log("[CHEAT] Fast Production DEACTIVATED");
            }
        }
        UpdateStatisticsPauseState();
    }

    private void CheatCompleteCurrentOrder()
    {
        if (orderManager != null && orderManager.orderIsActive)
        {
            if (cupController != null && orderManager.ActiveIngreedentPercentages != null)
            {
                FillCupPerfectly();
                orderManager.OrderFinished();
                Debug.Log("[CHEAT] ? Order completed with PERFECT rating!");
                UpdateStatisticsPauseState();
            }
        }
        else
        {
            Debug.Log("[CHEAT] No active order to complete!");
        }
    }

    private void FillCupPerfectly()
    {
        if (cupController == null || orderManager == null || orderManager.ActiveIngreedentPercentages == null)
            return;

        cupController.ResetCup();
        var percentages = orderManager.ActiveIngreedentPercentages;
        
        if (percentages.Count > 0) cupController.Chocolate = percentages[0] / 100f;
        if (percentages.Count > 1) cupController.Milk = percentages[1] / 100f;
        if (percentages.Count > 2) cupController.Tea = percentages[2] / 100f;
        if (percentages.Count > 3) cupController.Cream = percentages[3] / 100f;
        if (percentages.Count > 4) cupController.Espresso = percentages[4] / 100f;
        if (percentages.Count > 5) cupController.Sugar = percentages[5] / 100f;
        if (percentages.Count > 6) cupController.Coffee = percentages[6] / 100f;
        if (percentages.Count > 7) cupController.Boba = percentages[7] > 0;
        if (percentages.Count > 8) cupController.Ice = percentages[8] > 0;
        if (percentages.Count > 9) cupController.WhippedCream = percentages[9] > 0;
        if (percentages.Count > 10) cupController.ChocolateSauce = percentages[10] > 0;
        if (percentages.Count > 11) cupController.CaramelSauce = percentages[11] > 0;
        if (percentages.Count > 12) cupController.Sprinkles = percentages[12] > 0;
        if (percentages.Count > 13) cupController.BreastMilk = percentages[13] / 100f;
        
        cupController.Fullness = 1f;
    }

    private void CheatToggleAutoOrderCompletion()
    {
        autoOrderCompleteActive = !autoOrderCompleteActive;
        Debug.Log($"[CHEAT] Auto Order Completion {(autoOrderCompleteActive ? "ACTIVATED" : "DEACTIVATED")}!");
        UpdateStatisticsPauseState();
    }

    private void AutoCompleteOrders()
    {
        if (orderManager != null && orderManager.orderIsActive)
        {
            if (Time.time % 2f < Time.deltaTime)
            {
                FillCupPerfectly();
                orderManager.OrderFinished();
            }
        }
    }

    private void CheatMaxAllUpgrades()
    {
        if (gameMode != null)
        {
            gameMode.AddMoney(50000f);
            
            for (int i = 0; i < 10; i++)
            {
                gameMode.BuyUpgradeProduction(1);
                gameMode.BuyMaxSize(1);
                gameMode.BuyHappyness(1);
                gameMode.BuyMilkFullness(1, false);
            }
            
            gameMode.UpgradeCanGrow = true;
            Debug.Log("[CHEAT] ?? ALL UPGRADES MAXED OUT!");
            UpdateStatisticsPauseState();
        }
    }

    private void CheatResetBustSize()
    {
        if (gameMode != null)
        {
            float oldSize = gameMode.TargetBustSize;
            gameMode.TargetBustSize = 0f;
            gameMode.BustSize = 0f;
            Debug.Log($"[CHEAT] ?? Bust size reset: {oldSize:F1} ? 0");
            UpdateStatisticsPauseState();
        }
    }

    private void CheatToggleTimeScale()
    {
        float currentTimeScale = Time.timeScale;
        
        if (Mathf.Approximately(currentTimeScale, 1f))
        {
            Time.timeScale = 2f;
            Debug.Log("[CHEAT] ? Time scale: 2x SPEED!");
        }
        else if (Mathf.Approximately(currentTimeScale, 2f))
        {
            Time.timeScale = 0.5f;
            Debug.Log("[CHEAT] ?? Time scale: 0.5x SLOW MOTION!");
        }
        else
        {
            Time.timeScale = 1f;
            Debug.Log("[CHEAT] ?? Time scale: NORMAL!");
        }
        UpdateStatisticsPauseState();
    }
    
    #endregion

    #region Public API for External UI
    
    /// <summary>
    /// Public method to trigger cheats programmatically (for UI buttons)
    /// </summary>
    public void TriggerCheat(int cheatIndex)
    {
        if (!cheatEnabled) return;
        
        switch (cheatIndex)
        {
            case 0: CheatAddMoney(); break;
            case 1: CheatMaxHappiness(); break;
            case 2: CheatIncreaseBustSize(); break;
            case 3: CheatToggleGodMode(); break;
            case 4: CheatToggleFastProduction(); break;
            case 5: CheatCompleteCurrentOrder(); break;
            case 6: CheatToggleAutoOrderCompletion(); break;
            case 7: CheatMaxAllUpgrades(); break;
            case 8: CheatResetBustSize(); break;
            case 9: CheatToggleTimeScale(); break;
        }
    }
    
    /// <summary>
    /// Get names of all available cheats for UI buttons
    /// </summary>
    public string[] GetCheatNames()
    {
        return new string[]
        {
            "Add Money (+1000)",
            "Max Happiness (100%)", 
            "Increase Bust Size (+20)",
            "Toggle God Mode",
            "Toggle Fast Production",
            "Complete Current Order",
            "Toggle Auto Orders",
            "Max All Upgrades",
            "Reset Bust Size",
            "Toggle Time Scale"
        };
    }
    
    /// <summary>
    /// Get current status of a specific cheat
    /// </summary>
    public bool GetCheatStatus(int cheatIndex)
    {
        switch (cheatIndex)
        {
            case 3: return godModeActive;
            case 4: return fastProductionActive;
            case 6: return autoOrderCompleteActive;
            default: return false; // One-time cheats don't have a status
        }
    }
    
    #endregion

    void OnDisable()
    {
        if (gameMode != null)
        {
            Time.timeScale = 1f;
            gameMode.ProductionRate = originalProductionRate;
            gameMode.EventFastMilkFill = false;
            gameMode.EventFastMilkFillMultipler = 1f;
            gameMode.WhileHeadpatingDontDecreaseHappyness = false;
        }
        
        // Resume statistics when cheat manager is disabled
        ResumeStatistics();
        
        Debug.Log("[CHEAT MANAGER] Cheats disabled and values restored!");
    }

    public void SetCheatEnabled(bool enabled)
    {
        cheatEnabled = enabled;
        Debug.Log($"[CHEAT MANAGER] Cheats {(enabled ? "ENABLED" : "DISABLED")}");
        
        if (enabled)
        {
            LogPlatformSpecificBindings();
        }
        else
        {
            // Resume statistics when cheats are disabled
            ResumeStatistics();
        }
    }
    
    public string GetCheatStatus()
    {
        if (!cheatEnabled) return "Cheats Disabled";
        
        var activeFeatures = new System.Collections.Generic.List<string>();
        if (godModeActive) activeFeatures.Add("God Mode");
        if (fastProductionActive) activeFeatures.Add("Fast Production");
        if (autoOrderCompleteActive) activeFeatures.Add("Auto Orders");
        
        string statusText = activeFeatures.Count > 0 ? $"Active: {string.Join(", ", activeFeatures)}" : "Cheats Ready";
        
        if (statisticsCurrentlyPaused)
        {
            statusText += " (Statistics Paused)";
        }
        
        return statusText;
    }

    private void InitializeMobileDebugPanel()
    {
        // External UI will handle debug panel creation
        // This method is kept for compatibility but no longer creates UI
        Debug.Log("[CHEAT MANAGER] Mobile debug panel initialization delegated to external UI system");
    }
}
