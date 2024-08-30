using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// For the Good End it is similar, there is a GoodEnd trigger in the animator that should activate when the win condition is met
/// and whatever we do to show the results screen should wait for the animator event at the end
/// of that animation.
/// This one also expects her fullness to be paused and lower to 0 while the animation happens, over 10.5 seconds this time.
/// The intention here is for her to be spraying milk.
/// </summary>


public class GameWinManager : MonoBehaviour
{
    [Header("References")]
    public GameObject GameWinCanvas;

    public TextMeshProUGUI ActualTime;
    public TextMeshProUGUI BestTime;

    public TextMeshProUGUI ActualEarned;
    public TextMeshProUGUI BestEarned;

    public TextMeshProUGUI ActualCustomers;
    public TextMeshProUGUI BestCustomers;

    public TextMeshProUGUI ActualMilk;
    public TextMeshProUGUI BestMilk;

    public GameObject[] ObjectsToDeactivateWhileAnimating;

    [Space()]
    [Header("Win Condition")]
    [Range(10, 999.8f)][Tooltip("Min the Max Bustssize of the Barista hould be to count as win")]
    public float NeededMaxBustSize = 90;

    [Range(10, 999.8f)][Tooltip("Min of her Actual Bust size needed to count as win")]
    public float NeededActualBustSize = 80;

    [Space()]
    [Header("Settings")]
    public float BaristaWonAnimationTime = 13;
    public float TimeUntilCanvasGotShown = 15;

    [Header("Debug/Etc")]
    [ReadOnly]
    public bool GameWon = false;
    [ReadOnly]
    public bool PlayedWinAnimtion = false;

    [ReadOnly]
    public float MoneyBest = 0;
    [ReadOnly]
    public int CustomerServedBest = 0;
    [ReadOnly]
    public double MilkBest = 0;
    [ReadOnly]
    public float CurrentBestTime = 0; //This is the besttime on starting the winning squence

    private float TimeStartWinAnimation = 0;

    private BaseGameMode gameMode;
    private BaristaController baristaController;
    private BestTimeManager timeManager;
    private StatisticsHolder statisticsHolder;

    private GameOver_Arcade GameOverArcade;
    private MilkWave milkWave;

    private void Awake()
    {
        GameWinCanvas.SetActive(false);

        MoneyBest = PlayerPrefs.GetFloat(Consts.PlayerPrefMostEarned, 0);
        CustomerServedBest = PlayerPrefs.GetInt(Consts.PlayerPrefMostServed, 0);
        MilkBest = double.Parse(PlayerPrefs.GetString(Consts.PlayerPrefMostMilk, "0"));
    }

