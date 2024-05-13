using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;

public class BestTimeManager : MonoBehaviour
{
    [Header("References")]

    public GameObject BestTimeHolder;

    public TextMeshProUGUI TimeText;

    GameMode CurrentMode = GameMode.Normal;

    public LocalizedString StringTextTime;

    [Header("Debug")]

    [ReadOnly]
    public float PlayTime = 0;

    [ReadOnly]
    public bool IsNewBestTime = false;

    [ReadOnly]
    public float BestTime = 99999999;

    private bool isMilkyMode;

    private const string SceneNameNormal = "Game_Arcade";
    private const string SceneNameHard = "Game_Arcade_Hard";

    private const string SceneNameCasual = "Game_Arcade_Casual";
    private const string SceneNameChaos = "Game_Arcade_Chaos";
    private const string SceneNameChaosUltra = "Game_Arcade_UltraChaos";

    private const string SceneNoasMod = "Game_Arcade_Holiday";

    public static BestTimeManager instance;

    private void Awake()
    {
        instance = this;

        isMilkyMode = PlayerPrefs.GetInt(Consts.PlayerPrefNextIsMilkyMode, 0) == 1 ? true : false;

        if (isMilkyMode == true) // Milkymode
        {
            //CurrentMode = GameMode.MilkyMode;
            //BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeMilkymode, 99999999);

            if (SceneManager.GetActiveScene().name == SceneNameNormal) // Normal
            {
                CurrentMode = GameMode.NormalMilky;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeNormal + Consts.PlayerPrefBestTimeMilkymodeSuffix, 99999999);
            }
            else if (SceneManager.GetActiveScene().name == SceneNameHard) // Hard
            {
                CurrentMode = GameMode.HardMilky;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeHard + Consts.PlayerPrefBestTimeMilkymodeSuffix, 99999999);
            }
            else if (SceneManager.GetActiveScene().name == SceneNameCasual) // Casual
            {
                CurrentMode = GameMode.CasualMilky;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeCasual + Consts.PlayerPrefBestTimeMilkymodeSuffix, 99999999);
            }
            else if (SceneManager.GetActiveScene().name == SceneNameChaos) // Chaos
            {
                CurrentMode = GameMode.ChaosMilky;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix, 99999999);
            }
            else if (SceneManager.GetActiveScene().name == SceneNameChaosUltra) // UltraChaos
            {
                CurrentMode = GameMode.UltraChaosMilky;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeUltraChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix, 99999999);
            }

        }
        else
        {
            if (SceneManager.GetActiveScene().name == SceneNameNormal) // Normal
            {
                CurrentMode = GameMode.Normal;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeNormal, 99999999);
            }
            else if (SceneManager.GetActiveScene().name == SceneNameHard) // Hard
            {
                CurrentMode = GameMode.Hard;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeHard, 99999999);
            }
            else if (SceneManager.GetActiveScene().name == SceneNameCasual) // Casual
            {
                CurrentMode = GameMode.Casual;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeCasual, 99999999);
            }
            else if (SceneManager.GetActiveScene().name == SceneNameChaos) // Chaos
            {
                CurrentMode = GameMode.Chaos;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeChaos, 99999999);
            }
            else if (SceneManager.GetActiveScene().name == SceneNameChaosUltra) // UltraChaos
            {
                CurrentMode = GameMode.UltraChaos;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeUltraChaos, 99999999);
            }
            else if (SceneManager.GetActiveScene().name == SceneNoasMod) // NoasMod
            {
                CurrentMode = GameMode.NoasMod;
                BestTime = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeNoasMod, 99999999);
            }
        }

        CreateLocalizationEvents();
    }

    void CreateLocalizationEvents()
    {
        StringTextTime.StringChanged += TextTimeUpdate;
    }

    public void TextTimeUpdate(string value)
    {
        Statics.TextTime = value;
    }

    // Update is called once per frame
    void Update()
    {
        PlayTime = PlayTime + (Time.deltaTime * Time.timeScale);
        SetTimeText(PlayTime);
    }


    private void SetTimeText(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;

        TimeText.text = Statics.TextTime + ": " + minutes.ToString() + ":" + seconds.ToString("00");
    }

    /// <summary>
    /// Saves the Actual Time as BestTime
    /// </summary>
    public void SaveBestTime()
    {
        switch (CurrentMode)
        {
            case GameMode.Normal:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeNormal, PlayTime);
                break;
            case GameMode.Hard:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeHard, PlayTime);
                break;
            case GameMode.Casual:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeCasual, PlayTime);
                break;
            case GameMode.Chaos:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeChaos, PlayTime);
                break;
            case GameMode.UltraChaos:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeUltraChaos, PlayTime);
                break;
            case GameMode.NoasMod:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeNoasMod, PlayTime);
                break;
            case GameMode.NormalMilky:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeNormal + Consts.PlayerPrefBestTimeMilkymodeSuffix, PlayTime);
                break;
            case GameMode.HardMilky:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeHard + Consts.PlayerPrefBestTimeMilkymodeSuffix, PlayTime);
                break;
            case GameMode.CasualMilky:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeCasual + Consts.PlayerPrefBestTimeMilkymodeSuffix, PlayTime);
                break;
            case GameMode.ChaosMilky:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix, PlayTime);
                break;
            case GameMode.UltraChaosMilky:
                PlayerPrefs.SetFloat(Consts.PlayerPrefBestTimeUltraChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix, PlayTime);
                break;

            default:
                break;
        }

        PlayerPrefs.Save();
    }

    //private void OnApplicationQuit()
    //{
    //    SaveBestTime();
    //}
}

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
    UltraChaosMilky,
}
