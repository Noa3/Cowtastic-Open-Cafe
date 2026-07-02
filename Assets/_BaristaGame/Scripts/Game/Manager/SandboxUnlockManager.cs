using UnityEngine;
using System.Collections;

public class SandboxUnlockManager : MonoBehaviour
{
    [Header("Unlock Settings")]
    [Tooltip("SoundEffectVariation component to play unlock sound")]
    public SoundEffectVariation unlockSoundVariation;
    
    [Tooltip("Time window to register all 5 touches (in seconds)")]
    public float touchTimeWindow = 1f;
    
    [Tooltip("Enable debug logging")]
    public bool debugMode = false;

    // Events
    public System.Action OnSandboxUnlocked;

    private string keywordInput = "";
    private const string UNLOCK_KEYWORD = "unlock";
    private float lastTouchTime;
    private int maxTouchesDetected = 0;
    private bool commandArgUnlockDetected = false;

    // Scene name constants
    private const string SceneNameCasual = "Game_Arcade_Casual";
    private const string SceneNameNormal = "Game_Arcade";
    private const string SceneNameHard = "Game_Arcade_Hard";
    private const string SceneNameChaos = "Game_Arcade_Chaos";
    private const string SceneNameUltraChaos = "Game_Arcade_UltraChaos";

    private void Start()
    {
        // Check for command line argument unlock on start
        CheckCommandLineUnlock();
    }

    private void Update()
    {
        // Always check for user input, regardless of unlock state
        CheckKeyboardInput();
        CheckTouchInput();
    }

