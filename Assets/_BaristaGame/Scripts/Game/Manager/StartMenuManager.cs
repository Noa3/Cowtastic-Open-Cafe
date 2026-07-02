using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Localization;

public class StartMenuManager : MonoBehaviour
{
    [Header("References")]
    public Toggle ToggleTutorial;

    public GameObject SandboxToggle;
    public GameObject HolidayToggle;

    public GameObject ChaosTimesHolder;
    public GameObject ChaosUltraTimesHolder;

    public GameObject GeneralStatsHolder; //TODO

    [Header("Sandbox Unlock")]
    [Tooltip("The SandboxUnlockManager component for special unlock methods")]
    public SandboxUnlockManager sandboxUnlockManager;
    
    [Tooltip("Sound to play when sandbox is unlocked via special method")]
    public AudioClip sandboxUnlockSound;

    [Header("References Best Times")]
    public LocalizedString StringNoRecord;
    [Space]
    public LocalizedString StringBestTimeNormal;
    public TextMeshProUGUI TextBestTimeNormal;
    public LocalizedString StringBestTimeHard;
    public TextMeshProUGUI TextBestTimeHard;

    public LocalizedString StringBestTimeCasual;
    public TextMeshProUGUI TextBestTimeCasual;
    public LocalizedString StringBestTimeChaos;
    public TextMeshProUGUI TextBestTimeChaos;
    public LocalizedString StringBestTimeUltraChaos;
    public TextMeshProUGUI TextBestTimeUltraChaos;

    public LocalizedString StringBestTimeNormalMilky;
    public TextMeshProUGUI TextBestTimeNormalMilky;
    public LocalizedString StringBestTimeHardMilky;
    public TextMeshProUGUI TextBestTimeHardMilky;

    public LocalizedString StringBestTimeCasualMilky;
    public TextMeshProUGUI TextBestTimeCasualMilky;
    public LocalizedString StringBestTimeChaosMilky;
    public TextMeshProUGUI TextBestTimeChaosMilky;
    public LocalizedString StringBestTimeUltraChaosMilky;
    public TextMeshProUGUI TextBestTimeUltraChaosMilky;

    [Header("References Stats")]
    public LocalizedString StringMilkProduced;
    public TextMeshProUGUI TextMilkProduced;
    public LocalizedString StringCupsSold;
    public TextMeshProUGUI TextCupsSold;
    public LocalizedString StringPlayTime;
    public TextMeshProUGUI TextPlayTime;
    public LocalizedString StringEarnedMoney;
    public TextMeshProUGUI TextEarnedMoney;

    [Header("Settings")]
    public string ArcadeSceneName = "Game_Arcade";
    public string TutorialSceneName = "Tutorial";


    [Header("SceneStuff")]
    [ReadOnly]
    public bool NextLevelIsTutorial = true;
    [ReadOnly]
    public bool NextLevelIsMilkyMode = false;



    public void Awake()
    {
        if (ToggleTutorial != null)
        {
            ToggleTutorial.isOn = PlayerPrefs.GetInt(Consts.PlayerPrefNextIsTutorial, 1) == 1 ? true : false;
        }

        SetNextLevelIsTutorialManager(false);
        SetNextLevelIsMilkyMode(false);

        // Apply game settings when returning to main menu
        ApplyGameSettings();

        GetStats();
        CheckToggleStatus();

        if (ChaosTimesHolder != null)
        {
            ChaosTimesHolder.SetActive(PlayerPrefs.HasKey(Consts.PlayerPrefBestTimeChaos) || PlayerPrefs.HasKey(Consts.PlayerPrefBestTimeChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix));
        }
        if (ChaosUltraTimesHolder != null)
        {
            ChaosUltraTimesHolder.SetActive(PlayerPrefs.HasKey(Consts.PlayerPrefBestTimeUltraChaos) || PlayerPrefs.HasKey(Consts.PlayerPrefBestTimeUltraChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix));
        }

        CreateLocalizationEvents();
        RefreshTexts();

        SetGeneralStatsToggleState();
        
        // Initialize sandbox unlock manager
        InitializeSandboxUnlockManager();
    }
    
