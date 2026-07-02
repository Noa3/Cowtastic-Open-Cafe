using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using Unity.Burst;

/// <summary>
/// Manages best time tracking and display for different game modes
/// Optimized for Unity 6.1 with Burst compilation and centralized constants
/// Supports both normal and milky game modes with persistent storage
/// </summary>
public class BestTimeManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Container for best time display elements")]
    public GameObject BestTimeHolder;

    [Tooltip("Text component displaying the current time")]
    public TextMeshProUGUI TimeText;

    [Tooltip("Localized string for time display text")]
    public LocalizedString StringTextTime;

    [Header("Game State")]
    [ReadOnly]
    [Tooltip("Current elapsed play time in seconds")]
    public float PlayTime = 0f;

    [ReadOnly]
    [Tooltip("Whether the current time is a new best time record")]
    public bool IsNewBestTime = false;

    [ReadOnly]
    [Tooltip("Current best time for the active game mode")]
    public float BestTime = Consts.TimeManagement.DefaultBestTime;

    // Private fields
    private GameMode currentMode = GameMode.Normal;
    private bool isMilkyMode;
    private bool isInitialized = false;

    // Singleton instance
    public static BestTimeManager instance;

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeSingleton();
        InitializeGameMode();
        InitializeLocalization();
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        UpdatePlayTime();
        UpdateTimeDisplay();
        CheckForNewBestTime();
    }

    private void OnDestroy()
    {
        CleanupLocalization();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize singleton pattern with null check
    /// </summary>
    private void InitializeSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Statics.LogWarningSafe("Multiple BestTimeManager instances detected, destroying duplicate");
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Initialize game mode and load corresponding best time
    /// </summary>
    private void InitializeGameMode()
    {
        // Load milky mode preference safely
        isMilkyMode = PlayerPrefs.GetInt(Consts.PlayerPrefNextIsMilkyMode, 0) == 1;

        // Get current scene name safely
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Determine game mode using centralized logic
        currentMode = Statics.DetermineGameMode(currentSceneName, isMilkyMode);

        // Load best time for determined mode
        BestTime = Statics.LoadBestTime(currentMode);

        if (BestTime >= Consts.TimeManagement.DefaultBestTime)
        {
            Statics.LogWarningSafe($"No best time record found for mode: {currentMode}");
        }
    }

    /// <summary>
    /// Setup localization event handlers
    /// </summary>
    private void InitializeLocalization()
    {
        if (StringTextTime != null)
        {
            StringTextTime.StringChanged += OnTextTimeLocalizationChanged;
        }
        else
        {
            Statics.LogWarningSafe("StringTextTime LocalizedString is not assigned");
        }
    }

    /// <summary>
    /// Cleanup localization event handlers
    /// </summary>
    private void CleanupLocalization()
    {
        if (StringTextTime != null)
        {
            StringTextTime.StringChanged -= OnTextTimeLocalizationChanged;
        }
    }

    #endregion

    #region Update Logic

    /// <summary>
    /// Update play time with time scale consideration
    /// </summary>
    [BurstCompile]
    private void UpdatePlayTime()
    {
        PlayTime += Time.deltaTime * Time.timeScale;
    }

    /// <summary>
    /// Update time display with formatted text
    /// </summary>
    private void UpdateTimeDisplay()
    {
        if (TimeText == null)
        {
            Statics.LogErrorSafe("TimeText component is not assigned");
            return;
        }

        string formattedTime = Statics.FormatTimeAsString(PlayTime);
        TimeText.text = Statics.TextTime + Consts.TimeManagement.TimeDisplaySeparator + formattedTime;
    }

    /// <summary>
    /// Check if current time qualifies as new best time
    /// </summary>
    [BurstCompile]
    private void CheckForNewBestTime()
    {
        bool wasNewBestTime = IsNewBestTime;
        IsNewBestTime = Statics.IsNewBestTime(PlayTime, BestTime);

        // Log when new best time is achieved (only once)
        if (IsNewBestTime && !wasNewBestTime)
        {
            Debug.Log($"New best time achieved! {Statics.FormatTimeAsString(PlayTime)} for mode: {currentMode}");
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle localization changes for time text
    /// </summary>
    /// <param name="localizedValue">New localized string value</param>
    private void OnTextTimeLocalizationChanged(string localizedValue)
    {
        if (!string.IsNullOrEmpty(localizedValue))
        {
            Statics.TextTime = localizedValue;
        }
        else
        {
            Statics.LogWarningSafe("Received empty localized time text value");
        }
    }

    #endregion

    #region Public API

    /// <summary>
    /// Save current play time as new best time for the current game mode
    /// Includes validation and error handling
    /// </summary>
    public void SaveBestTime()
    {
        if (PlayTime <= 0)
        {
            Statics.LogWarningSafe("Cannot save best time: PlayTime is zero or negative");
            return;
        }

        try
        {
            Statics.SaveBestTime(currentMode, PlayTime);
            BestTime = PlayTime; // Update local cache
            Debug.Log($"Best time saved: {Statics.FormatTimeAsString(PlayTime)} for mode: {currentMode}");
        }
        catch (System.Exception ex)
        {
            Statics.LogErrorSafe($"Failed to save best time: {ex.Message}");
        }
    }

    /// <summary>
    /// Reset play time to zero
    /// </summary>
    public void ResetPlayTime()
    {
        PlayTime = 0f;
        IsNewBestTime = false;
    }

    /// <summary>
    /// Get current game mode
    /// </summary>
    /// <returns>Current GameMode enum value</returns>
    public GameMode GetCurrentGameMode()
    {
        return currentMode;
    }

    /// <summary>
    /// Get whether milky mode is currently active
    /// </summary>
    /// <returns>True if milky mode is active</returns>
    public bool IsMilkyModeActive()
    {
        return isMilkyMode;
    }

    /// <summary>
    /// Force refresh of game mode detection (useful for testing)
    /// </summary>
    public void RefreshGameMode()
    {
        InitializeGameMode();
    }

    #endregion
}

/// <summary>
/// Enumeration of available game modes
/// Supports both normal and milky variants
/// </summary>
public enum GameMode
{
    Normal,
    Hard,
    Casual,
    Chaos,
    UltraChaos,
    NoasMod,
    NormalMilky,
    HardMilky,
    CasualMilky,
    ChaosMilky,
    UltraChaosMilky
}