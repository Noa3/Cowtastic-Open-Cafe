using System.Collections;
using UnityEngine;

/// <summary>
/// This Script will manage the Talking of the barista and send the messages to the DialogSystem
/// Events and the selection of the sentences will be managed here
/// </summary>
/// 
public class BaristaTalkManager : MonoBehaviour
{
    [Space(5)]
    [Header("References")]
    public DialogueManager DialogueManager;

    public SoundEffectVariation SoundVariationAngry;
    public SoundEffectVariation SoundVariationSigh;
    public SoundEffectVariation SoundVariationThinking;

    //[Header("Settings")]
    //public float TimeBetweenSentenceMin = 20;

    [Header("Events")]
    public bool EventGreet = true;
    public float EventOffsetEnd = 2;
    [Tooltip("How Mouch time need to be ran, until the greet message come?")]
    public float EventGreetOffsetStart = 6f;
    public float EventGreetOffsetEnd = 2;
    public bool EventGreetNewCustomer = true;
    [Tooltip("If not mouch happend, check if random idle chat should be happen")]
    public bool EventIdle = true;
    public bool EventCookie = true;
    public float EventIdleMinTime = 60;
    public float EventIdleMaxTime = 180;
    public float EventIdleRandomTime = 0;
    public bool EventCookieBuyed = true;
    public bool EventAppron = true;
    [Range(10,70)]
    public float EventApronLimit = 23;
    public bool EventTooFull = true;
    [Range(50, 100)]
    public float EventTooFullMinTime = 30;
    public float EventTooFullLimit = 90f;
    public bool EventBuyUpgrade = true;
    public float EventBuyUpgradeMinTime = 30;
    [Range(1,100)]
    public float EventBuyUpgradeTalkChance = 20;
    public bool EventResetCup = true;
    public float EventResetCupMinTime = 30;
    public bool EventPatHead = true;
    public float EventPatHeadMinTime = 10;
    public bool EventBreastMilkAdd = true;
    [Tooltip("This will be used as additional timer to have not constatly talk her about milk")]
    public int EventBreastMilkAddMin = 30;
    [Tooltip("This will be used as additional timer to have not constatly talk her about milk")]
    public int EventBreastMilkAddMax = 180;


    [Header("Etc.")]
    [ReadOnly]
    public float TimeLastSentence = 0;
    [ReadOnly]
    private float TimeScinceLevelLoad = 0;


    private bool EventAppronDone = false;
    private BaseGameMode gameMode;
    private OrderManager orderManager;
    private float timeLastCheckUpgrade = 0;
    private float timeLastCheckResetCup = 0;

    private float EventBreastMilkAddCanSpeakAgain = 0;

    public static BaristaTalkManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (DialogueManager == null)
        {
            DialogueManager = DialogueManager.instance;
        }

        gameMode = BaseGameMode.instance;
        orderManager = FindObjectOfType<OrderManager>();