    /// <summary>
    /// Initializes the SandboxUnlockManager if available
    /// </summary>
    private void InitializeSandboxUnlockManager()
    {
        // Try to find SandboxUnlockManager if not assigned
        if (sandboxUnlockManager == null)
        {
            sandboxUnlockManager = FindObjectOfType<SandboxUnlockManager>();
        }
        
        // If still null, create one
        if (sandboxUnlockManager == null)
        {
            GameObject unlockManagerGO = new GameObject("SandboxUnlockManager");
            unlockManagerGO.transform.SetParent(this.transform);
            sandboxUnlockManager = unlockManagerGO.AddComponent<SandboxUnlockManager>();
            
            // Try to add SoundEffectVariation component and assign unlock sound
            if (sandboxUnlockSound != null)
            {
                SoundEffectVariation soundVariation = unlockManagerGO.AddComponent<SoundEffectVariation>();
                AudioSource audioSource = unlockManagerGO.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = unlockManagerGO.AddComponent<AudioSource>();
                }
                
                soundVariation.effectSource = audioSource;
                soundVariation.clipArray = new AudioClip[] { sandboxUnlockSound };
                sandboxUnlockManager.unlockSoundVariation = soundVariation;
            }
            
            // Enable debug mode in editor
            #if UNITY_EDITOR
            sandboxUnlockManager.debugMode = true;
            #endif
        }
        
