public class Event_FastMilkFill : EventBase
{
    //public new float MilkFillSpeedMultipler = 1f;

    private BaseGameMode gameMode;

    public void Awake()
    {
        gameMode = BaseGameMode.instance;
    }

    //// Start is called before the first frame update
    new void Start()
    {
        base.Start();


    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public override void SetEventState(bool state)
    {

        gameMode.EventFastMilkFill = state;

    }
}
