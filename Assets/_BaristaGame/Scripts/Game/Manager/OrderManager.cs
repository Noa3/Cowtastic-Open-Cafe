using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Web;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Localization;

public class OrderManager : MonoBehaviour
{
    [Header("References")]
    public LocalizedString StringCustomerDialogDrinkTypeCoffee;
    public LocalizedString CustomerDialogDrinkTypeMilk;
    public LocalizedString CustomerDialogDrinkTypeEspresso;
    public LocalizedString CustomerDialogDrinkTypeTea;
    public LocalizedString CustomerDialogDrinkTypeCream;
    public LocalizedString CustomerDialogDrinkTypeChocolate;

    [Header("Settings")]
    public float WaitBetweenOrderMax = 15;

    [Tooltip("This value will be multiplied with the wrong amound of Toppungs on the cup")]
    [Range(0,1)]
    public float PunishingMultiplerForWrongIngreedient = 0.45f;
    [Tooltip("If is on active order given a empty cup to the customer")]
    [Min(0)]
    public float PunishingValueForEmptyCupInOrder = 2;

    [Range(0.01f,0.99f)]
    public float FillingSuccecsTollerance = 0.70f;
    public IngreedientCosts IngredientValue;

    [Tooltip("Time until the cup gets reset after the press on 'Finish Order'")]
    public float FinishedOrderCupResetOffset = 0.5f;

    [Space(2)]
    public Customers[] customers;
    public List<CustomerAvatar> RandomCustomAvatars;
    public PossibileOrder[] PossibileWeightedOrders; //This will be used for random generation of the orders 
    public FillingPercentage PossibileFillingPercentages;


    public float[] IngredientsMultiplier = new float[] {0, 0.25f, 0.5f, 0.75f, 1, 1.25f, 1.5f, 1.75f};
    public Rating[] Ratings;

    [Space(2)]
    public bool MilkingIncreasesFlustered = true;
    [Tooltip("is it needed to be Flustered allready to increase the levek")]
    public bool NeedBeFlusteredToIncreaseLevel = false;
    [Tooltip("How mouch time should she be milked to increase a flustered stage")]
    public float MilkTimeToIncreaseFlustered = 2;

    [Tooltip("This are the values of much of the drink will be replaced with the Breastmilk for corrosponding Flustered Level")]
    public BreastMilkPercentages PercentagesReplacingBreastMilk;

    [Header("Events")]
    [ReadOnly]
    public bool EventAlwaysFlustered = false;
    public bool EventMoreValueSell = false;
    [HideInInspector][ReadOnly]
    public float EventSellMoreValueMultipler = 1.25f;
    public bool EventCustomerNotFlustered = false;
    [ReadOnly]
    public int ChangedIngreedientCount = 0;

    [Header("Etc./Info")]
    [ReadOnly]
    public int CompletedCups = 0;
    [ReadOnly]
    public bool orderIsActive = false;
    [ReadOnly]
    public Customers ActiveCustomer;
    [ReadOnly]
    public CustomerAvatar ActiveCustomerAvatar;
    [ReadOnly]
    public List<float> ActiveIngreedentPercentages = new List<float>();
    [ReadOnly][Range(0,100)]
    public float FlusteredChance = 0;

    public bool CustomerIsFlustered
    {
        get { return (CurrentFlusteredLevel > 0); }
    }

    [ReadOnly][Range(0,4)]
    public int CurrentFlusteredLevel = 0; //Level 0 disabled ,Level 1 = 25, 2 = 50, 3 = 75, 4 = 100

    [Range(0,100f)][Min(0)]
    public float FlusteredLevelActivationMin1 = 30f;
    [Range(0, 100f)]
    public float FlusteredLevelActivationMin2 = 45f;
    [Range(0, 100f)]
    public float FlusteredLevelActivationMin3 = 65f;
    [Range(0, 100f)]
    public float FlusteredLevelActivationMin4 = 78f;

    [Min(0)]
    public float FlusteredRandomMultiplerMin = 0.5f;
    public float FlusteredRandomMultiplerMax = 1.5f;

    [Header("Debug")]
    public bool isAlwaysFlustered = false;

    private DialogueManager dialogeManager;
    private BaseGameMode gamemode;
    private CupController cupController;
    private float TimeLastOrder = 0;


    [ReadOnly]
    private List<Toppings> UnlockedToppings = new List<Toppings>();
    [ReadOnly]
    private List<Fillings> UnlockedFillings = new List<Fillings>();

    private FillingTool[] fillingTools;
    private RatingAnimationHelper ratingAnimationHelper;
    private SoundEffectManager soundEffectManager;

    private float milkedTime = 0;
    private Animator CustomerDialogAnimator;

    private EventManager eventManager;
    private StatisticsHolder statisticsHolder;

    private EventBase CurrentAvatarEvent = null;
    private BaristaTalkManager baristaTalkManager;

    public static OrderManager instance;

    public void Awake()
    {
        instance = this;


        CreateLocalizationEvents();
    }


    void CreateLocalizationEvents()
    {
        foreach (Rating item in Ratings)
        {
            item.StringRatingText.StringChanged += item.SetName;
        }
    }

    private void Start()
    {
        dialogeManager = DialogueManager.instance;
        gamemode = BaseGameMode.instance;
        cupController = CupController.instance;
        ratingAnimationHelper = RatingAnimationHelper.instance;
        soundEffectManager = SoundEffectManager.instance;
        TimeLastOrder = Time.time;
        CustomerDialogAnimator = gamemode.CustomerDialogAnimator;
        eventManager = EventManager.instance;
        statisticsHolder = StatisticsHolder.instance;
        baristaTalkManager = BaristaTalkManager.instance;
    }

    private void FixedUpdate()
    {
        MyFixedUpdate();
    }

    [BurstCompile]
    private void MyFixedUpdate()
    {
        if (orderIsActive == false)
        {
            if (Time.time > (TimeLastOrder + WaitBetweenOrderMax))
            {
                StartOrder();
            }
        }
        else
        {
            if (CurrentFlusteredLevel < 4)
            {
                if (gamemode.BeeingMilked == true)
                {
                    milkedTime += Time.fixedDeltaTime;
                }

                if (milkedTime > MilkTimeToIncreaseFlustered)
                {
                    milkedTime = 0;

                    TryIncreaseFlusteredLevel();

                }
            }
        }
    }

    [BurstCompile]
    public void StartOrder(Customers custom = null)
    {
        if (orderIsActive == true)
        {
            return;
        }

        //GetUnlockedIngreediens(); //WrongPlace

        soundEffectManager.PlayNewOrderEffect();

        orderIsActive = true;
        ActiveCustomer = custom;


        //Need the Fulstered Value Before Generate the CUstomer because the text Generation
        //CurrentFlusteredLevel = CalcFlusteredLevel();


        if (ActiveCustomer == null)
        {
            //ActiveCustomer = customers[Random.Range(0, customers.Length)]; //Need to be reimplemented maybe again

            ActiveCustomer = GenerateCustomer();
            ActiveCustomer.name = ActiveCustomer.Avatar.name;

            if (ActiveCustomer == null)
            {
                Debug.Log("Couldnt Start Order, no Active Customer Possibile");
                return;
            }
        }

        //Build random Fill Value
        ActiveIngreedentPercentages = CreateRandomFillValue(ActiveCustomer);
        dialogeManager.StartDialogue(ActiveCustomer, ActiveIngreedentPercentages, CurrentFlusteredLevel, true);

        //Barista Talk
        baristaTalkManager.TryBaristaEventGreetNewCustomer();

        statisticsHolder.AddSeenCustomer(ActiveCustomer.name);
    }