    /// <summary>
    /// Check if sandbox should be unlocked based on all possible conditions
    /// </summary>
    /// <returns>True if sandbox should be unlocked</returns>
    public bool ShouldSandboxBeUnlocked()
    {
        // Check traditional unlock conditions
        bool CasualWon = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSceneWon + SceneNameCasual, false.ToString()));
        bool NormalWon = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSceneWon + SceneNameNormal, false.ToString()));
        bool HardWon = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSceneWon + SceneNameHard, false.ToString()));
        bool ChaosWon = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSceneWon + SceneNameChaos, false.ToString()));
        bool ChaosMilkyWon = (PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeUltraChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix, -1) > 0);
        bool CommandArgUnlock = !string.IsNullOrEmpty(GetCommandLineArg("-SandboxUnlocked"));

        // Check if unlocked through traditional means or special methods
        return (CasualWon && NormalWon && HardWon && ChaosWon)
            || ChaosMilkyWon
            || CommandArgUnlock
            || commandArgUnlockDetected;
    }

    private void CheckCommandLineUnlock()
    {
        // Check if sandbox unlock command line argument is present
        string sandboxArg = GetCommandLineArg("-SandboxUnlocked");
        commandArgUnlockDetected = !string.IsNullOrEmpty(sandboxArg);
        
        if (commandArgUnlockDetected)
        {
            if (debugMode)
            {
                Debug.Log("SandboxUnlockManager: Command line sandbox unlock detected (silent unlock)!");
            }
            // Trigger unlock without sound for command line
            TriggerSandboxUnlock(false);
        }
    }

    /// <summary>
    /// Get command line argument value
    /// </summary>
    /// <param name="name">Argument name</param>
    /// <returns>Argument value or null if not found</returns>
    private string GetCommandLineArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }
        return null;
    }

    private void CheckKeyboardInput()
    {
        // Check for keyboard input
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // Backspace
            {
                if (keywordInput.Length > 0)
                {
                    keywordInput = keywordInput.Substring(0, keywordInput.Length - 1);
                }
            }
            else if (c == '\n' || c == '\r') // Enter
            {
                // Clear input on enter
                keywordInput = "";
            }
            else if (char.IsLetter(c))
            {
                keywordInput += c.ToString().ToLower();
                
                // Keep only the last 6 characters to prevent memory issues
                if (keywordInput.Length > 6)
                {
                    keywordInput = keywordInput.Substring(keywordInput.Length - 6);
                }

                if (debugMode)
                {
                    Debug.Log($"SandboxUnlockManager: Current input: {keywordInput}");
                }

                // Check if the keyword is typed
                if (keywordInput.EndsWith(UNLOCK_KEYWORD))
                {
                    if (debugMode)
                    {
                        Debug.Log("SandboxUnlockManager: Unlock keyword detected by user!");
                    }
                    // Trigger unlock with sound for user input
                    TriggerSandboxUnlock(true);
                    return;
                }
            }
        }
    }

    private void CheckTouchInput()
    {
        // Check for touch input (mobile/tablet)
        if (Input.touchCount > 0)
        {
            // Update max touches detected within the time window
            if (Time.time - lastTouchTime > touchTimeWindow)
            {
                maxTouchesDetected = Input.touchCount;
                lastTouchTime = Time.time;
            }
            else
            {
                maxTouchesDetected = Mathf.Max(maxTouchesDetected, Input.touchCount);
            }

            if (debugMode && Input.touchCount > 1)
            {
                Debug.Log($"SandboxUnlockManager: {Input.touchCount} touches detected. Max in window: {maxTouchesDetected}");
            }

            // Check if 5 or more fingers touched the screen
            if (maxTouchesDetected >= 5)
            {
                if (debugMode)
                {
                    Debug.Log("SandboxUnlockManager: 5+ finger touch detected by user!");
                }
                // Trigger unlock with sound for user input
                TriggerSandboxUnlock(true);
                return;
            }
        }
        else if (Input.touchCount == 0 && Time.time - lastTouchTime > touchTimeWindow)
        {
            // Reset touch counter after time window expires
            maxTouchesDetected = 0;
        }

        // Also check for mouse clicks on desktop (for testing)
        #if UNITY_EDITOR || UNITY_STANDALONE
        CheckMouseInput();
        #endif
    }

    #if UNITY_EDITOR || UNITY_STANDALONE
    private void CheckMouseInput()
    {
        // For desktop testing: check if multiple mouse buttons are pressed simultaneously
        // This is mainly for development/testing purposes
        int mouseButtonsPressed = 0;
        
        if (Input.GetMouseButton(0)) mouseButtonsPressed++; // Left
        if (Input.GetMouseButton(1)) mouseButtonsPressed++; // Right
        if (Input.GetMouseButton(2)) mouseButtonsPressed++; // Middle
        
        // Check additional mouse buttons if available
        for (int i = 3; i < 7; i++)
        {
            if (Input.GetMouseButton(i)) mouseButtonsPressed++;
        }

        if (mouseButtonsPressed >= 3 && debugMode) // Use 3 for testing since most mice don't have 5 buttons
        {
            Debug.Log("SandboxUnlockManager: Multiple mouse buttons detected by user (testing mode)");
            // Trigger unlock with sound for user input
            TriggerSandboxUnlock(true);
        }
    }
    #endif

    private void TriggerSandboxUnlock(bool playSound)
    {
        if (debugMode)
        {
            Debug.Log($"SandboxUnlockManager: Sandbox unlock triggered! Play sound: {playSound}");
        }

        // Play unlock sound only if requested (user interaction)
        if (playSound)
        {
            PlayUnlockSound();
        }

        // Trigger the unlock event
        OnSandboxUnlocked?.Invoke();

        // Reset input states
        keywordInput = "";
        maxTouchesDetected = 0;
    }

    private void PlayUnlockSound()
    {
        if (unlockSoundVariation != null)
        {
            unlockSoundVariation.PlayRandomOneShot(true);
            
            if (debugMode)
            {
                Debug.Log("SandboxUnlockManager: Unlock sound played via SoundEffectVariation");
            }
        }
        else if (debugMode)
        {
            Debug.LogWarning("SandboxUnlockManager: No SoundEffectVariation assigned for unlock sound");
        }
    }

    /// <summary>
    /// Reset the unlock state (useful for testing)
    /// </summary>
    public void ResetUnlockState()
    {
        keywordInput = "";
        maxTouchesDetected = 0;
        commandArgUnlockDetected = false;
        
        if (debugMode)
        {
            Debug.Log("SandboxUnlockManager: Unlock state reset");
        }
    }

    /// <summary>
    /// Check if the sandbox has been unlocked via command line argument
    /// </summary>
    public bool IsCommandLineUnlocked => commandArgUnlockDetected;
}
