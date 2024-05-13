public class Event_NotFlusteredCustomers : EventBase
{
    private OrderManager orderManager;

    public void Awake()
    {
        orderManager = OrderManager.instance;
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();


    }

    // Update is called once per frame
    //void Update()
    //{
    //    base.Update();
    //}


    public override void SetEventState(bool state)
    {
        orderManager.EventCustomerNotFlustered = state;
    }
}