    // Start is called before the first frame update
    void Start()
    {
        baristaController = BaristaController.instance;
        timeManager = BestTimeManager.instance;
        gameMode = BaseGameMode.instance;
        statisticsHolder = StatisticsHolder.instance;

        GameOverArcade = FindObjectOfType<GameOver_Arcade>();
        milkWave = MilkWave.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameWon == false)
        {
            if (gameMode.CurrentMaxSize >= NeededMaxBustSize && (gameMode.BustSize - gameMode.EventMilkBurstBustExpanded) >= NeededActualBustSize)
            {
                GameWon = true;
                //ShowWinScreen();
                GameOverArcade.enabled = false;
                milkWave.GameWon = true;

                CurrentBestTime = timeManager.PlayTime;

                TimeStartWinAnimation = Time.timeSinceLevelLoad;
                baristaController.DoGoodEnd();
                gameMode.GameEndSequence = true;
                gameMode.GameEndSequenceDuration = BaristaWonAnimationTime;
                //GameWonEffect?.PlayFeedbacks();

                DeactivateOtherObjects(false);
            }
        }
        else
        {
            if (PlayedWinAnimtion == false)
            {
                if (Time.timeSinceLevelLoad > (TimeStartWinAnimation + TimeUntilCanvasGotShown) ) //If is Animation Time is Over
                {
                    PlayedWinAnimtion = true;
                    ShowWinScreen();
                }
            }
        }
    }

    private void DeactivateOtherObjects(bool active = false)
    {
        for (int i = 0; i < ObjectsToDeactivateWhileAnimating.Length; i++)
        {
            ObjectsToDeactivateWhileAnimating[i].SetActive(active);
        }
    }

    public void ShowWinScreen()
    {
        Time.timeScale = 0;
        GameWinCanvas.SetActive(true);

        #region Time
        float time = CurrentBestTime;
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;

        ActualTime.text = Statics.TextActualTime + ": " + minutes.ToString() + ":" + seconds.ToString("00");

        if (timeManager.BestTime < CurrentBestTime) // if Best Time faster/under than Playtime
        {
            time = timeManager.BestTime;
            minutes = (int)time / 60;
            seconds = (int)time % 60;
        }
        else
        {
            timeManager.SaveBestTime();
        }

        BestTime.text = Statics.TextBestTime + ": " + minutes.ToString() + ":" + seconds.ToString("00");
        #endregion

        #region Earned Money
        ActualEarned.text = Statics.TextMoneyEarned + ": " + (Mathf.Round(statisticsHolder.MoneyEarned * 100) / 100).ToString() ;
        if (MoneyBest > statisticsHolder.MoneyEarned) //If less money earned
        {
            BestEarned.text = Statics.TextMostEarned + ": " + (Mathf.Round(MoneyBest * 100) / 100).ToString() ;
        }
        else //more money than record earned
        {
            BestEarned.text = Statics.TextMostEarned + ": " + (Mathf.Round(statisticsHolder.MoneyEarned * 100) / 100).ToString();
            PlayerPrefs.SetFloat(Consts.PlayerPrefMostEarned, statisticsHolder.MoneyEarned);
        }
        #endregion

        #region Served Customers

        ActualCustomers.text = Statics.TextCustomerServed + ": " + statisticsHolder.CupsSold;
        if (CustomerServedBest > statisticsHolder.CupsSold)
        {

            BestCustomers.text = Statics.TextMostServed + ": " + CustomerServedBest;
        }
        else
        {

            BestCustomers.text = Statics.TextMostServed + ": " + statisticsHolder.CupsSold;
            PlayerPrefs.SetFloat(Consts.PlayerPrefMostServed, statisticsHolder.CupsSold);
        }


        #endregion
        #region Milk Produced

        ActualMilk.text = Statics.TextMilkCreated + ": " + String.Format("{0:0}", statisticsHolder.MilkCreated) + "ml";

        if (MilkBest > statisticsHolder.MilkCreated)
        {
            BestMilk.text = Statics.TextMostMilk + ": " + String.Format("{0:0}", MilkBest) + "ml";
        }
        else
        {

            BestMilk.text = Statics.TextMostMilk + ": " + String.Format("{0:0}", statisticsHolder.MilkCreated) + "ml";

            PlayerPrefs.SetString(Consts.PlayerPrefMostMilk, statisticsHolder.MilkCreated.ToString() );
        }

        #endregion

        SaveSceneWon();

        KeyBindingManager.instance.Paused();
    }


    public void KeepPlayingPressed()
    {
        RestoreOrginalValues();
        KeyBindingManager.instance.UnPaused();
    }

    public void RestoreOrginalValues()
    {
        DeactivateOtherObjects(true);
        Time.timeScale = 1;
        GameWinCanvas.SetActive(false);
        gameMode.GameEndSequence = false;
    }


    public void SaveSceneWon()
    {
        PlayerPrefs.SetString(Consts.PlayerPrefSceneWon + SceneManager.GetActiveScene().name,true.ToString());
        SaveStatistics();
    }

    public void SaveStatistics()
    {
        statisticsHolder.SaveValues();
    }

}