        // Subscribe to the unlock event
        if (sandboxUnlockManager != null)
        {
            sandboxUnlockManager.OnSandboxUnlocked += OnSpecialSandboxUnlock;
        }
    }
    
    /// <summary>
    /// Called when sandbox is unlocked via special method (5 finger touch, "unlock" keyword, or command line arg)
    /// </summary>
    private void OnSpecialSandboxUnlock()
    {
        Debug.Log("StartMenuManager: Special sandbox unlock triggered!");
        
        // Force enable the sandbox toggle
        if (SandboxToggle != null)
        {
            SandboxToggle.SetActive(true);
            
            // Unlock the sandbox achievement
            Achievements.UnlockAchievements(Achievements.AchievementsID.Sandbox_Mode);
            
            Debug.Log("StartMenuManager: Sandbox unlocked via special method!");
        }
        
        // Note: We don't save the unlock state persistently - 
        // user needs to use the gesture/keyword each time they want to unlock
    }
    
    /// <summary>
    /// Applies game settings when returning to main menu to ensure consistency
    /// Note: This preserves user's immediate choices like disabling fullscreen
    /// </summary>
    private void ApplyGameSettings()
    {
        try
        {
            // Only apply audio settings to avoid overriding user's fullscreen choice
            // Graphics settings (including fullscreen) should not be reapplied when returning to main menu
            GamePauseManager.ApplyVolumeSettingsToMusicController();
            
            Debug.Log("StartMenuManager: Audio settings applied successfully (preserving current fullscreen state)");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"StartMenuManager: Error applying game settings: {e.Message}");
        }
    }

    public void SetGeneralStatsToggleState()
    {
        if (GeneralStatsHolder == null)
        {
            return;
        }

        //double MilkProduced = 0;
        //int CupsSold = -1;
        //double playtime = -1;
        //float EarnedMoney = -1;

        //If one of the stats is recorded, then activate the stats page
        if (MilkProduced > 1 || CupsSold > 1 || playtime > 1 || EarnedMoney > 1)
        {
            GeneralStatsHolder.SetActive(true);
        }
        else
        {
            GeneralStatsHolder.SetActive(false);
        }
    }

    private void CreateLocalizationEvents()
    {
        StringNoRecord.StringChanged += TextNoRecordUpdate;
        StringBestTimeNormal.StringChanged += TextBestTimeNormalUpdate;
        StringBestTimeHard.StringChanged += TextBestTimeHardUpdate;
        StringBestTimeCasual.StringChanged += TextBestTimeCasualUpdate;
        StringBestTimeChaos.StringChanged += TextBestTimeChaosUpdate;
        StringBestTimeUltraChaos.StringChanged += TextBestTimeUltraChaosUpdate;
        StringBestTimeNormalMilky.StringChanged += TextBestTimeNormalMilkyUpdate;
        StringBestTimeHardMilky.StringChanged += TextBestTimeHardMilkyUpdate;
        StringBestTimeCasualMilky.StringChanged += TextBestTimeCasualMilkyUpdate;
        StringBestTimeChaosMilky.StringChanged += TextBestTimeChaosMilkyUpdate;
        StringBestTimeUltraChaosMilky.StringChanged += TextBestTimeUltraChaosMilkyUpdate;
        StringMilkProduced.StringChanged += TextMilkProducedUpdate;
        StringCupsSold.StringChanged += TextCupsSoldUpdate;
        StringPlayTime.StringChanged += TextPlayTimeUpdate;
        StringEarnedMoney.StringChanged += TextEarnedMoneyUpdate;
    }

    private void RefreshTexts()
    {
        StringNoRecord.RefreshString();
        StringBestTimeNormal.RefreshString();
        StringBestTimeHard.RefreshString();
        StringBestTimeCasual.RefreshString();
        StringBestTimeChaos.RefreshString();
        StringBestTimeUltraChaos.RefreshString();
        StringBestTimeNormalMilky.RefreshString();
        StringBestTimeHardMilky.RefreshString();
        StringBestTimeCasualMilky.RefreshString();
        StringBestTimeChaosMilky.RefreshString();
        StringBestTimeUltraChaosMilky.RefreshString();
        StringMilkProduced.RefreshString();
        StringCupsSold.RefreshString();
        StringPlayTime.RefreshString();
        StringEarnedMoney.RefreshString();
    }

    private void OnDisable()
    {
        StringNoRecord.StringChanged -= TextNoRecordUpdate;
        StringBestTimeNormal.StringChanged -= TextBestTimeNormalUpdate;
        StringBestTimeHard.StringChanged -= TextBestTimeHardUpdate;
        StringBestTimeCasual.StringChanged -= TextBestTimeCasualUpdate;
        StringBestTimeChaos.StringChanged -= TextBestTimeChaosUpdate;
        StringBestTimeUltraChaos.StringChanged -= TextBestTimeUltraChaosUpdate;
        StringBestTimeNormalMilky.StringChanged -= TextBestTimeNormalMilkyUpdate;
        StringBestTimeHardMilky.StringChanged -= TextBestTimeHardMilkyUpdate;
        StringBestTimeCasualMilky.StringChanged -= TextBestTimeCasualMilkyUpdate;
        StringBestTimeChaosMilky.StringChanged -= TextBestTimeChaosMilkyUpdate;
        StringBestTimeUltraChaosMilky.StringChanged -= TextBestTimeUltraChaosMilkyUpdate;
        StringMilkProduced.StringChanged -= TextMilkProducedUpdate;
        StringCupsSold.StringChanged -= TextCupsSoldUpdate;
        StringPlayTime.StringChanged -= TextPlayTimeUpdate;
        StringEarnedMoney.StringChanged -= TextEarnedMoneyUpdate;
        
        // Unsubscribe from sandbox unlock events
        if (sandboxUnlockManager != null)
        {
            sandboxUnlockManager.OnSandboxUnlocked -= OnSpecialSandboxUnlock;
        }
    }

    const string SceneNameCasual = "Game_Arcade_Casual";
    const string SceneNameNormal = "Game_Arcade";
    const string SceneNameHard = "Game_Arcade_Hard";
    const string SceneNameChaos = "Game_Arcade_Chaos";
    const string SceneNameUltraChaos = "Game_Arcade_UltraChaos";

    /// <summary>
    /// Will Check if the toggles schuld be active or not
    /// </summary>
    public void CheckToggleStatus()
    {
        //Holiday mode
        DateTime date = DateTime.Now;
        Debug.Log("Day of the week: " + date.DayOfWeek.ToString() + " || " + (date.DayOfWeek == DayOfWeek.Sunday) );
        //HolidayToggle.SetActive(date.DayOfWeek == DayOfWeek.Sunday);
        if (date.DayOfWeek == DayOfWeek.Sunday)
        {
            //HolidayToggle.SetActive(true); //Need to change here, if i want to activate the holiday mode
            Achievements.UnlockAchievements(Achievements.AchievementsID.Holiday);
        }

        if (SandboxToggle == null)
        {
            return;
        }

        // Use centralized unlock logic from SandboxUnlockManager
        bool shouldUnlock = false;
        if (sandboxUnlockManager != null)
        {
            shouldUnlock = sandboxUnlockManager.ShouldSandboxBeUnlocked();
        }
        else
        {
            // Fallback to original logic if no unlock manager is available
            bool CasualWon = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSceneWon + SceneNameCasual, false.ToString()));
            bool NormalWon = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSceneWon + SceneNameNormal, false.ToString()));
            bool HardWon = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSceneWon + SceneNameHard, false.ToString()));
            bool ChaosWon = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSceneWon + SceneNameChaos, false.ToString()));
            bool ChaosMilkyWon = (PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeUltraChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix, -1) > 0);
            bool CommandArgUnlock = !string.IsNullOrEmpty(Consts.GetArg(Consts.ARGSandboxUnlocked));

            shouldUnlock = (CasualWon && NormalWon && HardWon && ChaosWon)
                || ChaosMilkyWon
                || CommandArgUnlock;
        }

        if (shouldUnlock) 
        {
            Achievements.UnlockAchievements(Achievements.AchievementsID.Sandbox_Mode);
            SandboxToggle.SetActive(true);
        }
        else
        {
            SandboxToggle.SetActive(false);
        }
    }



    double MilkProduced = 0;
    int CupsSold = -1;
    double playtime = -1;
    float EarnedMoney = -1;
    /// <summary>
    /// For the General Stats Page in Mainmenu
    /// </summary>
    public void GetStats()
    {
        MilkProduced = double.Parse(PlayerPrefs.GetString(Consts.PlayerPrefMilkProducedOverall, "0"));
        CupsSold = PlayerPrefs.GetInt(Consts.PlayerPrefCupsSoldOverall, -1);
        playtime = double.Parse(PlayerPrefs.GetString(Consts.PlayerPrefTimePlayedOverall, "-1"));
        EarnedMoney = PlayerPrefs.GetFloat(Consts.PlayerPrefMoneyEarnedOverall, -1);
    }



    public void SetNextLevelIsTutorialManager(bool state)
    {
        NextLevelIsTutorial = state;

        PlayerPrefs.SetInt(Consts.PlayerPrefNextIsTutorial, state ? 1 : 0);
    }

    public void SetNextLevelIsMilkyMode(bool state)
    {
        NextLevelIsMilkyMode = state;

        PlayerPrefs.SetInt(Consts.PlayerPrefNextIsMilkyMode, state ? 1 : 0);
    }

    public void StartGame()
    {
        if (NextLevelIsTutorial == true)
        {
            LevelManager.ChangeScene(TutorialSceneName);
        }
        else
        {
            LevelManager.ChangeScene(ArcadeSceneName);
        }
    }

    public void TextNoRecordUpdate(string value)
    {
        Statics.TextNoRecord = value;
    }

    public void TextBestTimeNormalUpdate(string value)
    {
        //Normal
        float tmpTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeNormal, -1);
        int minutes = (int)tmpTime / 60;
        int seconds = (int)tmpTime % 60;
        if (TextBestTimeNormal != null)
        {
            if (tmpTime > 0)
            {
                TextBestTimeNormal.text = value + ": " + minutes.ToString() + ":" + seconds.ToString("00");
            }
            else
            {
                TextBestTimeNormal.text = Statics.TextNoRecord;
            }
        }
    }

    public void TextBestTimeHardUpdate(string value)
    {
        if (TextBestTimeHard != null)
        {
            //Hardmode
            float tmpTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeHard, -1);
            int minutes = (int)tmpTime / 60;
            int seconds = (int)tmpTime % 60;
            if (tmpTime > 0)
            {
                TextBestTimeHard.text = value + ": " + minutes.ToString() + ":" + seconds.ToString("00");
            }
            else
            {
                TextBestTimeHard.text = Statics.TextNoRecord;
            }
        }
    }

    public void TextBestTimeCasualUpdate(string value)
    {
        if (TextBestTimeCasual != null)
        {
            //Casual
            float tmpTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeCasual, -1);
            int minutes = (int)tmpTime / 60;
            int seconds = (int)tmpTime % 60;
            if (tmpTime > 0)
            {
                TextBestTimeCasual.text = value + ": " + minutes.ToString() + ":" + seconds.ToString("00");
            }
            else
            {
                TextBestTimeCasual.text = Statics.TextNoRecord;
            }
        }
    }

    public void TextBestTimeChaosUpdate(string value)
    {
        if (TextBestTimeChaos != null)
        {
            //Chaos
            float tmpTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeChaos, -1);
            int minutes = (int)tmpTime / 60;
            int seconds = (int)tmpTime % 60;
            if (tmpTime > 0)
            {
                TextBestTimeChaos.text = value + ": " + minutes.ToString() + ":" + seconds.ToString("00");
            }
            else
            {
                TextBestTimeChaos.text = Statics.TextNoRecord;
            }
        }
    }

    public void TextBestTimeUltraChaosUpdate(string value)
    {
        if (TextBestTimeUltraChaos != null)
        {
            //UltraChaos
            float tmpTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeUltraChaos, -1);
            int minutes = (int)tmpTime / 60;
            int seconds = (int)tmpTime % 60;
            if (tmpTime > 0)
            {
                TextBestTimeUltraChaos.text = value + ": " + minutes.ToString() + ":" + seconds.ToString("00");
            }
            else
            {
                TextBestTimeUltraChaos.text = Statics.TextNoRecord;
            }
        }

    }

    public void TextBestTimeNormalMilkyUpdate(string value)
    {
        if (TextBestTimeNormalMilky != null)
        {
            //Normal Milky
            float tmpTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeNormal + Consts.PlayerPrefBestTimeMilkymodeSuffix, -1);
            int minutes = (int)tmpTime / 60;
            int seconds = (int)tmpTime % 60;
            if (tmpTime > 0)
            {
                TextBestTimeNormalMilky.text = value + ": " + minutes.ToString() + ":" + seconds.ToString("00");
            }
            else
            {
                TextBestTimeNormalMilky.text = Statics.TextNoRecord;
            }
        }
    }

    public void TextBestTimeHardMilkyUpdate(string value)
    {
        if (TextBestTimeHardMilky != null)
        {
            //Hardmode Milky
            float tmpTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeHard + Consts.PlayerPrefBestTimeMilkymodeSuffix, -1);
            int minutes = (int)tmpTime / 60;
            int seconds = (int)tmpTime % 60;
            if (tmpTime > 0)
            {
                TextBestTimeHardMilky.text = value + ": " + minutes.ToString() + ":" + seconds.ToString("00");
            }
            else
            {
                TextBestTimeHardMilky.text = Statics.TextNoRecord;
            }
        }
    }

    public void TextBestTimeCasualMilkyUpdate(string value)
    {
        if (TextBestTimeCasualMilky != null)
        {
            //Casual Milky
            float tmpTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeCasual + Consts.PlayerPrefBestTimeMilkymodeSuffix, -1);
            int minutes = (int)tmpTime / 60;
            int seconds = (int)tmpTime % 60;
            if (tmpTime > 0)
            {
                TextBestTimeCasualMilky.text = value + ": " + minutes.ToString() + ":" + seconds.ToString("00");
            }
            else
            {
                TextBestTimeCasualMilky.text = Statics.TextNoRecord;
            }
        }
    }

    public void TextBestTimeChaosMilkyUpdate(string value)
    {
        if (TextBestTimeChaosMilky != null)
        {
            //Chaos Milky
            float tmpTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix, -1);
            int minutes = (int)tmpTime / 60;
            int seconds = (int)tmpTime % 60;
            if (tmpTime > 0)
            {
                TextBestTimeChaosMilky.text = value + ": " + minutes.ToString() + ":" + seconds.ToString("00");
            }
            else
            {
                TextBestTimeChaosMilky.text = Statics.TextNoRecord;
            }
        }
    }

    public void TextBestTimeUltraChaosMilkyUpdate(string value)
    {
        if (TextBestTimeUltraChaosMilky != null)
        {
            //UltraChaos Milky
            float tmpTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeUltraChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix, -1);
            int minutes = (int)tmpTime / 60;
            int seconds = (int)tmpTime % 60;
            if (tmpTime > 0)
            {
                TextBestTimeUltraChaosMilky.text = value + ": " + minutes.ToString() + ":" + seconds.ToString("00");
            }
            else
            {
                TextBestTimeUltraChaosMilky.text = Statics.TextNoRecord;
            }
        }
    }

    public void TextMilkProducedUpdate(string value)
    {
        if (MilkProduced < 1)
        {
            TextMilkProduced.enabled = false;
        }
        else
        {
            TextMilkProduced.text = value + String.Format("{0:0}", Math.Round(MilkProduced, 2, MidpointRounding.AwayFromZero)) + " " + Statics.ml;
        }
    }

    public void TextCupsSoldUpdate(string value)
    {
        if (TextCupsSold != null)
        {
            if (CupsSold < 1)
            {
                TextCupsSold.enabled = false;
            }
            else
            {
                TextCupsSold.text = value + CupsSold.ToString();
            }
        }
    }

    public void TextPlayTimeUpdate(string value)
    {
        if (TextPlayTime != null)
        {
            //TODO Add PlayTime and string convert
            int minutes = (int)playtime / 60;
            int seconds = (int)playtime % 60;

            if (playtime < 1)
            {
                TextPlayTime.enabled = false;
            }
            else
            {
                TextPlayTime.text = value + minutes + " : " + seconds;
            }
        }
    }

    public void TextEarnedMoneyUpdate(string value)
    {
        if (TextEarnedMoney != null)
        {
            if (EarnedMoney < 1)
            {
                TextEarnedMoney.enabled = false;
            }
            else
            {
                TextEarnedMoney.text = value + (Mathf.Round(EarnedMoney * 100) / 100).ToString("0.00") + " " + Statics.CurrencySymbol;
            }
        }
    }

}
