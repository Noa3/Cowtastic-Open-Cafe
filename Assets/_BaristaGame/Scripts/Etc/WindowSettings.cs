using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages window resolution settings and fullscreen toggle functionality.
/// Handles resolution dropdown population, user selection, and persistent storage.
/// </summary>
public class WindowSettings : MonoBehaviour
{
    #region Serialized Fields

    [Header("UI Components")]
    [SerializeField]
    [Tooltip("Text component to display current resolution (currently unused)")]
    private TextMeshProUGUI resolutionText;

    [SerializeField]
    [Tooltip("Dropdown component for resolution selection")]
    public TMP_Dropdown _resolutionDropdown;

    #endregion

    #region Private Fields

    [Header("Settings")]
    [SerializeField]
    [Tooltip("Delay in seconds before allowing another resolution change")]
    private float _resolutionChangeCooldown = 1f;

    // Resolution management
    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;

    // State management
    private bool clickable = true;
    private bool isInitialized = false;

    #endregion

    #region Constants

    // Use constant from Consts file for consistency
    private static readonly string RESOLUTION_PREF_KEY = Consts.PlayerPrefResolution;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        InitializeResolutionSettings();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes resolution dropdown with available screen resolutions
    /// and loads saved resolution preference.
    /// </summary>
    private void InitializeResolutionSettings()
    {
        clickable = true;

        if (!EnsureResolutionDropdownReference())
        {
            Debug.LogError("WindowSettings: Initialization aborted because no TMP_Dropdown was found for resolution settings.");
            return;
        }

        // Get all available resolutions and remove duplicates
        resolutions = Screen.resolutions
            .GroupBy(r => new { r.width, r.height })
            .Select(g => g.OrderByDescending(r => r.refreshRateRatio.value).First())
            .ToArray();

        if (resolutions == null || resolutions.Length == 0)
        {
            Debug.LogError("WindowSettings: No available screen resolutions were found.");
            return;
        }

        PopulateResolutionDropdown();
        LoadSavedResolution();
    }

    /// <summary>
    /// Populates the resolution dropdown with formatted resolution options.
    /// </summary>
    private void PopulateResolutionDropdown()
    {
        _resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution res = resolutions[i];
            string option = $"{res.width} x {res.height} @ {res.refreshRateRatio.value:F0}Hz";
            options.Add(option);
        }

        _resolutionDropdown.AddOptions(options);

        // Set to last index initially to prevent unwanted trigger
        _resolutionDropdown.value = -1;
        _resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Loads the saved resolution preference from PlayerPrefs.
    /// </summary>
    private void LoadSavedResolution()
    {
        int savedResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_PREF_KEY, -1);

        // If no saved resolution, find the best match for current resolution
        if (savedResolutionIndex == -1)
        {
            savedResolutionIndex = FindBestResolutionMatch();
        }

        // Ensure the saved index is valid
        if (savedResolutionIndex >= 0 && savedResolutionIndex < resolutions.Length)
        {
            currentResolutionIndex = savedResolutionIndex;
        }
        else
        {
            currentResolutionIndex = resolutions.Length - 1; // Default to highest resolution
        }
        
        // Set the dropdown to show the correct resolution without triggering change
        _resolutionDropdown.value = currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();
        