    public List<float> CreateRandomFillValue(Customers customer)
    {
        #region Order of Fillings
        //Chocolate
        //Milk
        //Tea
        //Cream
        //Espresso
        //Sugar
        //Coffee
        //Boba
        //Ice
        //WhipedCream
        //ChocolateSauce
        //CaramelSauce
        //Sprinkles
        //BreastMilk
        #endregion

        List<float> RetVal = new List<float>();

        List<Fillings> fillings = new List<Fillings>(customer.OrderFillings);
        int countFillings = fillings.Count;

        List<float> fillingValues = new List<float>();

        int rndcnt = 0;
        switch (countFillings)
        {
            case 1:
                fillingValues.Add(100);
                break;
            case 2:
                rndcnt = Statics.GetRandomRange(0, PossibileFillingPercentages.twoFillings.Length - 1);
                fillingValues.Add(PossibileFillingPercentages.twoFillings[rndcnt].value1);
                fillingValues.Add(PossibileFillingPercentages.twoFillings[rndcnt].value2);
                break;
            case 3:
                rndcnt = Statics.GetRandomRange(0, PossibileFillingPercentages.threeFillings.Length - 1);
                fillingValues.Add(PossibileFillingPercentages.threeFillings[rndcnt].value1);
                fillingValues.Add(PossibileFillingPercentages.threeFillings[rndcnt].value2);
                fillingValues.Add(PossibileFillingPercentages.threeFillings[rndcnt].value3);
                break;
            case 4:
                rndcnt = Statics.GetRandomRange(0, PossibileFillingPercentages.fourFillings.Length - 1);
                fillingValues.Add(PossibileFillingPercentages.fourFillings[rndcnt].value1);
                fillingValues.Add(PossibileFillingPercentages.fourFillings[rndcnt].value2);
                fillingValues.Add(PossibileFillingPercentages.fourFillings[rndcnt].value3);
                fillingValues.Add(PossibileFillingPercentages.fourFillings[rndcnt].value4);
                break;
            case 5:
                rndcnt = Statics.GetRandomRange(0, PossibileFillingPercentages.fiveFillings.Length - 1);
                fillingValues.Add(PossibileFillingPercentages.fiveFillings[rndcnt].value1);
                fillingValues.Add(PossibileFillingPercentages.fiveFillings[rndcnt].value2);
                fillingValues.Add(PossibileFillingPercentages.fiveFillings[rndcnt].value3);
                fillingValues.Add(PossibileFillingPercentages.fiveFillings[rndcnt].value4);
                fillingValues.Add(PossibileFillingPercentages.fiveFillings[rndcnt].value5);
                break;
            case 6:
                rndcnt = Statics.GetRandomRange(0, PossibileFillingPercentages.sixFillings.Length - 1);
                fillingValues.Add(PossibileFillingPercentages.sixFillings[rndcnt].value1);
                fillingValues.Add(PossibileFillingPercentages.sixFillings[rndcnt].value2);
                fillingValues.Add(PossibileFillingPercentages.sixFillings[rndcnt].value3);
                fillingValues.Add(PossibileFillingPercentages.sixFillings[rndcnt].value4);
                fillingValues.Add(PossibileFillingPercentages.sixFillings[rndcnt].value5);
                fillingValues.Add(PossibileFillingPercentages.sixFillings[rndcnt].value6);
                break;
            case 7:
                rndcnt = Statics.GetRandomRange(0, PossibileFillingPercentages.sevenFillings.Length - 1);
                fillingValues.Add(PossibileFillingPercentages.sevenFillings[rndcnt].value1);
                fillingValues.Add(PossibileFillingPercentages.sevenFillings[rndcnt].value2);
                fillingValues.Add(PossibileFillingPercentages.sevenFillings[rndcnt].value3);
                fillingValues.Add(PossibileFillingPercentages.sevenFillings[rndcnt].value4);
                fillingValues.Add(PossibileFillingPercentages.sevenFillings[rndcnt].value5);
                fillingValues.Add(PossibileFillingPercentages.sevenFillings[rndcnt].value6);
                fillingValues.Add(PossibileFillingPercentages.sevenFillings[rndcnt].value7);
                break;
            case 8:
                rndcnt = Statics.GetRandomRange(0, PossibileFillingPercentages.eightFillings.Length - 1);
                fillingValues.Add(PossibileFillingPercentages.eightFillings[rndcnt].value1);
                fillingValues.Add(PossibileFillingPercentages.eightFillings[rndcnt].value2);
                fillingValues.Add(PossibileFillingPercentages.eightFillings[rndcnt].value3);
                fillingValues.Add(PossibileFillingPercentages.eightFillings[rndcnt].value4);
                fillingValues.Add(PossibileFillingPercentages.eightFillings[rndcnt].value5);
                fillingValues.Add(PossibileFillingPercentages.eightFillings[rndcnt].value6);
                fillingValues.Add(PossibileFillingPercentages.eightFillings[rndcnt].value7);
                fillingValues.Add(PossibileFillingPercentages.eightFillings[rndcnt].value8);
                break;
        }

        int fillcnt = 0;
        if (fillings.Contains(Fillings.Chocolate) == true)
        {
            RetVal.Add(fillingValues[fillcnt]);
            fillcnt++;
        }
        else
        {
            RetVal.Add(0);
        }
        if (fillings.Contains(Fillings.Milk) == true)
        {
            RetVal.Add(fillingValues[fillcnt]);
            fillcnt++;
        }
        else
        {
            RetVal.Add(0);
        }
        if (fillings.Contains(Fillings.Tea) == true)
        {
            RetVal.Add(fillingValues[fillcnt]);
            fillcnt++;
        }
        else
        {
            RetVal.Add(0);
        }
        if (fillings.Contains(Fillings.Cream) == true)
        {
            RetVal.Add(fillingValues[fillcnt]);
            fillcnt++;
        }
        else
        {
            RetVal.Add(0);
        }
        if (fillings.Contains(Fillings.Espresso) == true)
        {
            RetVal.Add(fillingValues[fillcnt]);
            fillcnt++;
        }
        else
        {
            RetVal.Add(0);
        }
        if (fillings.Contains(Fillings.Sugar) == true)
        {
            RetVal.Add(fillingValues[fillcnt]);
            fillcnt++;
        }
        else
        {
            RetVal.Add(0);
        }
        if (fillings.Contains(Fillings.Coffee) == true)
        {
            RetVal.Add(fillingValues[fillcnt]);
            fillcnt++;
        }
        else
        {
            RetVal.Add(0);
        }

        List<Toppings> toppings = new List<Toppings>(customer.Toppings);

        if (toppings.Contains(Toppings.Boba) == true)
        {
            RetVal.Add(1);
        }
        else
        {
            RetVal.Add(0);
        }
        if (toppings.Contains(Toppings.Ice) == true)
        {
            RetVal.Add(1);
        }
        else
        {
            RetVal.Add(0);
        }
        if (toppings.Contains(Toppings.WhipedCream) == true)
        {
            RetVal.Add(1);
        }
        else
        {
            RetVal.Add(0);
        }
        if (toppings.Contains(Toppings.ChocolateSauce) == true)
        {
            RetVal.Add(1);
        }
        else
        {
            RetVal.Add(0);
        }
        if (toppings.Contains(Toppings.CaramelSauce) == true)
        {
            RetVal.Add(1);
        }
        else
        {
            RetVal.Add(0);
        }
        if (toppings.Contains(Toppings.Sprinkles) == true)
        {
            RetVal.Add(1);
        }
        else
        {
            RetVal.Add(0);
        }

        //BreastMilk
        if (fillings.Contains(Fillings.BreastMilk) == true)
        {
            RetVal.Add(fillingValues[fillcnt]);
            fillcnt++;
        }
        else
        {
            RetVal.Add(0);
        }


        RetVal = OverrideMilkFillingLogic(RetVal, fillings);

        return RetVal;
    }

    /// <summary>
    /// Increases on a Active order the Flustered Level
    /// </summary>
    [BurstCompile]
    public void TryIncreaseFlusteredLevel()
    {
        Debug.Log("Try to Increase Flustered Level");

        if (CurrentFlusteredLevel < 4)
        {
            if (NeedBeFlusteredToIncreaseLevel == false)
            {
                IncreaseFlusteredLevel();
            }
            else if (NeedBeFlusteredToIncreaseLevel == true && CurrentFlusteredLevel > 0)
            {
                IncreaseFlusteredLevel();
            }
        }
    }

    [BurstCompile]
    private void IncreaseFlusteredLevel()
    {
        CurrentFlusteredLevel++;
        IncreaseOneWantedBreastMilkActiveCustomer();
        dialogeManager.SetFlusteredLevel(ActiveCustomer, CurrentFlusteredLevel, ActiveIngreedentPercentages);
        CustomerDialogAnimator.SetTrigger("Fluster");
    }

    public PossibileOrder GetPossibileOrderCombo(List<Fillings> UnlockedStartFillings)
    {
        List<PossibileOrder> possibileOrder = new List<PossibileOrder>();

        for (int i = 0; i < PossibileWeightedOrders.Length; i++)
        {
            if (UnlockedStartFillings.Contains(PossibileWeightedOrders[i].StartFilling))
            {
                possibileOrder.Add(PossibileWeightedOrders[i]);
            }
        }

        if (possibileOrder.Count == 0)
        {
            Debug.LogError("No Fitting Startingreedient found!");
            return null;
        }
        else if (possibileOrder.Count == 1)
        {
            return possibileOrder[0];
        }
        else
        {
            return possibileOrder[Statics.GetRandomRange(0, possibileOrder.Count - 1)];
        }

        //Debug.LogError("No Fitting Order Found!");
        //return PossibileWeightedOrders[0]; //if soemthing goes wrong?
    }

