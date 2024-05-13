using UnityEngine;

public class GameOver_Holiday : MonoBehaviour
{
    [Header("References")]
    public GameObject GameOverCanvas;
    public GameObject GameOverFlood;

    [Space(5)]
    [Header("Flood Stiing")]
    [Range(0.2f, 0.98f)]
    public float FloodEndTrigger = 0.98f;

    //Privates
    private MilkWave MilkWaveObject;
    private BaseGameMode gameMode;
    private OrderManager orderManager;
    private BaristaController baristaController;
    private BaristaTalkManager baristaTalkManager;
    private float timeSinceUnderLimit = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameMode = BaseGameMode.instance;
        orderManager = FindObjectOfType<OrderManager>();
        baristaController = BaristaController.instance;
        baristaTalkManager = FindObjectOfType<BaristaTalkManager>();
        MilkWaveObject = FindObjectOfType<MilkWave>();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void StartGameoverFloodSequence()
    {
        timeSinceUnderLimit = 0;
        orderManager.enabled = false;
        gameMode.TargetBustSize = 0;
        GameOverFlood.SetActive(true);
    }
}
