using UnityEngine;

public class Event_ChangeMood : EventBase
{
    [Header("Event Setting")]
    public float MoodChange = 1;
    public float MoodChangeTickTime = 4;

    private BaseGameMode gameMode;

    public void Awake()
    {
        gameMode = BaseGameMode.instance;
    }

    //// Start is called before the first frame update
    new void Start()
    {
        base.Start();

        gameMode.EventMoodToChange = MoodChange;
        gameMode.EventMoodChangeTickTime= MoodChangeTickTime;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public override void SetEventState(bool state)
    {
        gameMode.EventMoodChange = state;
    }
}
