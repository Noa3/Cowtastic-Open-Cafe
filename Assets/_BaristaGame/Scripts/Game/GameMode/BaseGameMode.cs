using System.Collections;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;

public class BaseGameMode : MonoBehaviour
{
    [Header("Base Game Mode Stuff")]

    [Header("References")]


    [HideInInspector]
    public StatsManager statsManager;
    [HideInInspector]
    public BaristaController baristaController;
    [HideInInspector]
    public CupController cupController;

    public Animator CustomerDialogAnimator;

    [Header("Setting")]

    public AnimationCurve BustHappynessDecrease;


    [Range(0,100)]
    public float HappynessNeededForProduce = 20;
    public float HappynessIncreaseFromMilkingPerSecond = 2;

    [Tooltip("if the barista gets a Headpat, her Happyness will not go down")]
    public bool WhileHeadpatingDontDecreaseHappyness = true;
    [Tooltip("If the Barista start to get a headpat, give this boost to the Happyness")]
    public float HappynessIncreaseHeadPatOneTime = 2;
    [Tooltip("Time between the Happyness increases of duration from headpatting")]
    public float HappynessIncreaseHeadPatTickTime = 2;
    [Tooltip("this gives Happyness for the duration of headpating")]
    public float HappynessIncreaseHeadPatOverTime = 0.3f;

    [Header("Stats")]
    public float Money = 100;

    [HideInInspector] //Currently not needed
    public int Level = 1;
    [HideInInspector] //Currently not needed
    public float ExpCurrent = 0;
    [HideInInspector] //Currently not needed
    public float ExpNeedToNextLevel = 3;
    [HideInInspector] //Currently not needed
    public bool CanLevelUp = true;

    public double ProductionRate = 10;
    /// <summary>
    /// max of maxsize will be 100, its corrospending to the current bust size
    /// </summary>
    [Tooltip("How far the values can get for the Bust")]
    public float MaxSize = 200;
    [Tooltip("MaxSize for the upgrades")][Min(1)]
    public float CurrentMaxSize = 10;
    public float MilkSpeed = 2;


    [Range(0,100)]
    [Tooltip("Targeted Actualbust")]
    /// <summary>
    ///How Much Bust have the barista currently(Not visual)
    /// </summary>
    public float TargetBustSize = 0;     
    [Range(0, 100)][ReadOnly]
    [InspectorName("BustSize (Read Only)")][Tooltip("Actual Bust, used for the visual effect")]
    public float BustSize = 0;
    [Tooltip("How fast the BustSize catches up to the TargetBustSize")]
    public float BustSizeSmoothMultipler = 3;
    [Range(0, 100)]
    public float Fullness = 0;     ///In Percent, how full is the barsita with milk
    [Range(0, 100)]
    public float Happiness = 100;
    public bool Clothed = true;

    [Header("Upgrades")]
    public bool UpgradeCanGrow = false;
    [ReadOnly]
    public int Upgradeshappiness = 0;
    public float UpgradesHappinessValue = 20;
    [ReadOnly]
    public int UpgradesMaxSize = 0;
    public float UpgradesMaxSizeValue = 5;
    private float UpggradeInitialValueMaxSize = 0;
    [ReadOnly]
    public int UpgradesProductionRate = 0;
    public float UpgradesProductionRateValue = 2;
    [ReadOnly]
    public int UpgradesMilkFullness = 0;
    public float UpgradesMilkFullnesPercentage = 10;
    [Tooltip("How much time should be waited to add the value")][Min(0)]
    public float UpgradeMilkFullnessValueAddOffset = 1;
    [ReadOnly]
    public int UpgradesFullTolerance = 0;
    //public float UpgradesFullTolleranceValue = 2;
    //private float UpggradeInitialValueTollerance = 0;

    [Header("Events")]
    public bool GameEndSequence = false;
    public float GameEndSequenceDuration = 10;
    public bool EventFastMilkFill = false;
    public float EventFastMilkFillMultipler = 2;
    public bool EventMilkBurst = false;
    public bool EventMilkBurstShrinkBack = false;
    [ReadOnly]
    public float EventMilkBurstBustExpanded = 0;
    public float EventMilkBurstGrowTickTime = 5;
    public float EventMilkBurstGrowTickSize = 2;
    private float EventMilkBurstNextTickTime = 0;
    public bool EventMoodChange = false;
    public float EventMoodToChange = 1;
    public float EventMoodChangeTickTime = 4;



