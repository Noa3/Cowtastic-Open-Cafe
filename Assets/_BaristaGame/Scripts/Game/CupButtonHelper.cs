using UnityEngine;

public class CupButtonHelper : MonoBehaviour
{
    public GameObject ResetCupButton;
    public GameObject FinishOrderButton;

    CupController controller;
    OrderManager orderManager;

    private void Awake()
    {
        ResetCupButton.SetActive(false);
        FinishOrderButton.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = CupController.instance;
        orderManager = FindObjectOfType<OrderManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ResetCupButton.SetActive(controller.Fullness != 0);
        FinishOrderButton.SetActive(orderManager.orderIsActive == true);
    }
}
