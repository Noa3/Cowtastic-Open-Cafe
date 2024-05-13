using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;


public class EventManager : MonoBehaviour
{
    [Header("References")]

    [Tooltip("The game object on which the icons for the events should be spawned")]
    public Transform IconHolder;

    public EventBase[] PossibileEvents;



    [Header("Condition to get a RandomEvent (Only for Random Activation)")]
    [Tooltip("Minimal time needed to play, until the first event can appear")]
    public float minTimeForFirstEvent = 30;

    [Tooltip("Minmal MaxSize of the Barista to start the events")]
    [Min(0)]
    public float minMaxBustStartEvents = 20;
    [Min(0)]
    public int CompletedCupsNeeded = 10;

    [Header("Time Between Events")]
    [Tooltip("In Seconds")]
    [Min(0)]
    public float MinTime = 60;
    [Min(1)]
    public float MaxTime = 120;



    [Header("Settings")]
    [Min(0)]
    public int MaxEventsOnSameTime = 10;

    [Header("Current Event State")]

    public List<EventBase> CurrentEvents = new List<EventBase>();

    [ReadOnly]     //Will be used to fasten the MilkProduction
    public float EventFillSpeedMultipler = 1;
    [ReadOnly]     //Will be used to calc all current events and aplay it to the game
    private float EventSellMoreValueMultipler = 1.00f;
    [ReadOnly]
    public int moreIngreedients = 0;


    [Header("Etc")]
    public bool ForceEventStart = false;


    [HideInInspector]
    public static EventManager instance;


    private OrderManager orderManager;
    private BaseGameMode gameMode;


    private float TimeLastEventEnd = 0;
    private float NextEventTime = 0;
    private float CompletedCupsSinceLastEventStart = 0;

    private BestTimeManager bestTimeManager;


    public void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        TimeLastEventEnd = Time.timeSinceLevelLoad;

        orderManager = OrderManager.instance;
        gameMode = BaseGameMode.instance;
        bestTimeManager = BestTimeManager.instance;
    }


    private void FixedUpdate()
    {
        MyFixedUpdate();
    }

    [BurstCompile]
    private void MyFixedUpdate()
    {
        if (gameMode.CurrentMaxSize >= minMaxBustStartEvents && bestTimeManager.PlayTime > minTimeForFirstEvent)
        {
            if (Time.timeSinceLevelLoad > NextEventTime || ForceEventStart == true) // Check Time
            {
                if (orderManager.CompletedCups >= (CompletedCupsSinceLastEventStart + CompletedCupsNeeded) || ForceEventStart == true) //Check min Cups
                {
                    RandomStartEvent();
                    ForceEventStart = false;
                }
            }
        }

        //New Routine ####################################
        if (CurrentEvents.Count > 0)
        {
            EventSellMoreValueMultipler = 1f;
            EventFillSpeedMultipler = 1f;
            moreIngreedients = 0;

            for (int i = 0; i < CurrentEvents.Count; i++)
            {
                EventSellMoreValueMultipler = EventSellMoreValueMultipler * CurrentEvents[i].MoreSellValue;
                EventFillSpeedMultipler = EventFillSpeedMultipler * CurrentEvents[i].MilkFillSpeedMultipler;

                string type = CurrentEvents.GetType().ToString();
                //Debug.Log(type);
                if (CurrentEvents[i].GetType() == typeof(Event_MoreIngreedients))
                {
                    Event_MoreIngreedients moreI = (Event_MoreIngreedients)CurrentEvents[i];
                    moreIngreedients = moreIngreedients + moreI.AdditionalIngreedients;
                }
            }

            orderManager.EventSellMoreValueMultipler = EventSellMoreValueMultipler;
            gameMode.EventFastMilkFillMultipler = EventFillSpeedMultipler;
            orderManager.ChangedIngreedientCount = moreIngreedients;
        }
    }

    public void EndActiveEvents()
    {

    }

    [BurstCompile]
    public void RandomStartEvent()
    {
        if (PossibileEvents != null && PossibileEvents.Length > 0)
        {

            //Set Which Event Should be started
            EventBase EventToStart= null;
            if (PossibileEvents.Count() == 1)
            {
                EventToStart = PossibileEvents[0];
            }
            else
            {
                int rnd = Mathf.RoundToInt(UnityEngine.Random.Range(0, PossibileEvents.Count()));
                EventToStart = PossibileEvents[rnd];
            }

            if (EventToStart.MinMaxBust > gameMode.CurrentMaxSize) //if event max bust is not fitting return
            {
                return;
            }

            //GoThrough Current events to prevent the wanted event?
            if (CurrentEvents.Count > 0)
            {
                for (int i = 0; i < CurrentEvents.Count; i++)
                {
                    if (CurrentEvents[i].PreventEventsWhileRunning != null && CurrentEvents[i].PreventEventsWhileRunning.Count() > 0)
                    {
                        if (CurrentEvents[i].PreventEventsWhileRunning.Length == 1)
                        {
                            //For the Forst
                            if (EventToStart.GetType() == CurrentEvents[i].PreventEventsWhileRunning[0].GetType())
                            {
                                return;
                            }
                        }
                        else
                        {
                            //For all Prevent While Running
                            for (int i2 = 0; i2 < CurrentEvents[i].PreventEventsWhileRunning.Count(); i2++)
                            {
                                if  (EventToStart.GetType() == CurrentEvents[i].PreventEventsWhileRunning[i2].GetType() )
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }


            StartEvent(EventToStart);
        }
    }

    [BurstCompile]
    public EventBase StartEvent(EventBase eventObject, bool ForceThisEvent = false)
    {
        if (eventObject == null)
        {
            return null;
        }

        if (ForceThisEvent == true && CurrentEvents.Count > 0)
        {
            for (int i = 0; i < CurrentEvents.Count; i++)
            {
                if (CurrentEvents[i].GetType() == eventObject.GetType())
                {
                    StopEvent(CurrentEvents[i]);
                    break;
                }
            }
        }

        EventBase eventBase;

        eventBase = Instantiate(eventObject, IconHolder);

        eventBase.eventManager = this;
        CurrentEvents.Add(eventBase);

        SetNextEventTime();

        return eventBase;
    }

    [BurstCompile]
    public void StopEvent(EventBase eventObject)
    {
        if (eventObject == null)
        {
            return;
        }

        if (CurrentEvents.Count > 0)
        {
            byte eventCountOfSameType = 0;
            for (int i = 0; i < CurrentEvents.Count; i++)
            {
                if (CurrentEvents[i].GetType() == eventObject.GetType())
                {
                    eventCountOfSameType++;
                    if (eventCountOfSameType > 1) //are there more events of this type?
                    {
                        break;
                    }
                }
            }

            if (eventCountOfSameType < 2) //is this the only event of this type
            {
                eventObject.SetEventState(false);
            }

            CurrentEvents.Remove(eventObject);
            Destroy(eventObject.gameObject);
        }
    }

    [BurstCompile]
    private void SetNextEventTime()
    {
        NextEventTime = Time.timeSinceLevelLoad + UnityEngine.Random.Range(MinTime, MaxTime) ;
    }

}