    [Header("Etc. / Info")]
    [ReadOnly]
    public bool TryMilking = false;
    [ReadOnly]
    public bool BeeingMilked = false;
    [ReadOnly]
    public bool MilkingFailed = false;
    [ReadOnly]
    private bool StartedHeadpatting = false;

    [ReadOnly]
    public AnimationCurve BustHappinessDecreaseTolerance;

#if UNITY_EDITOR
    [ReadOnly]
    public float CurrentHappinessDecreasePerFrame = 0;
#endif

    private float winBustDize = 0; // this value is needed if the game is won to evenly reduce the bust

    private float TimeToReachProductionRate = 60; // i calc here in seconds,60 = if Productionrate is 10 then 10 Bust will be reached in 60s

    private float EventMoodUntilTickTime = 0;


    public static BaseGameMode instance;

    private SoundEffectManager soundEffectManager;
    private StatisticsHolder statisticsHolder;
    private BaristaTalkManager baristaTalkManager;

    protected void Awake()
    {
        instance = this;

        UpggradeInitialValueMaxSize = CurrentMaxSize;


        CalcBust();
    }

    // Start is called before the first frame update
    public void Start()
    {
        statsManager = StatsManager.instance;
        baristaController = BaristaController.instance;
        cupController = CupController.instance;
        soundEffectManager = SoundEffectManager.instance;
        statisticsHolder = StatisticsHolder.instance;
        baristaTalkManager = BaristaTalkManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        MyUpdate();
    }

    [BurstCompile]
    public virtual void MyUpdate()
    {
        if (GameEndSequence == false)
        {
            BustSize = Mathf.Lerp(BustSize, TargetBustSize, Time.deltaTime * BustSizeSmoothMultipler);

            Fullness = (TargetBustSize / CurrentMaxSize) * 100;

            if (TryMilking == true)
            {


                if (TargetBustSize > 0.01f && MilkingFailed == false)
                {
                    BeeingMilked = true;
                }
                else
                {
                    MilkingFailed = true;
                    BeeingMilked = false;
                    //Show Only Clothes?
                }

                Clothed = false;
            }
            else
            {
                BeeingMilked = false;
                MilkingFailed = false;
                Clothed = true;
            }

            if (BeeingMilked == true)
            {
                TargetBustSize = TargetBustSize - (Time.deltaTime * MilkSpeed);

                FillCupBreastMilk();
                ChangeHappyness(Time.deltaTime * HappynessIncreaseFromMilkingPerSecond);
                //Happiness = Happiness + (Time.deltaTime * HappynessIncreaseFromMilkingPerSecond);

                baristaController.BeeingMilked = true;
                baristaTalkManager.TryBaristaEventCupAddBreastMilk();
            }
            else
            {
                if (EventMilkBurst == true && UpgradeCanGrow == true)
                {
                    if (Time.timeSinceLevelLoad > EventMilkBurstNextTickTime)
                    {
                        EventMilkBurstNextTickTime = Time.timeSinceLevelLoad + EventMilkBurstGrowTickTime;

                        if (TargetBustSize < MaxSize)
                        {
                            baristaController.DoMiniSupriseGrowth();

                            if (EventMilkBurstShrinkBack == true)
                            {
                                EventMilkBurstBustExpanded = EventMilkBurstBustExpanded + EventMilkBurstGrowTickSize;
                            }

                            //Let her Grow Muhahahha!
                            //CurrentMaxSize = CurrentMaxSize + EventMilkBurstGrowTickSize;
                            Fullness = Fullness + EventMilkBurstGrowTickSize;
                            TargetBustSize = TargetBustSize + EventMilkBurstGrowTickSize;
                        }
                    }
                }
                else if (EventMilkBurstShrinkBack == true && EventMilkBurstBustExpanded > 0)
                {
                    if (Time.timeSinceLevelLoad > EventMilkBurstNextTickTime)
                    {
                        EventMilkBurstNextTickTime = Time.timeSinceLevelLoad + EventMilkBurstGrowTickTime;

                        EventMilkBurstBustExpanded = EventMilkBurstBustExpanded - EventMilkBurstGrowTickSize;
                        //CurrentMaxSize = CurrentMaxSize - EventMilkBurstGrowTickSize;
                        TargetBustSize = TargetBustSize - EventMilkBurstGrowTickSize;

                        //if (CurrentMaxSize < 0)
                        //{
                        //    CurrentMaxSize = 0;
                        //}

                        if (TargetBustSize < 0)
                        {
                            TargetBustSize = 0;
                        }
                    }
                }

                //Stats Clac
                if (UpgradeCanGrow == true && Happiness > HappynessNeededForProduce)
                {
                    CalcBust();
                }

                //If not milked decrease the happyness
                if (UpgradesFullTolerance <= 0)
                {
                    if ((WhileHeadpatingDontDecreaseHappyness == true && StartedHeadpatting == true) == false)  //if not getting headpated
                    {
                        Happiness = Happiness - (Time.deltaTime * BustHappynessDecrease.Evaluate(Fullness));
                    }


#if UNITY_EDITOR
                    CurrentHappinessDecreasePerFrame = BustHappynessDecrease.Evaluate(Fullness);
#endif
                }
                else
                {
                    if ((WhileHeadpatingDontDecreaseHappyness == true && StartedHeadpatting == true) == false) //if not getting headpated
                    {
                        float abzug = (BustHappynessDecrease.Evaluate(Fullness));

                        for (int i = 0; i < UpgradesFullTolerance; i++)
                        {
                            abzug = (abzug / 100) * (100 - BustHappinessDecreaseTolerance.Evaluate(Fullness));
                        }

                        Happiness = Happiness - (Time.deltaTime * abzug);

#if UNITY_EDITOR
                        CurrentHappinessDecreasePerFrame = abzug;
#endif

                    }
                }

                baristaController.BeeingMilked = false;
            }

            //Set CustomerDialog Animation
            CustomerDialogAnimator.SetBool("Milking", BeeingMilked);

            if (Happiness < 0)
            {
                Happiness = 0;
            }
            else if (Happiness > 100)
            {
                Happiness = 100;
            }

            //CheckForLevelUp();
        }
        else //If GameOverSequence
        {
            if (winBustDize == 0)
            {
                winBustDize = BustSize;
            }

            BustSize = BustSize - ((winBustDize / GameEndSequenceDuration) * Time.deltaTime);
        }

        if (EventMoodChange == true)
        {
            if (Time.timeSinceLevelLoad > EventMoodUntilTickTime)
            {
                ChangeHappyness(EventMoodToChange);
                EventMoodUntilTickTime = Time.timeSinceLevelLoad + EventMoodChangeTickTime;
            }
        }

        UpdateOtherComponents();
    }

