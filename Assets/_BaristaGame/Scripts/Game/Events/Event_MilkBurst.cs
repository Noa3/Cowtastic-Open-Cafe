using UnityEngine;

public class Event_MilkBurst : EventBase
{
    [Header("Event Setting")]
    [Tooltip("How much seconds need to be gone until a Burst Ticks")]
    public float MilkBurstTimeUntilTick = 5;
    [Tooltip("How much milk and bust will be added per Tick")]
    public float MilkBurstGrowPerTick = 2;

    private BaseGameMode gameMode;

    public void Awake()
    {
        gameMode = BaseGameMode.instance;
    }

    //// Start is called before the first frame update
    new void Start()
    {
        base.Start();

        gameMode.EventMilkBurstGrowTickTime = MilkBurstTimeUntilTick;
        gameMode.EventMilkBurstGrowTickSize= MilkBurstGrowPerTick;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public override void SetEventState(bool state)
    {
        gameMode.EventMilkBurst = state;
    }
}