        Debug.Log($"WindowSettings: Loaded resolution index {currentResolutionIndex} - {resolutions[currentResolutionIndex].width}x{resolutions[currentResolutionIndex].height}");
    }

    /// <summary>
    /// Ensures the resolution dropdown reference is valid.
    /// Tries to recover missing inspector wiring by searching children.
    /// </summary>
    /// <returns>True when dropdown is available</returns>
    private bool EnsureResolutionDropdownReference()
    {
        if (_resolutionDropdown != null)
        {
            return true;
        }

        _resolutionDropdown = GetComponentInChildren<TMP_Dropdown>(true);
        if (_resolutionDropdown == null)
        {
            Debug.LogError("WindowSettings: _resolutionDropdown is not assigned and no TMP_Dropdown could be found in children.");
            return false;
        }

        Debug.LogWarning("WindowSettings: _resolutionDropdown was not assigned in inspector. Auto-assigned from child TMP_Dropdown.");
        return true;
    }
    
    /// <summary>
    /// Finds the best matching resolution index for the current screen resolution
    /// </summary>
    /// <returns>Index of the best matching resolution</returns>
    private int FindBestResolutionMatch()
    {
        var currentRes = Screen.currentResolution;
        
        // First try to find exact match
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == currentRes.width && 
                resolutions[i].height == currentRes.height)
            {
                return i;
            }
        }
        
        // If no exact match, find closest resolution
        int bestIndex = resolutions.Length - 1; // Default to highest
        int bestScore = int.MaxValue;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            int widthDiff = Mathf.Abs(resolutions[i].width - currentRes.width);
            int heightDiff = Mathf.Abs(resolutions[i].height - currentRes.height);
            int score = widthDiff + heightDiff;
            
            if (score < bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }
        
        Debug.Log($"WindowSettings: Found best resolution match for {currentRes.width}x{currentRes.height} at index {bestIndex}");
        return bestIndex;
    }

    #endregion

    #region Resolution Management

    /// <summary>
    /// Sets the resolution index and applies the change with cooldown protection.
    /// </summary>
    /// <param name="newResolutionIndex">Index of the new resolution to apply</param>
    private void SetAndApplyResolution(int newResolutionIndex)
    {
        if (newResolutionIndex < 0 || newResolutionIndex >= resolutions.Length)
        {
            Debug.LogWarning($"Invalid resolution index: {newResolutionIndex}");
            return;
        }

        currentResolutionIndex = newResolutionIndex;
        ApplyCurrentResolution();
        StartCoroutine(ApplyCooldown());
    }

    /// <summary>
    /// Applies the currently selected resolution.
    /// </summary>
    private void ApplyCurrentResolution()
    {
        if (currentResolutionIndex >= 0 && currentResolutionIndex < resolutions.Length)
        {
            ApplyResolution(resolutions[currentResolutionIndex]);
            Debug.Log($"Applied resolution: {resolutions[currentResolutionIndex].width}x{resolutions[currentResolutionIndex].height}");
        }
    }

    /// <summary>
    /// Applies the specified resolution and saves the preference.
    /// </summary>
    /// <param name="resolution">Resolution to apply</param>
    private void ApplyResolution(Resolution resolution)
    {
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
        PlayerPrefs.SetInt(RESOLUTION_PREF_KEY, currentResolutionIndex);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Coroutine to handle click cooldown prevention.
    /// </summary>
    /// <returns>IEnumerator for coroutine</returns>
    private IEnumerator ApplyCooldown()
    {
        clickable = false;
        yield return new WaitForSeconds(_resolutionChangeCooldown);
        clickable = true;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Applies the saved resolution from PlayerPrefs without UI interaction
    /// Useful for applying resolution settings on game startup
    /// </summary>
    public static void ApplySavedResolution()
    {
        try
        {
            int savedResolutionIndex = PlayerPrefs.GetInt(Consts.PlayerPrefResolution, -1);
            
            // Only apply resolution if there's actually a saved preference
            if (savedResolutionIndex >= 0)
            {
                Resolution[] availableResolutions = Screen.resolutions;
                
                if (savedResolutionIndex < availableResolutions.Length)
                {
                    Resolution targetResolution = availableResolutions[savedResolutionIndex];
                    
                    // Preserve current fullscreen mode when applying resolution
                    FullScreenMode currentFullScreenMode = Screen.fullScreenMode;
                    
                    Screen.SetResolution(targetResolution.width, targetResolution.height, 
                                       currentFullScreenMode, targetResolution.refreshRateRatio);
                    
                    Debug.Log($"WindowSettings: Applied saved resolution {targetResolution.width}x{targetResolution.height} @ {targetResolution.refreshRateRatio.value:F0}Hz, FullScreenMode: {currentFullScreenMode}");
                }
                else
                {
                    Debug.LogWarning($"WindowSettings: Saved resolution index {savedResolutionIndex} is out of range");
                }
            }
            else
            {
                Debug.Log("WindowSettings: No saved resolution found, keeping current screen settings unchanged");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WindowSettings: Error applying saved resolution: {e.Message}");
        }
    }

    /// <summary>
    /// Called when the resolution dropdown value changes.
    /// Handles the first initialization call and subsequent user selections.
    /// </summary>
    /// <param name="selectedIndex">Index of the selected resolution</param>
    public void ApplyChanges(int selectedIndex)
    {
        if (_resolutionDropdown == null || resolutions == null || resolutions.Length == 0)
        {
            Debug.LogError("WindowSettings: Cannot apply resolution changes because settings are not initialized.");
            return;
        }

        // Skip the first call during initialization
        if (!isInitialized)
        {
            isInitialized = true;
            return;
        }

        // Prevent rapid clicking/changes
        if (!clickable)
        {
            return;
        }

        // Validate the selected index
        if (selectedIndex < 0 || selectedIndex >= resolutions.Length)
        {
            Debug.LogWarning($"Invalid resolution selection: {selectedIndex}");
            return;
        }

        // Update dropdown display
        _resolutionDropdown.value = selectedIndex;
        _resolutionDropdown.RefreshShownValue();

        // Apply the new resolution
        SetAndApplyResolution(selectedIndex);
    }

    /// <summary>
    /// Toggles fullscreen mode on/off.
    /// </summary>
    /// <param name="isFullScreen">True for fullscreen, false for windowed</param>
    public void ToggleFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        // Save fullscreen preference
        PlayerPrefs.SetInt(Consts.PlayerPrefFullscreen, isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"Fullscreen mode: {(isFullScreen ? "Enabled" : "Disabled")}");
    }

    #endregion

    #region Utility Methods (Currently Unused)

    /// <summary>
    /// Sets the resolution text display (currently unused but kept for future use).
    /// </summary>
    /// <param name="resolution">Resolution to display</param>
    private void SetResolutionText(Resolution resolution)
    {
        if (resolutionText != null)
        {
            resolutionText.text = $"{resolution.width} x {resolution.height}";
        }
    }

    #endregion
}