    public virtual void FixedUpdate()
    {
        
    }


    public void AddExp(float value)
    {
        ExpCurrent = ExpCurrent + value;
        //CheckForLevelUp(); //Removed to not trigger the effect which are used with it
    }

    private void CheckForLevelUp()
    {
        if (CanLevelUp == true)
        {
            if (ExpCurrent > ExpNeedToNextLevel)
            {
                ExpCurrent -= ExpNeedToNextLevel;
                Level++;
                soundEffectManager.PlayLevelUpEffect();

                ExpNeedToNextLevel = ((Level * Level) / 2) * 3; //New Level exp needed formula! 3,6,13.5,24,37.5,54,73.5,96,121.5,...
            }
        }
    }

    [BurstCompile]
    public void CalcBust()
    {
        double valueToIncrease = ProductionRate * (Time.deltaTime / TimeToReachProductionRate);

        if (EventFastMilkFill == true)
        {
            valueToIncrease = valueToIncrease * EventFastMilkFillMultipler;
        }

        TargetBustSize = TargetBustSize + Statics.DoubleToFloat(valueToIncrease);

        if (TargetBustSize > CurrentMaxSize)
        {
            TargetBustSize = CurrentMaxSize;
        }
        else
        {
            if (statisticsHolder != null)
            {
                statisticsHolder.AddMilk(valueToIncrease);
            }
        }
    }

    [BurstCompile]
    protected void UpdateOtherComponents()
    {
        statsManager.Funds = Money;
        statsManager.CanGrow = UpgradeCanGrow;
        statsManager.MaxSize = CurrentMaxSize;
        statsManager.ProductionRate = ProductionRate;

        baristaController.BustSize = BustSize / 1000;

        baristaController.Fullness = Fullness / 100;
        statsManager.MilkFill = Fullness;

        statsManager.CurrentBust = BustSize;

        baristaController.Happiness = Happiness / 100;
        statsManager.Happiness = Happiness;

        baristaController.Clothed = Clothed;

        statsManager.Level = Level;
        statsManager.ExpPercent = ExpCurrent / ExpNeedToNextLevel;
        statsManager.CanLevelUp = CanLevelUp;
    }

