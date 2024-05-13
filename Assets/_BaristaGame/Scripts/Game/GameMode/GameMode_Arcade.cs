using UnityEngine;

/// <summary>
/// This is the main Script wich says how the current game will run and what are the conditions for applay the stat's
/// </summary>

public class GameMode_Arcade : BaseGameMode
{
    //[Header("Arcade Game Mode Stuff")]

    [Header("Arcade Gamemode Settings")]

    public bool isMilkyMode = false;

    [Header("Milkymode Settings")]
    public float TimeUntilNextUpgradeMin = 60f;
    public float TimeUntilNextUpgradeMax = 120f;

    [Tooltip("This Overides the starting ProductionRate for Milk")]
    public float OverrideStartMilkProductionRate = 1;

    [Tooltip("This Overides the starting Upgrade rate for the Production, on the next Upgrade")]
    public float OverrideProductionRateUpgradeValue = 2.5f;

    [Tooltip("This will multiply the next upgrade for the productionrate to make an ever faster gameplay")]
    public float ProductionrateUpgradeMultipler = 1.05f;


    [Header("Debug")]
    public bool ForceMilkyMode = false;
    [ReadOnly]
    public float NextTimeUpgrade = 0;


    public new void Awake()
    {
        base.Awake();

        if (ForceMilkyMode == true)
        {
            isMilkyMode = true;
        }
        else
        {
            isMilkyMode = PlayerPrefs.GetInt(Consts.PlayerPrefNextIsMilkyMode, 0) == 1 ? true : false;
        }
    }

    public new void Start()
    {
        base.Start();

        if (isMilkyMode == true)
        {
            InitMilkyMode();
        }
    }

    public void InitMilkyMode()
    {
        CalcNextTimeMilkyUpgrade();

        ProductionRate = OverrideStartMilkProductionRate;
        UpgradesProductionRateValue = OverrideProductionRateUpgradeValue;

        UpgradeManager upgradeManager = FindObjectOfType<UpgradeManager>();
        upgradeManager.BuyInitialUpgarde();
        upgradeManager.SetPanelActive(false);


        CafeVisualsController caveVisualsController = FindObjectOfType<CafeVisualsController>();
        caveVisualsController.SetStatsLightning(true);

    }

    public override void FixedUpdate()
    {
        if (isMilkyMode == true && Time.timeSinceLevelLoad > NextTimeUpgrade)
        {
            DoMilkyModeStepUp();
            CalcNextTimeMilkyUpgrade();
        }
    }

    public void CalcNextTimeMilkyUpgrade()
    {
        NextTimeUpgrade = Time.timeSinceLevelLoad + Random.Range(TimeUntilNextUpgradeMin, TimeUntilNextUpgradeMax);
    }

    public void DoMilkyModeStepUp()
    {
        //UpgradesProductionRateValue = UpgradesProductionRateValue * ProductionrateUpgradeMultipler;
        BuyUpgradeProduction(1);
    }

}