    /// <summary>
    /// Will be used to create Randomized Customers, with standart dialogs
    /// </summary>
    /// <returns></returns>
    [BurstCompile]
    public Customers GenerateCustomer()
    {
        Customers generatedCusomer = Customers.CreateInstance<Customers>();

        //Get Avatar
        generatedCusomer.Avatar = GetRandomWeightingAvatar();

        if (generatedCusomer.Avatar.EventToActivate != null)
        {
            CurrentAvatarEvent = eventManager.StartEvent(generatedCusomer.Avatar.EventToActivate, true);
        }


        //Need Executed after got Avatar!
        //Need the Fulstered Value Before Generate the CUstomer because the text Generation
        CurrentFlusteredLevel = CalcFlusteredLevel(generatedCusomer.Avatar.Stats.FlusteredMultipler);


        //Generate Fillings
        GetUnlockedIngrediens();

        List<Fillings> generatedFillings = new List<Fillings>(); //this will be used to determine what will be added


        List<WeightedFillings> PossibileFillings = new List<WeightedFillings>();
        PossibileOrder possibileOrderCombo = GetPossibileOrderCombo(UnlockedFillings);
        if (possibileOrderCombo == null)
        {
            return null;
        }

        generatedFillings.Add(possibileOrderCombo.StartFilling); //Add First/Start filling

        for (int i = 0; i < possibileOrderCombo.PossibileExtraFillings.Length; i++) //Determine if the Filling is Unlocked and is in the OrderList
        {
            for (int i2 = 0; i2 < UnlockedFillings.Count; i2++)
            {
                if (UnlockedFillings[i2] == possibileOrderCombo.PossibileExtraFillings[i].Filling)
                {
                    PossibileFillings.Add(possibileOrderCombo.PossibileExtraFillings[i]);
                    break;
                }
            }

        }


        //TODO: Need to extent this to inculde the toppings
        //Event increase ingreedient count

        //int numChangedFillingsCount = 0;
        //bool canTopping = false;

        byte minFillingCount = 0;
        byte maxFillingCount = (byte)(PossibileFillings.Count);
        byte minToppingCount = 0;
        if (ChangedIngreedientCount != 0)
        {
            if (ChangedIngreedientCount > 0) //if the event wants more ingreedients
            {
                if (ChangedIngreedientCount <= maxFillingCount)
                {
                    minFillingCount = (byte)ChangedIngreedientCount;
                }
                else
                {
                    minFillingCount = maxFillingCount;
                }

            }
            else //if the event wants less ingreedients
            {
                if ( (ChangedIngreedientCount + maxFillingCount) > minToppingCount)
                {
                    maxFillingCount = (byte)(maxFillingCount + ChangedIngreedientCount);
                }
                else
                {
                    maxFillingCount = minFillingCount;
                }
            }
        }


        byte RandomCount = Statics.GetRandomRange(minFillingCount, maxFillingCount);
        //int RandomCount = Random.Range(minFillingCount, maxFillingCount);
        //Debug.Log("RandomCount: " + RandomCount + ", max: " + PossibileFillings.Count);
        if (RandomCount == 1 && PossibileFillings.Count == 1)
        {
            //Debug.Log("Added: " + PossibileFillings[0].Filling);
            generatedFillings.Add(PossibileFillings[0].Filling);
        }
        else
        {
            for (int i = 0; i < RandomCount; i++)
            {
                //int possibileFillingToAdd = Random.Range(0, PossibileFillings.Count);
                int maxValue = 0;
                for (int i2 = 0; i2 < PossibileFillings.Count; i2++)
                {
                    maxValue += PossibileFillings[i2].Weighting;
                }

                //Weighting
                int result = 0; //Wich fill should be added value
                int total = 0;
                int randVal = Statics.GetRandomRange(0, maxValue+1);
                for (result = 0; result < PossibileFillings.Count; result++)
                {
                    total += PossibileFillings[result].Weighting;
                    if (total >= randVal)
                    {
                        break;
                    }
                }

                if (result > (PossibileFillings.Count - 1))
                {
                    result = PossibileFillings.Count - 1;
                }

                //Add "result" too the ingreedient
                Debug.Log("Added: " + PossibileFillings[result].Filling);
                generatedFillings.Add(PossibileFillings[result].Filling);
                PossibileFillings.RemoveAt(result);
            }
        }


        List<Toppings> toppingsFillings = new List<Toppings>(); //this will be used to determine what will be added

        if (UnlockedToppings.Count > 0)
        {
            List<WeightedToppings> PossibileToppings = new List<WeightedToppings>();
            //PossibileToppings.AddRange(UnlockedToppings);

            for (int i = 0; i < possibileOrderCombo.WeightedToppings.Length; i++)
            {
                if (UnlockedToppings.Contains(possibileOrderCombo.WeightedToppings[i].Topping))
                {
                    PossibileToppings.Add(possibileOrderCombo.WeightedToppings[i]);
                }
            }

            RandomCount = Statics.GetRandomRange(minToppingCount, (byte)(PossibileToppings.Count));

            //Here work more
            if (RandomCount > 0)
            {
                for (int i = 0; i < RandomCount; i++)
                {
                    int maxValue = 0;
                    for (int i2 = 0; i2 < PossibileToppings.Count; i2++)
                    {
                        maxValue += PossibileToppings[i2].Weighting;
                    }

                    //Weighting
                    int result = 0; //Wich fill should be added value
                    int total = 0;
                    int randVal = Statics.GetRandomRange(0, maxValue+1);
                    for (result = 0; result < PossibileFillings.Count; result++)
                    {
                        total += PossibileFillings[result].Weighting;
                        if (total >= randVal)
                        {
                            break;
                        }
                    }

                    if (result > (PossibileToppings.Count-1))
                    {
                        result = PossibileToppings.Count - 1;
                    }

                    //Debug.Log("Added Topping: " + PossibileToppings[result].Topping);
                    toppingsFillings.Add(PossibileToppings[result].Topping);
                    PossibileToppings.RemoveAt(result);
                }
            }
        }

        //Fill Order
        generatedCusomer.OrderFillings = generatedFillings.ToArray();
        generatedCusomer.Toppings = toppingsFillings.ToArray();

        //Generate Text
        GenerateCustomerDialoges(ref generatedCusomer, generatedFillings, toppingsFillings);


        return generatedCusomer;
    }

    [BurstCompile]
    public CustomerAvatar GetRandomWeightingAvatar()
    {
        if (RandomCustomAvatars != null && RandomCustomAvatars.Count > 0)
        {
            //if (RandomCustomAvatars.Count > 1)
            //{
            //    return RandomCustomAvatars[Random.Range(0, RandomCustomAvatars.Count)];
            //}
            //else
            //{
            //    return RandomCustomAvatars[0];
            //}

            int maxValue = 0; //MaxWeight
            for (int i2 = 0; i2 < RandomCustomAvatars.Count; i2++)
            {
                maxValue += RandomCustomAvatars[i2].Stats.Weighted;
            }

            //Weighting
            int result = 0; //Wich fill should be added value
            int total = 0;
            int randVal = Statics.GetRandomRange(0, maxValue + 1);
            for (result = 0; result < RandomCustomAvatars.Count; result++)
            {
                total += RandomCustomAvatars[result].Stats.Weighted;
                if (total >= randVal)
                {
                    break;
                }
            }

            if (result > (RandomCustomAvatars.Count - 1))
            {
                result = RandomCustomAvatars.Count - 1;
            }

            return RandomCustomAvatars[result];

        }
        else { return null; }
    }

    /// <summary>
    /// This is for increasing the flustered level of the customer and update the wanted Breastmilk
    /// Realy Realy Drity written, need to be reworked to have a lot more reuasabillity
    /// </summary>
    [BurstCompile]
    private void IncreaseOneWantedBreastMilkActiveCustomer()
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
        //BreastMilk 13
        #endregion

        float MilkForReplace = 0;

        if (CurrentFlusteredLevel == 0) //NotFlustere
        {
            Debug.LogError("Something went wrong! Check the Target Values");
        }
        else
        //First need to get the FlusteredLevel then the percentage
        if (CurrentFlusteredLevel == 1) //Shouldnt come here
        {
            if (ActiveIngreedentPercentages[13] >= PercentagesReplacingBreastMilk.Level1 * 100)
            {
                return;
            }
            else
            {
                MilkForReplace = (PercentagesReplacingBreastMilk.Level2 - PercentagesReplacingBreastMilk.Level1) * 100;
            }
        }
        else if (CurrentFlusteredLevel == 2)
        {
            if (ActiveIngreedentPercentages[13] >= PercentagesReplacingBreastMilk.Level2 * 100)
            {
                return;
            }
            else
            {
                MilkForReplace = (PercentagesReplacingBreastMilk.Level2 - PercentagesReplacingBreastMilk.Level1) * 100;
            }
        }
        else if (CurrentFlusteredLevel == 3)
        {
            if (ActiveIngreedentPercentages[13] >= PercentagesReplacingBreastMilk.Level3 * 100)
            {
                return;
            }
            else
            {
                MilkForReplace = (PercentagesReplacingBreastMilk.Level3 - PercentagesReplacingBreastMilk.Level2) * 100;
            }
        }
        else if (CurrentFlusteredLevel == 4)
        {
            if (ActiveIngreedentPercentages[13] >= PercentagesReplacingBreastMilk.Level4 * 100)
            {
                return;
            }
            else
            {
                MilkForReplace = (PercentagesReplacingBreastMilk.Level4 - PercentagesReplacingBreastMilk.Level3) * 100;
            }
        }
        ActiveIngreedentPercentages[13] += MilkForReplace;