    [System.Obsolete("Method BuyUpgrade is deprecated")]
    public void BuyUpgrade(int costs,int happiness = 0, int maxSize = 0, int productionRate = 0)
    {
        Debug.Log("Upgrade:" + costs + " || " + happiness + " || " + maxSize + " || " + productionRate);
        //Onetime Changes
        SubMoney(costs);
        //Money = Money - costs;

        if (happiness != 0)
        {
            Upgradeshappiness = Upgradeshappiness + happiness;
            this.Happiness = this.Happiness + ((Upgradeshappiness * UpgradesHappinessValue) / 100);
        }

        //Permanent Changes
        if (maxSize != 0)
        {
            UpgradesMaxSize = UpgradesMaxSize + maxSize;
            this.CurrentMaxSize = this.CurrentMaxSize + (UpgradesMaxSize * UpgradesMaxSizeValue);

            if (CurrentMaxSize > MaxSize)
            {
                CurrentMaxSize = MaxSize;
            }
            else if(CurrentMaxSize < 0)
            {
                CurrentMaxSize = 0;
            }
        }

        if (productionRate != 0)
        {
            UpgradesProductionRate = UpgradesProductionRate + productionRate;
            this.ProductionRate = this.ProductionRate + (UpgradesProductionRate * UpgradesProductionRateValue);
        }
    }

    //Espresso,
    //Coffee,
    //Chocolate,
    //Tea,
    //Milk,
    //BreastMilk,
    //Cream,
    //Sugar

    [BurstCompile]
    public void BuyUpgradeProduction(int value = 1)
    {
        Debug.Log("Buy Upgrade: Production," + value);

        if (UpgradesProductionRate + value <= 0)
        {
            UpgradesProductionRate = 0;
        }
        else
        {
            UpgradesProductionRate = UpgradesProductionRate + value;
        }

        if (value > 0)
        {
            for (int i = 0; i < value; i++)
            {
                ProductionRate = ProductionRate * UpgradesProductionRateValue;
            }
            //EffectProductionUpgrade?.PlayFeedbacks();
        }
        else
        {
            for (int i = 0; i < -value; i++)
            {
                ProductionRate = ProductionRate / UpgradesProductionRateValue;
            }
            //EffectProductionDowngrade?.PlayFeedbacks();
        }


        if (ProductionRate < 0)
        {
            ProductionRate = 0;
        }
        else
        {
            //Adding Ceiling to the value
            ProductionRate = math.ceil(ProductionRate);
        }

        if(value > 0)
        {
            statsManager.AddProductionRate();
        }
    }

    [BurstCompile]
    public void BuyMaxSize(int value = 1)
    {
        Debug.Log("Buy Upgrade: MaxSize," + value);

        if (UpgradesMaxSize + value <= 0)
        {
            UpgradesMaxSize = 0;
        }
        else
        {
            UpgradesMaxSize = UpgradesMaxSize + value;
        }
        CurrentMaxSize = UpggradeInitialValueMaxSize + (UpgradesMaxSize * UpgradesMaxSizeValue);

        if (CurrentMaxSize > MaxSize)
        {
            CurrentMaxSize = MaxSize;
        }
        else if (CurrentMaxSize < 0.0002f)
        {
            CurrentMaxSize = 0.0002f;
        }

        if (value > 0)
        {
            statsManager.AddMaxSize();
            //EffectMaxSizeUpgarde?.PlayFeedbacks();
        }
    }

    [BurstCompile]
    public void BuyHappyness(int value = 1)
    {
        Debug.Log("Buy Upgrade: Happyness," + value);
        Upgradeshappiness = Upgradeshappiness + value;
        Happiness = Happiness + (UpgradesHappinessValue * value);

        if (value > 0)
        {
            statsManager.AddHappyness();
            //EffectHappynessIncrease?.PlayFeedbacks();
        }

        if (Happiness < 0)
        {
            Happiness = 0;
        }
        else if (Happiness > 100)
        {
            Happiness = 100;
        }
    }

