using UnityEngine;
using TMPro;

public class CheatStatsCup : MonoBehaviour
{
    [Header("Automatic References")]
    public CupController controller;
    public OrderManager orderManager;

    [Header("Text References")]
    public TextMeshProUGUI FullnessActual;
    public TextMeshProUGUI FullnessExpect;
    public TextMeshProUGUI ChocolateActual;
    public TextMeshProUGUI ChocolateExpect;
    public TextMeshProUGUI MilkActual;
    public TextMeshProUGUI MilkExpect;
    public TextMeshProUGUI TeaActual;
    public TextMeshProUGUI TeaExpect;
    public TextMeshProUGUI CreamActual;
    public TextMeshProUGUI CreamExpect;
    public TextMeshProUGUI EspressoActual;
    public TextMeshProUGUI EspressoExpect;
    public TextMeshProUGUI SugarActual;
    public TextMeshProUGUI SugarExpect;
    public TextMeshProUGUI CoffeeActual;
    public TextMeshProUGUI CoffeeExpect;
    public TextMeshProUGUI BobaActual;
    public TextMeshProUGUI BobaExpect;
    public TextMeshProUGUI IceActual;
    public TextMeshProUGUI IceExpect;
    public TextMeshProUGUI WhipedCreamActual;
    public TextMeshProUGUI WhipedCreamExpect;
    public TextMeshProUGUI CaramelSauceActual;
    public TextMeshProUGUI CaramelSauceExpect;
    public TextMeshProUGUI SprinklesActual;
    public TextMeshProUGUI SprinklesExpect;
    public TextMeshProUGUI AccuracyActual;
    public TextMeshProUGUI AccuracyExpect;
    public TextMeshProUGUI RatingActual;
    public TextMeshProUGUI RatingExpect;
    public TextMeshProUGUI MoneyActual;
    public TextMeshProUGUI MoneyExpect;
    public TextMeshProUGUI FlusteredLevel;

    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<CupController>();
        orderManager = FindObjectOfType<OrderManager>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        #region Order of Fillings
        //Chocolate 0
        //Milk 1
        //Tea 2
        //Cream 3
        //Espresso 4
        //Sugar 5
        //Coffee 6
        //Boba 7
        //Ice 8
        //WhipedCream 9
        //ChocolateSauce 10
        //CaramelSauce 11
        //Sprinkles 12
        #endregion

        if (controller != null)
        {
            FullnessActual.text = (controller.Fullness * 1).ToString();
            ChocolateActual.text = (controller.Chocolate * 100).ToString();
            MilkActual.text = (controller.Milk * 100).ToString();
            TeaActual.text = (controller.Tea * 100).ToString();
            CreamActual.text = (controller.Cream * 100).ToString();
            EspressoActual.text = (controller.Espresso * 100).ToString();
            SugarActual.text = (controller.Sugar * 100).ToString();
            CoffeeActual.text = (controller.Coffee * 100).ToString();
            BobaActual.text = controller.Boba.ToString();
            IceActual.text = controller.Ice.ToString();
            WhipedCreamActual.text = controller.WhippedCream.ToString();
            CaramelSauceActual.text = controller.CaramelSauce.ToString();
            SprinklesActual.text = controller.Sprinkles.ToString();
        }

        if (orderManager != null && orderManager.ActiveIngreedentPercentages != null && orderManager.ActiveIngreedentPercentages.Count > 9)
        {
            FullnessExpect.text = "1";
            ChocolateExpect.text = orderManager.ActiveIngreedentPercentages[0].ToString();
            MilkExpect.text = orderManager.ActiveIngreedentPercentages[1].ToString();
            TeaExpect.text = orderManager.ActiveIngreedentPercentages[2].ToString();
            CreamExpect.text = orderManager.ActiveIngreedentPercentages[3].ToString();
            EspressoExpect.text = orderManager.ActiveIngreedentPercentages[4].ToString();
            SugarExpect.text = orderManager.ActiveIngreedentPercentages[5].ToString();
            CoffeeExpect.text = orderManager.ActiveIngreedentPercentages[6].ToString();
            BobaExpect.text = orderManager.ActiveIngreedentPercentages[7].ToString();
            IceExpect.text = orderManager.ActiveIngreedentPercentages[8].ToString();
            WhipedCreamExpect.text = orderManager.ActiveIngreedentPercentages[9].ToString();
            CaramelSauceExpect.text = orderManager.ActiveIngreedentPercentages[10].ToString();
            SprinklesExpect.text = orderManager.ActiveIngreedentPercentages[11].ToString();

            float rating = orderManager.CheckIfOrderIsValid();
            AccuracyActual.text = rating.ToString();
            AccuracyExpect.text = "1";

            Rating moneyMultipler = orderManager.CalcOrderRating(rating,false);
            if (moneyMultipler != null)
            {
                RatingActual.text = moneyMultipler.MoneyMultipler.ToString();

                float moneyGain = orderManager.CalcMoneyGain(moneyMultipler.MoneyMultipler);
                MoneyActual.text = moneyGain.ToString();
            }

            RatingExpect.text = orderManager.Ratings[0].MoneyMultipler.ToString();
            MoneyExpect.text = orderManager.CalcMoneyGain(orderManager.Ratings[0].MoneyMultipler).ToString();

            FlusteredLevel.text = orderManager.CurrentFlusteredLevel.ToString();
        }
    }
}