        //Just for beeing save
        if (ActiveIngreedentPercentages[13] > 100)
        {
            ActiveIngreedentPercentages[13] = 100;
        }


        //Fix values which end with a unrounded number 0-6 for all normal drink ingreedients without breastmilk
        //Should not be possibile to get this?
        for (int i = 0; i < 7; i++)
        {
            float drinkModula = (ActiveIngreedentPercentages[i] % 10);
            if (drinkModula != 0)
            {
               ActiveIngreedentPercentages[i] = ActiveIngreedentPercentages[i] - drinkModula;
               ActiveIngreedentPercentages[13] = ActiveIngreedentPercentages[13] + drinkModula;
            }
        }

        List<Fillings> OrderFillings = new List<Fillings>(ActiveCustomer.OrderFillings);
        OrderFillings.Remove(Fillings.BreastMilk);
        OrderFillings.Shuffle();


        for (int i = 0; i < OrderFillings.Count+1; i++)
        {
            if (MilkForReplace <= 0) //if no milk for replace left, go out of for //Uneven number for errorfighting
            {
                break;
            }

            switch (OrderFillings[i])
            {
                case Fillings.Chocolate:
                    if (ActiveIngreedentPercentages[0] > MilkForReplace)
                    {
                        ActiveIngreedentPercentages[0] = Mathf.Round(ActiveIngreedentPercentages[0] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - ActiveIngreedentPercentages[0]);
                        ActiveIngreedentPercentages[0] = 0;
                    }
                    break;
                case Fillings.Milk:
                    if (ActiveIngreedentPercentages[1] > MilkForReplace)
                    {
                        ActiveIngreedentPercentages[1] = Mathf.Round(ActiveIngreedentPercentages[1] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - ActiveIngreedentPercentages[1]);
                        ActiveIngreedentPercentages[1] = 0;
                    }
                    break;
                case Fillings.Tea:
                    if (ActiveIngreedentPercentages[2] > MilkForReplace)
                    {
                        ActiveIngreedentPercentages[2] = Mathf.Round(ActiveIngreedentPercentages[2] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - ActiveIngreedentPercentages[2]);
                        ActiveIngreedentPercentages[2] = 0;
                    }
                    break;
                case Fillings.Cream:
                    if (ActiveIngreedentPercentages[3] > MilkForReplace)
                    {
                        ActiveIngreedentPercentages[3] = Mathf.Round(ActiveIngreedentPercentages[3] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - ActiveIngreedentPercentages[3]);
                        ActiveIngreedentPercentages[3] = 0;
                    }
                    break;
                case Fillings.Espresso:
                    if (ActiveIngreedentPercentages[4] > MilkForReplace)
                    {
                        ActiveIngreedentPercentages[4] = Mathf.Round(ActiveIngreedentPercentages[4] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - ActiveIngreedentPercentages[4]);
                        ActiveIngreedentPercentages[4] = 0;

                    }
                    break;
                case Fillings.Sugar:
                    if (ActiveIngreedentPercentages[5] > MilkForReplace)
                    {
                        ActiveIngreedentPercentages[5] = Mathf.Round(ActiveIngreedentPercentages[5] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - ActiveIngreedentPercentages[5]);
                        ActiveIngreedentPercentages[5] = 0;
                    }
                    break;
                case Fillings.Coffee:
                    if (ActiveIngreedentPercentages[6] > MilkForReplace)
                    {
                        ActiveIngreedentPercentages[6] = Mathf.Round(ActiveIngreedentPercentages[6] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - ActiveIngreedentPercentages[6]);
                        ActiveIngreedentPercentages[6] = 0;
                    }
                    break;

                default:
                    break;
            }
        }

    }

    /// <summary>
    /// This Overrides the calc for the drink to add the milk value
    /// </summary>
    /// <param name="drink"></param>
    /// <returns></returns>
    [BurstCompile]
    public List<float> OverrideMilkFillingLogic(List<float> drink, List<Fillings> OrderFillings)
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
        //BreastMilk 13
        #endregion

        float MilkForReplace = 0;

        if (CurrentFlusteredLevel == 0) //NotFlustere
        {
            return drink;
        }
        else
        //First need to get the FlusteredLevel then the percentage
        if (CurrentFlusteredLevel == 1)
        {
            MilkForReplace = PercentagesReplacingBreastMilk.Level1 * 100;
        }
        else if (CurrentFlusteredLevel == 2)
        {
            MilkForReplace = PercentagesReplacingBreastMilk.Level2 * 100;
        }
        else if (CurrentFlusteredLevel == 3)
        {
            MilkForReplace = PercentagesReplacingBreastMilk.Level3 * 100;
        }
        else if (CurrentFlusteredLevel == 4)
        {
            MilkForReplace = PercentagesReplacingBreastMilk.Level4 * 100;
        }

        //Fix values which end with a unrounded number 0-6 for all normal drink ingreedients without breastmilk
        for (int i = 0; i < 7; i++)
        {
            float drinkModula = (drink[i] % 10);
            if (drinkModula != 0) 
            {
               drink[i] = drink[i] - drinkModula;
               drink[13] = drink[13] + drinkModula;
            }
        }

        //Set here breastmilk required
        drink[13] = drink[13] + MilkForReplace;


        //Lets tryout the "Substract Method"

        //Shuffle Fillings to substract Randomly
        OrderFillings.Shuffle();

        for (int i = 0; i < OrderFillings.Count; i++)
        {
            if (MilkForReplace <= 0) //if no milk for replace left, go out of for //Uneven number for errorfighting
            {
                break;
            }

            switch (OrderFillings[i])
            {
                case Fillings.Chocolate:
                    if (drink[0] > MilkForReplace)
                    {
                        drink[0] = Mathf.Round(drink[0] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - drink[0]);
                        drink[0] = 0;
                    }
                    break;
                case Fillings.Milk:
                    if (drink[1] > MilkForReplace)
                    {
                        drink[1] = Mathf.Round(drink[1] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - drink[1]);
                        drink[1] = 0;
                    }
                    break;
                case Fillings.Tea:
                    if (drink[2] > MilkForReplace)
                    {
                        drink[2] = Mathf.Round(drink[2] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - drink[2]);
                        drink[2] = 0;
                    }
                    break;
                case Fillings.Cream:
                    if (drink[3] > MilkForReplace)
                    {
                        drink[3] = Mathf.Round(drink[3] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - drink[3]);
                        drink[3] = 0;
                    }
                    break;
                case Fillings.Espresso:
                    if (drink[4] > MilkForReplace)
                    {
                        drink[4] = Mathf.Round(drink[4] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - drink[4]);
                        drink[4] = 0;
                    }
                    break;
                case Fillings.Sugar:
                    if (drink[5] > MilkForReplace)
                    {
                        drink[5] = Mathf.Round(drink[5] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - drink[5]);
                        drink[5] = 0;
                    }
                    break;
                case Fillings.Coffee:
                    if (drink[6] > MilkForReplace)
                    {
                        drink[6] = Mathf.Round(drink[6] - MilkForReplace);
                        MilkForReplace = 0;
                    }
                    else
                    {
                        MilkForReplace = Mathf.Round(MilkForReplace - drink[6]);
                        drink[6] = 0;
                    }
                    break;
                default:
                    break;
            }

        }

        return drink;
    }

    public void GenerateCustomerDialoges(ref Customers generatedCusomer, List<Fillings> generatedFillings, List<Toppings> toppingsFillings)
    {
        StringBuilder mainOrder = new StringBuilder();

        //CustomerDialogStartGreetings <<<< Greeting

        int OrderIngreedients = generatedFillings.Count + toppingsFillings.Count;

        //Here Condition which text will come
        bool NeedFallback = false;

        //if (eventManager != null && eventManager.EventPickyCustomerActive)
        //{
        //    int rnd = Random.Range(0, 4);

        //    switch (rnd)
        //    {
        //        case 0:
        //            mainOrder.Append("Make sure to get my drink just right.");
        //            break;
        //        case 1:
        //            mainOrder.Append("I’ll have a number 8, and two number 0s, and a number 8 large, and don’t forget the number 5...");
        //            break;
        //        case 2:
        //            mainOrder.Append("What’s the best drink on the menu?  I’ll take that.");
        //            break;
        //        case 3:
        //            mainOrder.Append("I’ll take an everything please.  But remove anything I don’t like.");
        //            break;

        //        default:
        //            mainOrder.Append("What’s the best drink on the menu?  I’ll take that.");
        //            break;
        //    }
        //}
        //else if (eventManager != null && eventManager.EventTimedOrderActive)
        //{
        //    int rnd = Random.Range(0, 5);

        //    switch (rnd)
        //    {
        //        case 0:
        //            mainOrder.Append("I’m late for my meeting!");
        //            break;
        //        case 1:
        //            mainOrder.Append("Could you hurry up?");
        //            break;
        //        case 2:
        //            mainOrder.Append("I need to get to work, make it quick.");
        //            break;
        //        case 3:
        //            mainOrder.Append("My bus is almost here, hurry please!");
        //            break;
        //        case 4:
        //            mainOrder.Append("Is that ");
        //            mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
        //            mainOrder.Append(" ready yet?");
        //            break;
        //        case 5:
        //            mainOrder.Append("How long could a ");
        //            mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
        //            mainOrder.Append(" take to make?");
        //            break;

        //        default:
        //            mainOrder.Append("My bus is almost here, hurry please!");
        //            break;
        //    }
        //}
        //else if (CustomerIsFlustered == false) // = Kerbs Customer Orders Fluster Level 1:
        if (CustomerIsFlustered == false) // = Kerbs Customer Orders Fluster Level 1:
        {
                int rnd = Statics.GetRandomRange(0, 11);

                switch (rnd)
                {
                    case 0:
                        mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                        mainOrder.Append(" Just a drink please.");
                        break;
                    case 1:
                        mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                        mainOrder.Append(" Can I get some ");
                        mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                        mainOrder.Append(" ?");
                        break;
                    case 2:
                        mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                        mainOrder.Append(" Do you have ");
                        mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                        mainOrder.Append(" ?");
                        break;
                    case 3:
                        mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                        mainOrder.Append(" Some ");
                        mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                        mainOrder.Append(" please.");
                        break;
                    case 4:
                        mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                        mainOrder.Append(".");
                        break;
                    case 5:
                        mainOrder.Append(CustomerDialogGetDrinkModifier(generatedCusomer));
                        mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                        mainOrder.Append(".");
                        break;
                    case 6:
                        mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                        mainOrder.Append(". Hold the ");
                        mainOrder.Append(CustomerDialogGetNotIngreedient(generatedCusomer));
                        mainOrder.Append(" .");
                        break;
                    case 7:
                        mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                        mainOrder.Append(" Can I get some ");
                        mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                        mainOrder.Append(", without any ");
                        mainOrder.Append(CustomerDialogGetNotIngreedient(generatedCusomer));
                        mainOrder.Append(" ?");
                        break;
                    case 8:
                        mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                        break;
                    case 9:
                        mainOrder.Append("One ");
                        mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                        mainOrder.Append(".");
                        break;
                    case 10:
                        mainOrder.Append("One ");
                        mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                        mainOrder.Append(" with no ");
                        mainOrder.Append(CustomerDialogGetNotIngreedient(generatedCusomer));
                        mainOrder.Append(".");
                        break;
                    case 11:
                        mainOrder.Append("A ");
                        mainOrder.Append(CustomerDialogGetDrinkModifier(generatedCusomer));
                        mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                        mainOrder.Append(" with no ");
                        mainOrder.Append(CustomerDialogGetNotIngreedient(generatedCusomer));
                        mainOrder.Append(".");
                        break;

                    default:
                        mainOrder.Append("Just a drink please.");
                        break;
                }
            //}

   
        }
        else if (CurrentFlusteredLevel == 1) // = Kerbs Customer Orders Fluster Level 2
        {
            int rnd = Statics.GetRandomRange(0, 11);

            switch (rnd)
            {
                case 0:
                    mainOrder.Append("Ah yes, ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Can I get some ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append("?");
                    break;
                case 1:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    break;
                case 2:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Nice day today, can I get a ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append("?");
                    break;
                case 3:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(".");
                    break;
                case 4:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    break;
                case 5:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(", hold the ");
                    mainOrder.Append(CustomerDialogGetNotIngreedient(generatedCusomer));
                    mainOrder.Append(".");
                    break;
                case 6:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" You have ");
                    mainOrder.Append(CustomerDialogGetDrinkModifier(generatedCusomer));
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append("s right?");
                    break;
                case 7:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Some ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" please.");
                    break;
                case 8:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Seems like a good day for some ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(".");
                    break;
                case 9:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" One ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(".");
                    break;
                case 10:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" One ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(",no ");
                    mainOrder.Append(CustomerDialogGetNotIngreedient(generatedCusomer));
                    mainOrder.Append(".");
                    break;
                case 11:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" A ");
                    mainOrder.Append(CustomerDialogGetDrinkModifier(generatedCusomer));
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(",with no ");
                    mainOrder.Append(CustomerDialogGetNotIngreedient(generatedCusomer));
                    mainOrder.Append(".");
                    break;

                default:
                    mainOrder.Append("Just a drink please.");
                    break;
            }
        }
        else if (CurrentFlusteredLevel == 2) // = Kerbs Customer Orders Fluster Level 3
        {
            int rnd = Statics.GetRandomRange(0, 10);

            switch (rnd)
            {
                case 0:
                    mainOrder.Append("Oh my...");
                    break;
                case 1:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    break;
                case 2:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(CustomerDialogGetDrinkModifier(generatedCusomer));
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" please?");
                    break;
                case 3:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(". hold the ");
                    mainOrder.Append(CustomerDialogGetNotIngreedient(generatedCusomer));
                    mainOrder.Append(" please?");
                    break;
                case 4:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" This place seems nice, can I get a ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append("?");
                    break;
                case 05:
                    mainOrder.Append("The tip jar helps fund your progress… right?");
                    break;
                case 6:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" One ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" please.");
                    break;
                case 7:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" One ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(",no ");
                    mainOrder.Append(CustomerDialogGetNotIngreedient(generatedCusomer));
                    mainOrder.Append(" please.");
                    break;
                case 8:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" One ");
                    mainOrder.Append(CustomerDialogGetDrinkModifier(generatedCusomer));
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" with no ");
                    mainOrder.Append(CustomerDialogGetNotIngreedient(generatedCusomer));
                    mainOrder.Append(" please.");
                    break;
                case 9:
                    mainOrder.Append("You’re free range right?");
                    break;
                case 10:
                    mainOrder.Append("Am I reading the menu right?");
                    break;

                default:
                    mainOrder.Append("Just a drink please.");
                    break;
            }
        }
        else if (CurrentFlusteredLevel == 3) // = Kerbs Customer Orders Fluster Level 4
        {
            int rnd = Statics.GetRandomRange(0, 10);

            switch (rnd)
            {
                case 0:
                    mainOrder.Append("Wow the ads weren’t lying about you...");
                    break;
                case 1:
                    mainOrder.Append("This place really has Mooters beat!");
                    break;
                case 2:
                    mainOrder.Append("Um… erm… ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" One");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" please?");
                    break;
                case 3:
                    mainOrder.Append("Wow… ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Can I get a");
                    mainOrder.Append(CustomerDialogGetDrinkModifier(generatedCusomer));
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" please?");
                    break;
                case 4:
                    mainOrder.Append("Wow the ads weren’t lying about you...");
                    break;
                case 5:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    break;
                case 6:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Can I please have a ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append("  mommy- erm!  Miss...");
                    break;
                case 7:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Do you serve ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" with a side of you?");
                    break;
                case 8:
                    mainOrder.Append("Whoa, are you on the menu?");
                    break;
                case 9:
                    mainOrder.Append("This is quite a different type of open bar cafe.");
                    break;
                case 10:
                    mainOrder.Append("Isn’t that “No Shirt No Service” sign out front a bit ironic…?");
                    break;
                case 11:
                    mainOrder.Append("You’re definitely bigger than last time. Awesome.");
                    break;
                case 12:
                    mainOrder.Append("You charge extra if I DON’T stare??");
                    break;
                case 13:
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" please, maybe with fresh milk…?");
                    break;
                case 14:
                    mainOrder.Append("Did the menu always include “fresh milk”?");
                    break;
                case 15:
                    mainOrder.Append("Big lady! Good ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(".");
                    break;

                default:
                    mainOrder.Append("Just a drink please.");
                    break;
            }
        }
        else if (CurrentFlusteredLevel == 4) // = Kerbs Customer Orders Fluster Level 5
        {
            int rnd = Statics.GetRandomRange(0, 14);

            switch (rnd)
            {
                case 0:
                    mainOrder.Append("Whoa mama!");
                    break;
                case 1:
                    mainOrder.Append("H-hi can I get a ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" with breast milk- I mean breast milk!- I mean breast milk…! I mean I mean breast milk sorry breast milk I mean...");
                    break;
                case 2:
                    mainOrder.Append("I-I-I-I… drink… thirsty...");
                    break;
                case 3:
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append("!");
                    break;
                case 4:
                    mainOrder.Append("!!!");
                    break;
                case 5:
                    mainOrder.Append("Awooga!");
                    break;
                case 6:
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" with EXTRA fresh milk… please?");
                    break;
                case 7:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Do you sell straight from the tap…?");
                    break;
                case 8:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" J-Just some ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" please… (Wow she’s huge...)");
                    break;
                case 9:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append("...");
                    break;
                case 10:
                    mainOrder.Append("This is quite the marketing strategy you’ve got going…!");
                    break;
                case 11:
                    mainOrder.Append("Oh baby!");
                    break;
                case 12:
                    mainOrder.Append("Is it okay if I get some fresh milk instead?");
                    break;
                case 13:
                    mainOrder.Append("Just some breast milk please.");
                    break;
                case 14:
                    mainOrder.Append("One cup of fresh milk please..");
                    break;

                default:
                    mainOrder.Append("Just a drink please.");
                    break;
            }
        }
        else
        {
            mainOrder.Append("Something went wrong, Contact Noa to correct this!");
        }

        //If something is not working right, get this texts...
        if (NeedFallback == true)
        {
            int rnd = Statics.GetRandomRange(0, 5);

            switch (rnd)
            {
                case 0:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Just a drink please.");
                    break;
                case 1:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Can I get some ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" ?");
                    break;
                case 2:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append( "Do you have ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" ?");
                    break;
                case 3:
                    mainOrder.Append(Statics.CustomerDialogStartGreetings[Statics.GetRandomRange(0, Statics.CustomerDialogStartGreetings.Length - 1)]);
                    mainOrder.Append(" Some ");
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(" please.");
                    break;
                case 4:
                    mainOrder.Append(CustomerDialogGetDrinkType(generatedCusomer));
                    mainOrder.Append(".");
                    break;

                default:
                    mainOrder.Append("My bus is almost here, hurry please!");
                    break;
            }
        }


        //mainOrder.Append(Statics.CustomerDialogStartEnd);

        List<string> OrderBuilder = new List<string>();

        OrderBuilder.Add(mainOrder.ToString());

        Dialogue Order = new Dialogue(OrderBuilder.ToArray());
        generatedCusomer.DialogeTextForOrder = Order;

    }

    [BurstCompile]
    private string CustomerDialogGetDrinkModifier(Customers generatedCusomer)
    {
        bool useToppings = false;
        if (generatedCusomer.Toppings.Length > 0)
        {
            if (generatedCusomer.OrderFillings.Length == 1)
            {
                useToppings = true;
            }
            else
            {
                if (Statics.RandomBool())
                {
                    useToppings = true;
                }
            }
        }

        if (useToppings == true)
        {
            switch (generatedCusomer.Toppings[Statics.GetRandomRange(0, generatedCusomer.Toppings.Length - 1)])
            {
                case Toppings.Ice:
                    return Statics.CustomerDialogDrinkModiferIce;
                case Toppings.ChocolateSauce:
                    return Statics.CustomerDialogDrinkModiferChocolateSauce;
                case Toppings.Boba:
                    return Statics.CustomerDialogDrinkModiferBoba;
                case Toppings.Sprinkles:
                    return Statics.CustomerDialogDrinkModiferBoba;
                case Toppings.CaramelSauce:
                    return Statics.CaramelSauce;
                case Toppings.WhipedCream:
                    return Statics.CustomerDialogDrinkModiferWhippedCream;

                default:
                    return "";
            }
        }
        else
        {
            if (generatedCusomer.OrderFillings.Length > 2)
            {
                switch (generatedCusomer.OrderFillings[Statics.GetRandomRange(1, generatedCusomer.OrderFillings.Length - 1)])
                {
                    case Fillings.Milk:
                        return Statics.CustomerDialogDrinkModiferMilk;
                    case Fillings.Espresso:
                        return Statics.CustomerDialogDrinkModiferEspresso;
                    case Fillings.Sugar:
                        return Statics.CustomerDialogDrinkModiferSugar;
                    case Fillings.Cream:
                        return Statics.CustomerDialogDrinkModiferWhippedCream;
                    case Fillings.Coffee:
                        return Statics.CustomerDialogDrinkModiferCoffee;
                    case Fillings.Chocolate:
                        return Statics.CustomerDialogDrinkModiferChocolate;

                    default:
                        return "";
                }
            }
            else
            {
                return "";
            }
        }

        //return null;
    }

    [BurstCompile]
    private string CustomerDialogGetDrinkType(Customers generatedCusomer)
    {
        switch (generatedCusomer.OrderFillings[0])
        {
            case Fillings.Coffee:
                return Statics.CustomerDialogDrinkTypeCoffee;
            case Fillings.Milk:
                return Statics.CustomerDialogDrinkTypeMilk;
            case Fillings.Espresso:
                return Statics.CustomerDialogDrinkTypeEspresso;
            case Fillings.Tea:
                return Statics.CustomerDialogDrinkTypeTea;
            case Fillings.Cream:
                return Statics.CustomerDialogDrinkTypeCream;
            case Fillings.Chocolate:
                return Statics.CustomerDialogDrinkTypeChocolate;

            default:
                return "";
        }

        //return null;
    }

    //TODO: work the script out
    //A random ingredient not included in the drink.  For drinks that have every ingredient, we could maybe use a word like “sawdust” as a joke ingredient customers don’t want.
    [BurstCompile]
    private string CustomerDialogGetNotIngreedient(Customers generatedCusomer)
    {
        //Espresso,
        //Coffee,
        //Chocolate,
        //Tea,
        //Milk,
        //BreastMilk,
        //Cream,
        //Sugar
        List<Fillings> NotInculdedFillingsList = new List<Fillings>();

        if (generatedCusomer.OrderFillings.Contains(Fillings.Espresso) == false)
        {
            NotInculdedFillingsList.Add(Fillings.Espresso);
        }
        if (generatedCusomer.OrderFillings.Contains(Fillings.Coffee) == false)
        {
            NotInculdedFillingsList.Add(Fillings.Coffee);
        }
        if (generatedCusomer.OrderFillings.Contains(Fillings.Chocolate) == false)
        {
            NotInculdedFillingsList.Add(Fillings.Chocolate);
        }
        if (generatedCusomer.OrderFillings.Contains(Fillings.Tea) == false)
        {
            NotInculdedFillingsList.Add(Fillings.Tea);
        }
        if (generatedCusomer.OrderFillings.Contains(Fillings.Milk) == false)
        {
            NotInculdedFillingsList.Add(Fillings.Milk);
        }
        if (generatedCusomer.OrderFillings.Contains(Fillings.Cream) == false)
        {
            NotInculdedFillingsList.Add(Fillings.Cream);
        }
        if (generatedCusomer.OrderFillings.Contains(Fillings.Sugar) == false)
        {
            NotInculdedFillingsList.Add(Fillings.Sugar);
        }

        int count = NotInculdedFillingsList.Count;
        const string placeholder = "Sawdust";

        if (count == 0)
        {
            return placeholder;
        }
        else if (count == 1)
        {
            return NotInculdedFillingsList[0].ToString();
        }
        else
        {
            return NotInculdedFillingsList[Statics.GetRandomRange(0, count - 1)].ToString();
        }
    }


    [BurstCompile]
    public void GetUnlockedIngrediens()
    {
        UnlockedToppings.Clear();
        UnlockedFillings.Clear();

        fillingTools = FindObjectsOfType<FillingTool>();

        for (int i = 0; i < fillingTools.Length; i++)
        {
            if (fillingTools[i].Unlocked)
            {
                if (fillingTools[i].isTopping)
                {
                    UnlockedToppings.Add(fillingTools[i].CupToppings);
                }
                else
                {
                    UnlockedFillings.Add(fillingTools[i].MashineFilling);
                }
            }
        }
    }

    [BurstCompile]
    public bool CheckIfFlustered()
    {
        if (isAlwaysFlustered == true || EventAlwaysFlustered == true )
        {
            return true;
        }
        else if (EventCustomerNotFlustered == true)
        {
            return false;
        }

        FlusteredChance = gamemode.Fullness + gamemode.TargetBustSize; //Fullness and MaxSize gives together 200
        float rnd = Statics.GetRandom().NextFloat(0f, 175f); //the 175 here need maybe to adjust to depend the game difficulty
        Debug.Log("Flustered Chance: " + FlusteredChance + " ? > " + rnd +  " || Fullness: " + gamemode.Fullness + " || BustSize: " + gamemode.TargetBustSize);

        if (FlusteredChance > rnd)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    [BurstCompile]
    public int CalcFlusteredLevel(float multipler = 1f) //Depending on fullness?
    {
        float randomNumber = ((gamemode.Fullness + gamemode.TargetBustSize) / 2 * multipler) * Statics.GetRandom().NextFloat(FlusteredRandomMultiplerMin, FlusteredRandomMultiplerMax);

        if (randomNumber > FlusteredLevelActivationMin4)
        {
            return 4; ;
        }
        else if (randomNumber > FlusteredLevelActivationMin3)
        {
            return 3;
        }
        else if (randomNumber > FlusteredLevelActivationMin2 || isAlwaysFlustered == true)
        {
            return 2;
        }
        else if (randomNumber > FlusteredLevelActivationMin1)
        {
            return 1;
        }

        return 0;
    }

    [BurstCompile]
    public void OrderFinished()
    {
        CheckArchievementOneOfEverything();

        if (ActiveCustomer != null)
        {
            float rating = CheckIfOrderIsValid();

            Rating ratingClass = CalcOrderRating(rating);

            if (ratingClass == null)
            {
                Debug.LogError("Didnt Find Rating!");
                dialogeManager.StartDialogueFail(ActiveCustomer);
                //return;
            }
            else
            {
                Debug.Log("RatingClass: " + ratingClass.Name);

                soundEffectManager.PlaySoundOneShot(ratingClass.SoundEffect);
                gamemode.ChangeHappyness(ratingClass.HappynessChange);
            }


            if (ratingClass.isFail == false) //rating > 0.1f || 
            {
                //Calc Money
                float moneyMultipler = ratingClass.MoneyMultipler;
                if (moneyMultipler > 0)
                {
                    float moneyGain = CalcMoneyGain(moneyMultipler);
                    if (EventMoreValueSell == true)
                    {
                        moneyGain = moneyGain * EventSellMoreValueMultipler;
                    }
                    gamemode.AddMoney(moneyGain);

                    if (statisticsHolder != null)
                    {
                        statisticsHolder.AddServedCup();
                        statisticsHolder.AddEarnedMoney(moneyGain);
                    }
                }

                dialogeManager.StartDialogueSuccess(ActiveCustomer);
                baristaTalkManager.TryBaristaEventCupFinished();
                CompletedCups++;
            }
            else
            {
                if (cupController.FillVolume == 0)
                {
                    gamemode.SubHappyness(PunishingValueForEmptyCupInOrder);
                }
                dialogeManager.StartDialogueFail(ActiveCustomer);
            }

            TimeLastOrder = Time.time;
            orderIsActive = false;

            cupController.DoFadeAway();
            StartCoroutine(DoResetCup(FinishedOrderCupResetOffset));


        }
        else
        {
            StartCoroutine( DoResetCup() );
        }

        if (CurrentAvatarEvent != null && ActiveCustomer.Avatar.Stats.RemoveEventIfCustomerIsFinished == true)
        {
            eventManager.StopEvent(CurrentAvatarEvent);
        }

        ActiveCustomer = null;
    }

    public IEnumerator DoResetCup(float offset = 0)
    {
        yield return new WaitForSeconds(offset);
        cupController.ResetCup();
    }

    [BurstCompile]
    public float CheckIfOrderIsValid()
    {
        if (ActiveCustomer == null)
        {
            return 0;
        }

        bool retVal = true;

        int CustomerfillingCount = ActiveCustomer.OrderFillings.Length + ActiveCustomer.Toppings.Length;
        //Debug.Log("Fullness: " + cupController.Fullness + " ;CustomerFillingcount = " + CustomerfillingCount);

        if (cupController.Fullness < FillingSuccecsTollerance)
        {
            //Debug.Log("To less Fill!");
            return 0;
        }

        if (CustomerfillingCount == 0)
        {
            //Debug.Log("No order?");
            return 0;
        }

        ///value for determine how perfect is the filling
        float AccuracyDeviation = 0;
        float missingToppingValue = (1f / CustomerfillingCount);
        int usedCustomerToppings = 0;

        //Check for toppings first
        if (ActiveCustomer.Toppings != null && ActiveCustomer.Toppings.Length > 0)
        {
            for (int i = 0; i < ActiveCustomer.Toppings.Length; i++)
            {
                switch (ActiveCustomer.Toppings[i])
                {
                    case Toppings.Ice:
                        if (cupController.Ice == false)
                        {
                            //retVal = false;
                            AccuracyDeviation += missingToppingValue;
                            break;
                        }
                        else
                        {
                            usedCustomerToppings++;
                        }
                        break;
                    case Toppings.Boba:
                        if (cupController.Boba == false)
                        {
                            //retVal = false;
                            AccuracyDeviation += missingToppingValue;
                            break;
                        }
                        else
                        {
                            usedCustomerToppings++;
                        }
                        break;
                    case Toppings.WhipedCream:
                        if (cupController.WhippedCream == false)
                        {
                            //retVal = false;
                            AccuracyDeviation += missingToppingValue;
                            break;
                        }
                        else
                        {
                            usedCustomerToppings++;
                        }
                        break;
                    case Toppings.CaramelSauce:
                        if (cupController.CaramelSauce == false)
                        {
                            //retVal = false;
                            AccuracyDeviation += missingToppingValue;
                            break;
                        }
                        else
                        {
                            usedCustomerToppings++;
                        }
                        break;
                    case Toppings.ChocolateSauce:
                        if (cupController.ChocolateSauce == false)
                        {
                            //retVal = false;
                            AccuracyDeviation += missingToppingValue;
                            break;
                        }
                        else
                        {
                            usedCustomerToppings++;
                        }
                        break;
                    case Toppings.Sprinkles:
                        if (cupController.Sprinkles == false)
                        {
                            //retVal = false;
                            AccuracyDeviation += missingToppingValue;
                            break;
                        }
                        else
                        {
                            usedCustomerToppings++;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        //If more toppings are used then Customer requested...
        int usedCupToppings = cupController.CountUsedToppings();
        if (usedCupToppings > usedCustomerToppings)
        {
            AccuracyDeviation += (missingToppingValue * (usedCupToppings - usedCustomerToppings)) * PunishingMultiplerForWrongIngreedient;
        }


        //if still is order valid, check fillings
        if (retVal == true)
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
            //BreastMilk 13
            #endregion

            //Old Calc
            //AccuracyDeviation = AccuracyDeviation + Mathf.Abs(cupController.Chocolate - (ActiveIngreedentPercentages[0] / 100));
            //AccuracyDeviation = AccuracyDeviation + Mathf.Abs(cupController.Milk - (ActiveIngreedentPercentages[1] / 100));
            //AccuracyDeviation = AccuracyDeviation + Mathf.Abs(cupController.Tea - (ActiveIngreedentPercentages[2] / 100));
            //AccuracyDeviation = AccuracyDeviation + Mathf.Abs(cupController.Cream - (ActiveIngreedentPercentages[3] / 100));
            //AccuracyDeviation = AccuracyDeviation + Mathf.Abs(cupController.Espresso - (ActiveIngreedentPercentages[4] / 100));
            //AccuracyDeviation = AccuracyDeviation + Mathf.Abs(cupController.Sugar - (ActiveIngreedentPercentages[5] / 100));
            //AccuracyDeviation = AccuracyDeviation + Mathf.Abs(cupController.Coffee - (ActiveIngreedentPercentages[6] / 100));

            //ArionWays Calc
            if ((ActiveIngreedentPercentages[0] / 100) > cupController.Chocolate)//If less of the ingredient than wanted
            {
                AccuracyDeviation = AccuracyDeviation + ((ActiveIngreedentPercentages[0] / 100) - cupController.Chocolate);//+deviation from the target value
            }
            else//If more of the ingredient than wanted
            {
                //maybe price?
            }
            if ((ActiveIngreedentPercentages[1] / 100) > cupController.Milk)//If less of the ingredient than wanted
            {
                AccuracyDeviation = AccuracyDeviation + ((ActiveIngreedentPercentages[1] / 100) - cupController.Milk);//+deviation from the target value
            }
            else//If more of the ingredient than wanted
            {
                //maybe price?
            }
            if ((ActiveIngreedentPercentages[2] / 100) > cupController.Tea)//If less of the ingredient than wanted
            {
                AccuracyDeviation = AccuracyDeviation + ((ActiveIngreedentPercentages[2] / 100) - cupController.Tea);//+deviation from the target value
            }
            else//If more of the ingredient than wanted
            {
                //maybe price?
            }
            if ((ActiveIngreedentPercentages[3] / 100) > cupController.Cream)//If less of the ingredient than wanted
            {
                AccuracyDeviation = AccuracyDeviation + ((ActiveIngreedentPercentages[3] / 100) - cupController.Cream);//+deviation from the target value
            }
            else//If more of the ingredient than wanted
            {
                //maybe price?
            }
            if ((ActiveIngreedentPercentages[4] / 100) > cupController.Espresso)//If less of the ingredient than wanted
            {
                AccuracyDeviation = AccuracyDeviation + ((ActiveIngreedentPercentages[4] / 100) - cupController.Espresso);//+deviation from the target value
            }
            else//If more of the ingredient than wanted
            {
                //maybe price?
            }
            if ((ActiveIngreedentPercentages[5] / 100) > cupController.Sugar)//If less of the ingredient than wanted
            {
                AccuracyDeviation = AccuracyDeviation + ((ActiveIngreedentPercentages[5] / 100) - cupController.Sugar);//+deviation from the target value
            }
            else//If more of the ingredient than wanted
            {
                //maybe price?
            }
            if ((ActiveIngreedentPercentages[6] / 100) > cupController.Coffee)//If less of the ingredient than wanted
            {
                AccuracyDeviation = AccuracyDeviation + ((ActiveIngreedentPercentages[6] / 100) - cupController.Coffee);//+deviation from the target value
            }
            else//If more of the ingredient than wanted
            {
                //maybe price?
            }
            if ((ActiveIngreedentPercentages[13] / 100) > cupController.BreastMilk)//If less of the ingredient than wanted
            {
                AccuracyDeviation = AccuracyDeviation + ((ActiveIngreedentPercentages[13] / 100) - cupController.BreastMilk);//+deviation from the target value
            }
            else//If more of the ingredient than wanted
            {
                //maybe price?
            }

            //Here the breas Milk calc?
            #region Old Milk Calc
            //if (FlusteredLevel >= 0 && cupController.BreastMilk > 0.02f)
            //{
            //    float maxCalcMilkValue = cupController.BreastMilk;

            //    if (FlusteredLevel == 0)
            //    {
            //        if (maxCalcMilkValue > 0.25f)
            //        {
            //            maxCalcMilkValue = 0.25f;
            //        }
            //    }
            //    if (FlusteredLevel == 1)
            //    {
            //        if (maxCalcMilkValue > 0.50f)
            //        {
            //            maxCalcMilkValue = 0.50f;
            //        }
            //    }
            //    if (FlusteredLevel == 2)
            //    {
            //        if (maxCalcMilkValue > 0.75f)
            //        {
            //            maxCalcMilkValue = 0.75f;
            //        }
            //    }
            //    if (FlusteredLevel == 3)
            //    {
            //        //This will never be happen
            //        //if (maxCalcMilkValue > 1f)
            //        //{
            //        //    maxCalcMilkValue = 1f;
            //        //}
            //    }

            //    //if maxMilk > cupController.BreastMilk then do penalty?
            //    if (FlusteredLevel < 3 && cupController.BreastMilk > maxCalcMilkValue)
            //    {
            //        AccuracyDeviation = AccuracyDeviation + (cupController.BreastMilk - maxCalcMilkValue);
            //    }

            //    AccuracyDeviation = AccuracyDeviation - ((AccuracyDeviation / 100) * (maxCalcMilkValue * 100));

            //    //AccuracyDeviation = (((AccuracyDeviation / 100) * maxCalcMilkValue) * 100 ); //Percentage of milk on the acuracy
            //    //AccuracyDeviation = AccuracyDeviation - maxCalcMilkValue;
            //}
            #endregion
        }

        //Substract the missing cup fill value
        AccuracyDeviation = AccuracyDeviation - (cupController.Fullness - 1);

        AccuracyDeviation = 1 - (AccuracyDeviation);


#if UNITY_EDITOR
        if (AccuracyDeviation < 0)
        {
            AccuracyDeviation = 0;
        }
#endif


        if (retVal == true)
        {
            //Debug.Log("Accuracy: " + AccuracyDeviation);
            //CalcOrderRating(AccuracyDeviation); //- 1f
        }
        else
        {
            AccuracyDeviation = 0;
        }

        return AccuracyDeviation;
    }

    [BurstCompile]
    public Rating CalcOrderRating(float decreassingValue , bool showRating = true)
    {

        for (int i = 0; i < Ratings.Length; i++)
        {
            if (Ratings[i].MinValue <= decreassingValue)
            {
                //Debug.Log("Rating: " + Ratings[i].Name);
                if (showRating == true)
                {
                    RatingAnimationHelper.instance.ShowRating(Ratings[i].Name, Ratings[i].RatingColor);
                }
                return Ratings[i];
                //break;
            }
        }

        return null;
    }

    [BurstCompile]
    public float CalcMoneyGain(float Multipler = 1f)
    {
        if (ActiveCustomer == null)
        {
            return 0;
        }

        float retVal = 0f;

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
        //BreastMilk 13
        #endregion

        retVal = retVal + (cupController.Chocolate * IngredientValue.Chocolate);
        retVal = retVal + (cupController.Milk * IngredientValue.Milk);
        retVal = retVal + (cupController.Tea * IngredientValue.Tea);
        retVal = retVal + (cupController.Cream * IngredientValue.Cream);
        retVal = retVal + (cupController.Espresso * IngredientValue.Espresso);
        retVal = retVal + (cupController.Sugar * IngredientValue.Sugar);
        retVal = retVal + (cupController.Coffee * IngredientValue.Coffee);
        retVal = retVal + (cupController.BreastMilk * IngredientValue.BreastMilk); //Most Important ones

        if (cupController.Boba)
        {
            retVal = retVal + IngredientValue.Boba;
        }
        if (cupController.Ice)
        {
            retVal = retVal + IngredientValue.Ice;
        }
        if (cupController.WhippedCream)
        {
            retVal = retVal + IngredientValue.WhippedCream;
        }
        if (cupController.ChocolateSauce)
        {
            retVal = retVal + IngredientValue.ChocolateSauce;
        }
        if (cupController.CaramelSauce)
        {
            retVal = retVal + IngredientValue.CaramelSauce;
        }
        if (cupController.Sprinkles)
        {
            retVal = retVal + IngredientValue.Sprinkles;
        }

        int fillingsCount = cupController.CountUsedFillings();

        if (fillingsCount > 0)
        {
            retVal = retVal * (IngredientsMultiplier[fillingsCount - 1] + 1); //Apply the multipler for ingreedients
        }

        return retVal * Multipler;
    }

    [BurstCompile]
    public void CheckArchievementOneOfEverything()
    {
        if (cupController.Espresso > 0.01f && cupController.Coffee > 0.01f && cupController.Chocolate > 0.01f && cupController.Tea > 0.01f && cupController.Milk > 0.01f && cupController.BreastMilk > 0.01f && cupController.Cream > 0.01f && cupController.Sugar > 0.01f
            && cupController.Ice == true && cupController.Boba == true && cupController.WhippedCream == true && cupController.CaramelSauce == true && cupController.ChocolateSauce == true && cupController.Sprinkles == true)
        {
            Archievements.UnlockArchievement(Archievements.ArchievementID.One_OfEverything);
        }
    }
}

[System.Serializable]
public class BreastMilkPercentages
{
    [Range(0,1f)]
    public float Level0 = 0;
    [Range(0, 1f)]
    public float Level1 = 0.2f;
    [Range(0, 1f)]
    public float Level2 = 0.4f;
    [Range(0, 1f)]
    public float Level3 = 0.6f;
    [Range(0, 1f)]
    public float Level4 = 0.8f;
}

[System.Serializable]
public class Rating
{
    public string Name = "";
    public LocalizedString StringRatingText;
    [Range(0,1)]
    public float MinValue = 0;
    public float MoneyMultipler = 1.0f;
    public AudioClip SoundEffect;
    [Tooltip("Changes the happyness of the given value")]
    public float HappynessChange = 0;
    public Color RatingColor = Color.green;
    public bool isFail = false;

    public void SetName(string name)
    {
        Name = name;
    }
}


[System.Serializable]
public class IngreedientCosts
{
    public float Espresso = 7.50f;
    public float Coffee = 2f;
    public float Chocolate = 5f;
    public float Tea = 3f;
    public float Milk = 1f;
    public float BreastMilk = 10f;
    public float Cream = 2f;
    public float Sugar = 1f;
    public float Ice = 0.50f;
    public float Boba = 2f;
    public float WhippedCream = 1f;
    public float CaramelSauce = 0.75f;
    public float ChocolateSauce = 0.75f;
    public float Sprinkles = 0.50f;
}

[System.Serializable]
//Holds the value how much will be come in the cup
public class FillingPercentage
{
    public TwoFillings[] twoFillings;
    public ThreeFillings[] threeFillings;
    public FourFillings[] fourFillings;
    public FiveFillings[] fiveFillings;
    public SixFillings[] sixFillings;
    public SevenFillings[] sevenFillings;
    public EightFillings[] eightFillings;


    [System.Serializable]
    public class TwoFillings
    {
        public float value1;
        public float value2;
    }

    [System.Serializable]
    public class ThreeFillings
    {
        public float value1;
        public float value2;
        public float value3;
    }

    [System.Serializable]
    public class FourFillings
    {
        public float value1;
        public float value2;
        public float value3;
        public float value4;
    }

    [System.Serializable]
    public class FiveFillings
    {
        public float value1;
        public float value2;
        public float value3;
        public float value4;
        public float value5;
    }

    [System.Serializable]
    public class SixFillings
    {
        public float value1;
        public float value2;
        public float value3;
        public float value4;
        public float value5;
        public float value6;
    }

    [System.Serializable]
    public class SevenFillings
    {
        public float value1;
        public float value2;
        public float value3;
        public float value4;
        public float value5;
        public float value6;
        public float value7;
    }

    [System.Serializable]
    public class EightFillings
    {
        public float value1;
        public float value2;
        public float value3;
        public float value4;
        public float value5;
        public float value6;
        public float value7;
        public float value8;
    }
}