    [BurstCompile]
    public void BuyMilkFullness(int value = 1,bool DoSUpriseGrowthAnimation = false)
    {
        Debug.Log("Buy Upgrade: Fullness," + value + " Suprise? " + DoSUpriseGrowthAnimation);
        //BustSize = BustSize + ((CurrentMaxSize / 100) * (UpgradesMilkFullnesPercentage * value));
        StartCoroutine(AddMilkFullness(value, UpgradeMilkFullnessValueAddOffset));

        if (DoSUpriseGrowthAnimation == true)
        {
            baristaController.DoSupriseGrowth();
        }

        //EffectFillMilk?.PlayFeedbacks();
    }

    [BurstCompile]
    public void BuyTolerance(int UpgradedTimes, AnimationCurve ToleranceCurve)
    {
        Debug.Log("Buy Upgrade: Tolerance," + UpgradedTimes);
        if (UpgradesFullTolerance + UpgradedTimes <= 0)
        {
            UpgradesFullTolerance = 0;
        }
        else
        {
            UpgradesFullTolerance = UpgradedTimes;
        }
        BustHappinessDecreaseTolerance = ToleranceCurve;

        //EffectFullnesTolleranceUpgrade?.PlayFeedbacks();
    }

    [BurstCompile]
    public IEnumerator AddMilkFullness(int value,float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);
        TargetBustSize = TargetBustSize + ((CurrentMaxSize / 100) * (UpgradesMilkFullnesPercentage * value));

        if (TargetBustSize > CurrentMaxSize)
        {
            TargetBustSize = CurrentMaxSize;
        }
        else if (TargetBustSize < 0)
        {
            TargetBustSize = 0;
        }
    }



    public void FillCupEspresso()
    {
        cupController.FillCup(Fillings.Espresso);
    }

    public void FillCupCoffee()
    {
        cupController.FillCup(Fillings.Coffee);
    }

    public void FillCupChocolate()
    {
        cupController.FillCup(Fillings.Chocolate);
    }

    public void FillCupTea()
    {
        cupController.FillCup(Fillings.Tea);
    }

    public void FillCupMilk()
    {
        cupController.FillCup(Fillings.Milk);
    }

    public void FillCupBreastMilk()
    {
        cupController.FillCup(Fillings.BreastMilk);
    }

    public void FillCupCream()
    {
        cupController.FillCup(Fillings.Cream);
    }

    public void FillCuSugar()
    {
        cupController.FillCup(Fillings.Sugar);
    }

    [BurstCompile]
    public void AddMoney(float amount)
    {
        float newAmount = (Mathf.Round(amount * 100) / 100);//so we have allways a nice 3 decimal number like 2.40
        Money = Money + newAmount;

        //Animation
        statsManager.AddMoney(newAmount);

        Debug.Log("Money Add: " + newAmount);
    }

    public void SubMoney(float amount)
    {
        float newAmount = (Mathf.Round(amount * 100) / 100);//so we have allways a nice 3 decimal number like 2.40
        Money = Money - newAmount;

        //Animation
        statsManager.SubMoney(newAmount);

        Debug.Log("Money Sub: " + newAmount);
    }

    public void ChangeHappyness(float amount)
    {
        if (amount > 0)
        {
            AddHappyness(amount);
        }
        else
        {
            SubHappyness(Mathf.Abs(amount));
        }
    }

    public void AddHappyness(float amount)
    {
        Happiness = Happiness + amount;
        statsManager.AddHappyness();
    }

    public void SubHappyness(float amount)
    {
        Happiness = Happiness - amount;
        statsManager.SubHappyness();
    }

    public void SetBustToMax()
    {
        TargetBustSize = MaxSize;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    public void BaristaStartHeadpatting()
    {
        if (StartedHeadpatting == false)
        {
            StartedHeadpatting = true;
            AddHappyness(HappynessIncreaseHeadPatOneTime);

            if (HappynessIncreaseHeadPatTickTime > 0.03f)
            {
                StartCoroutine(HeadpatHappynessIncreaseOverTimeRoutine(HappynessIncreaseHeadPatTickTime));
            }
            else
            {
                Debug.LogWarning("Tick time to low!");
            }
        }
    }

    public void BaristaStoptHeadpatting()
    {
        StartedHeadpatting = false;
        StopCoroutine(HeadpatHappynessIncreaseOverTimeRoutine());
    }

    [BurstCompile()]
    private IEnumerator HeadpatHappynessIncreaseOverTimeRoutine(float tickTime = 1)
    {
        while (StartedHeadpatting == true)
        {
            yield return new WaitForSeconds(tickTime);
            if (StartedHeadpatting == true)
            {
                AddHappyness(HappynessIncreaseHeadPatOverTime);
            }
        }
    }

}
