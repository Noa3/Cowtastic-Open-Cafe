using UnityEngine;
using UnityEngine.UI;

public class EventBase : MonoBehaviour
{
    [Header("References")]
    public Image FillingTimerCircle;


    [Header("Condition to allow Activation")]
    [Min(0)]
    public float MinMaxBust = 8;



    [Header("Setting")]

    public float MinRunTime = 15;
    public float MaxRunTime = 30;

    public bool RunningUnlimitedAfterStart = false;

    public EventBase[] PreventEventsWhileRunning;

    [Header("Etc Setting")]
    public float MoreSellValue = 1f;
    public float MilkFillSpeedMultipler = 1f;



    private float TimeEventStart = 0;
    private float TimeEventEnd = 0; //the time when the event will end

    [HideInInspector]
    public EventManager eventManager;

    // Start is called before the first frame update
    public void Start()
    {
        SetEventState(true);

        TimeEventStart = Time.timeSinceLevelLoad;
        SetEventEndTime(MinRunTime, MaxRunTime);
    }

    // Update is called once per frame
    public void Update()
    {
        if (RunningUnlimitedAfterStart == true)
        {
            FillingTimerCircle.fillAmount = 0;
        }
        else
        {
            FillingTimerCircle.fillAmount = (Time.timeSinceLevelLoad - TimeEventStart) / (TimeEventEnd - TimeEventStart);
        }

    }

    public void FixedUpdate()
    {
        if (RunningUnlimitedAfterStart == false && Time.timeSinceLevelLoad > TimeEventEnd)
        {
            EndEvent();
        }
    }

    public void EndEvent()
    {
        if (eventManager != null)
        {
            eventManager.StopEvent(this);
        }
    }

    /// <summary>
    /// This will be used mainly by the event logic and activates, deactivates the script routines
    /// </summary>
    /// <param name="state"></param>
    public virtual void SetEventState(bool state)
    {

    }

    public void SetEventEndTime(float min, float max)
    {
        float curLevelTime = Time.timeSinceLevelLoad;
        TimeEventEnd = Statics.GetRandomRange(curLevelTime + min, curLevelTime + max, Statics.EventDurationRNG());
    }


}
