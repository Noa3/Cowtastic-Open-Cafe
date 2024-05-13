using UnityEngine;


/// <summary>
/// This script will check for the GameOver
/// </summary>
/// 
public class GameOver_Arcade : MonoBehaviour
{
    [Header("Settings")]
    //public string SceneNameGameOver = "GameOver";
    public GameObject GameOverCanvas;
    public GameObject GameOverFlood;

    [Tooltip("How long can she be unhappy until trigger the Gameover")]
    public float TimeUnderHappynessLimit = 5;
    [Range(0,100)]
    public float HappynessLimit = 0.2f;

    [Space(5)]
    [Tooltip("After the time is done, it will switch the scenes")]
    public float GameoverAnimationDuration = 5;

    [Space(5)]
    [Header("Flood Stiing")]
    [Range(0.2f, 0.98f)]
    public float FloodEndTrigger = 0.98f;

    [Header("Debug/Etc")]
    [ReadOnly]
    public bool GameOverTriggered = false;

    //Privates
    private BaseGameMode gameMode;
    private OrderManager orderManager;
    private float timeSinceUnderLimit = 0;

    private BaristaController baristaController;
    private BaristaTalkManager baristaTalkManager;

    private MilkWave MilkWaveObject;


    // Start is called before the first frame update
    void Start()
    {
        gameMode = BaseGameMode.instance;
        orderManager = FindObjectOfType<OrderManager>();
        baristaController = BaristaController.instance;
        baristaTalkManager = FindObjectOfType<BaristaTalkManager>();
        MilkWaveObject = FindObjectOfType<MilkWave>();
    }


    void FixedUpdate()
    {
        if(GameOverTriggered == true)
        {
            timeSinceUnderLimit += Time.fixedDeltaTime;

            if (timeSinceUnderLimit > TimeUnderHappynessLimit)
            {
                //LevelManager.ChangeScene(SceneNameGameOver);
                GameOverCanvas.SetActive(true);
            }

        }
        else
        {
            if (gameMode.Happiness < HappynessLimit)
            {
                timeSinceUnderLimit += Time.fixedDeltaTime;

                if (timeSinceUnderLimit > TimeUnderHappynessLimit)
                {
                    //LevelManager.ChangeScene(SceneNameGameOver);
                    Debug.Log("Trigger Gameover");
                    StartGameoverSequence();

                }
            }
            else
            {
                timeSinceUnderLimit = 0;
            }

            //GameoverFlood Check
            if (MilkWaveObject != null && GameOverFlood != null && MilkWaveObject.Height > FloodEndTrigger)
            {
                StartGameoverFloodSequence();
            }

        }
    }

    public void StartGameoverSequence()
    {
        GameOverTriggered = true;
        timeSinceUnderLimit = 0;
        baristaController.DoBadEnd();
        orderManager.enabled = false;
        gameMode.TargetBustSize = 0;
        baristaTalkManager.DoBaristaEventBadEnd();
    }

    public void StartGameoverFloodSequence()
    {
        timeSinceUnderLimit = 0;
        orderManager.enabled = false;
        gameMode.TargetBustSize = 0;
        GameOverFlood.SetActive(true);

        Archievements.UnlockArchievement(Archievements.ArchievementID.Make_Waves);
    }
}
