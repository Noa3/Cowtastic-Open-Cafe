public class Event_MoreIngreedients : EventBase
{
    //public float SellValueMultipler = 1.25f;

    //private OrderManager orderManager;

    public int AdditionalIngreedients = 1;

    private void Awake()
    {
        //orderManager = OrderManager.instance;
    }

    // Start is called before the first frame update
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

    }
}