        DoBaristaEventStartGame();
    }

    public void FixedUpdate()
    {
        TimeScinceLevelLoad = Time.timeSinceLevelLoad;

        //AppronEvent
        if (EventAppronDone == false && gameMode.TargetBustSize >= EventApronLimit)
        {
            Debug.Log("BarsitaTalk Appron");
            DoBaristaEventAppron();
            EventAppronDone = true;
        }

        //TooFullEvent
        if (EventTooFull == true &&
            gameMode.UpgradeCanGrow == true &&
            gameMode.Fullness > EventTooFullLimit &&
            TimeScinceLevelLoad > (TimeLastSentence + EventTooFullMinTime))
        {
            DoBaristaEventTooFull();
        }

        //Idle Chat + Cookie
        if (EventIdle == true &&
            (TimeScinceLevelLoad > (TimeLastSentence + EventIdleMaxTime) || TimeScinceLevelLoad > (TimeLastSentence + EventIdleRandomTime)) )
        {
            Debug.Log("BarsitaTalk Idle");
            if (EventCookie == true && gameMode.UpgradeCanGrow == false)
            {
                DoBaristaEventCookie();
            }
            else
            {
                DoBaristaEventIdle();
            }
        }

    }

    public void DoBaristaEventStartGame()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_StartGame_Arcade), EventGreetOffsetStart, EventGreetOffsetEnd);
    }

    public void TryBaristaEventPatHead()
    {
        if (EventPatHead == true && TimeScinceLevelLoad > (TimeLastSentence + EventPatHeadMinTime))
        {
            Debug.Log("BarsitaTalk Pat Head");

            DoBaristaEventPatHead();

        }
    }

    public void DoBaristaEventPatHead()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_PatHead), 0, EventOffsetEnd);
    }

    public void TryBaristaEventUpgrade()
    {
        if (EventBuyUpgrade == true && TimeScinceLevelLoad > (TimeLastSentence + EventBuyUpgradeMinTime))
        {

            if (TimeScinceLevelLoad > (timeLastCheckUpgrade + EventBuyUpgradeMinTime))
            {
                timeLastCheckUpgrade = Time.timeSinceLevelLoad;

                //Check if should do the evnt
                int rnd = Random.Range(0, 100);
                if (EventBuyUpgradeTalkChance > rnd)
                {
                    Debug.Log("BarsitaTalk Upgrade");
                    DoBaristaEventUpgrade();
                }
            }

        }
    }

    public void DoBaristaEventUpgrade()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_BuyUpgrade), 0, EventOffsetEnd);
    }

    public void TryBaristaEventCookieBuyed()
    {
        if (EventCookieBuyed == true)
        {
                timeLastCheckResetCup = Time.timeSinceLevelLoad;
                DoBaristaEventCookieBuyed();
        }

    }

    /// <summary>
    /// Event that got called if the inital cookoie got buyed
    /// </summary>
    public void DoBaristaEventCookieBuyed()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_CookieBuyed), 0, EventOffsetEnd);
    }

    public void TryBaristaEventCupReset()
    {
        if (EventResetCup == true && orderManager.orderIsActive == true  && TimeScinceLevelLoad > (TimeLastSentence + EventResetCupMinTime))
        {
            if (TimeScinceLevelLoad > (timeLastCheckResetCup + EventBuyUpgradeMinTime))
            {
                timeLastCheckResetCup = Time.timeSinceLevelLoad;

                DoBaristaEventCupReset();
            }
        }
    }

    public void DoBaristaEventCupReset()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_ResetCup), 0, EventOffsetEnd);
        if (SoundVariationThinking != null)
        {
            SoundVariationThinking.PlayRandomOneShot();
        }
    }

    public void TryBaristaEventCupFinished()
    {
        if ( TimeScinceLevelLoad > (TimeLastSentence + EventIdleMinTime) )
        {
            DoBaristaEventCupFinished();
        }
    }


    public void DoBaristaEventCupFinished()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_FinishCup), 0, EventOffsetEnd);
    }

    public void TryBaristaEventGreetNewCustomer()
    {
        if (TimeScinceLevelLoad > (TimeLastSentence + EventIdleMinTime))
        {
            DoBaristaEventGreetNewCustomer();
        }
    }

    public void DoBaristaEventGreetNewCustomer()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_WelcomingNewCustomer), 0, EventOffsetEnd);
    }

    public void TryBaristaEventCupAddBreastMilk()
    {
        if (TimeScinceLevelLoad > (TimeLastSentence + EventIdleMinTime) && TimeScinceLevelLoad > EventBreastMilkAddCanSpeakAgain )
        {
            SetNewEventBreasMilkTime();
            DoBaristaEventCupAddBreastMilk();
        }
    }

    private void SetNewEventBreasMilkTime()
    {
        EventBreastMilkAddCanSpeakAgain = Time.deltaTime + Statics.GetRandomRange(EventBreastMilkAddMin, EventBreastMilkAddMax);
    }

    public void DoBaristaEventCupAddBreastMilk()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_AddMilk), 0, EventOffsetEnd);
    }

    public void DoBaristaEventAppron()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_ApronLimit), 0, EventOffsetEnd);
    }

    public void DoBaristaEventCookie()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Cookie), 0, EventOffsetEnd);
    }

    public void DoBaristaEventIdle()
    {
        //First testing Standart variant of talking
        //StartBaristaTalk(GetRandomFromArray(Statics.BaristaTalk_Idle), 0, EventOffsetEnd);

        if (gameMode.Money >= 25)
        {
            int rndtmp = Statics.GetRandomRange(0,3);
            if (rndtmp == 0)
            {
                DoBaristaMoneyTalk();
            }
            else if (rndtmp == 1)
            {
                DoBaristaBustTalk();
            }
            else
            {
                DoBaristaMoodTalk();
            }
        }
        else
        {
            if (Statics.RandomBool() )
            {
                DoBaristaMoodTalk();
            }
            else
            {
                DoBaristaBustTalk();
            }
        }

    }

    public void DoBaristaMoodTalk()
    {
        if (gameMode.Happiness > 80)
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Mood_100), 0, EventOffsetEnd);
        }
        else if (gameMode.Happiness > 55)
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Mood_80), 0, EventOffsetEnd);
        }
        else if (gameMode.Happiness > 25)
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Mood_60), 0, EventOffsetEnd);
        }
        else if (gameMode.Happiness > 10)
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Mood_40), 0, EventOffsetEnd);
        }
        else
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Mood_20), 0, EventOffsetEnd);
        }
    }

    public void DoBaristaBustTalk()
    {
        if (gameMode.BustSize > 75)
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Bust_100), 0, EventOffsetEnd);
        }
        else if (gameMode.BustSize > 40)
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Bust_80), 0, EventOffsetEnd);
        }
        else if (gameMode.BustSize > 10)
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Bust_50), 0, EventOffsetEnd);
        }
        else
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Bust_20), 0, EventOffsetEnd);
        }
    }

    public void DoBaristaMoneyTalk()
    {
        if (gameMode.Money >= 100)
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Money_Above_100), 0, EventOffsetEnd);
        }
        else if (gameMode.Money >= 25)
        {
            StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_Idle_Money_Above_25), 0, EventOffsetEnd);
        }
    }


    public void DoBaristaEventTooFull()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_TooFull), 0, EventOffsetEnd);
        if (SoundVariationSigh != null)
        {
            SoundVariationSigh.PlayRandomOneShot();
        }
    }

    public void DoBaristaEventBadEnd()
    {
        StartBaristaTalk(Statics.GetRandomFromArray(Statics.BaristaTalk_BadEnd), 0, EventOffsetEnd);
    }

    public void StartBaristaTalk(DialogSentence sentence, float startOffset, float stopOffset)
    {
        SetTimeForLastSentence();
        if (startOffset > 0)
        {
            StartCoroutine(startBaristaTalk(sentence, startOffset, stopOffset));
        }
        else
        {
            startBaristaTalk(sentence, stopOffset);
        }
    }

    private IEnumerator startBaristaTalk(DialogSentence sentence, float startOffset, float stopOffset)
    {
        //StartOffset
        yield return new WaitForSeconds(startOffset);
        DialogueManager.StartDialoguebarista(sentence, stopOffset);

    }

    private void startBaristaTalk(DialogSentence sentence, float stopOffset)
    {
        DialogueManager.StartDialoguebarista(sentence, stopOffset);
    }

    public void SetTimeForLastSentence()
    {
        TimeLastSentence = Time.timeSinceLevelLoad;
        EventIdleRandomTime = Statics.GetRandomRange(EventIdleMinTime, EventIdleMaxTime);
    }

}