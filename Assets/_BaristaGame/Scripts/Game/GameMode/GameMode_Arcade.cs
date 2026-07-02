using UnityEngine;
using Unity.Burst;

/// <summary>
/// Arcade game mode implementation for the Barista Game
/// Handles both normal and milky mode gameplay mechanics including automatic upgrades
/// Optimized for Unity 6.1 with Burst compilation support
/// </summary>
public class GameMode_Arcade : BaseGameMode
{
    #region Serialized Fields

    [Header("Arcade Gamemode Settings")]
    public bool isMilkyMode = false;

    [Header("Milkymode Settings")]
    [Tooltip("Minimum time in seconds until next automatic upgrade")]
    public float TimeUntilNextUpgradeMin = 60f;

    [Tooltip("Maximum time in seconds until next automatic upgrade")]
    public float TimeUntilNextUpgradeMax = 120f;

    [Tooltip("This overrides the starting ProductionRate for Milk")]
    public float OverrideStartMilkProductionRate = 1f;

    [Tooltip("This overrides the starting Upgrade rate for the Production, on the next Upgrade")]
    public float OverrideProductionRateUpgradeValue = 2.5f;

    [Tooltip("This will multiply the next upgrade for the productionrate to make an ever faster gameplay")]
    public float ProductionrateUpgradeMultipler = 1.05f;

    [Header("Debug")]
    public bool ForceMilkyMode = false;

    [ReadOnly]
    public float NextTimeUpgrade = 0f;

    #endregion

    #region Private Fields

    private UpgradeManager upgradeManager;
    private CafeVisualsController cafeVisualsController;

    #endregion

    #region Unity Lifecycle

    public new void Awake()
    {
        base.Awake();
        InitializeMilkyModeSettings();
    }

    public new void Start()
    {
        base.Start();
        CacheComponentReferences();

        if (isMilkyMode)
        {
            InitMilkyMode();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isMilkyMode && Statics.IsTimeForNextUpgrade(Time.timeSinceLevelLoad, NextTimeUpgrade))
        {
            ProcessMilkyModeUpgrade();
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initializes milky mode settings based on preferences or debug flags
    /// </summary>
    [BurstCompile]
    private void InitializeMilkyModeSettings()
    {
        isMilkyMode = ForceMilkyMode || Statics.LoadBoolPreference(Consts.PlayerPrefNextIsMilkyMode);
    }

    /// <summary>
    /// Caches component references for better performance
    /// </summary>
    private void CacheComponentReferences()
    {
        upgradeManager = Statics.FindObjectOfTypeSafe<UpgradeManager>();
        cafeVisualsController = Statics.FindObjectOfTypeSafe<CafeVisualsController>();
    }

    /// <summary>
    /// Initializes milky mode specific settings and visual effects
    /// </summary>
    private void InitMilkyMode()
    {
        CalcNextTimeMilkyUpgrade();
        ConfigureMilkyModeProductionRates();
        SetupMilkyModeUpgrades();
        EnableMilkyModeVisuals();
    }

    /// <summary>
    /// Configures production rates for milky mode
    /// </summary>
    [BurstCompile]
    private void ConfigureMilkyModeProductionRates()
    {
        ProductionRate = OverrideStartMilkProductionRate;
        UpgradesProductionRateValue = OverrideProductionRateUpgradeValue;
    }

    /// <summary>
    /// Sets up initial upgrades for milky mode
    /// </summary>
    private void SetupMilkyModeUpgrades()
    {
        if (upgradeManager != null)
        {
            upgradeManager.BuyInitialUpgarde();
            upgradeManager.SetPanelActive(false);
        }
        else
        {
            Statics.LogWarningSafe("UpgradeManager not found during milky mode initialization");
        }
    }

    /// <summary>
    /// Enables visual effects for milky mode
    /// </summary>
    private void EnableMilkyModeVisuals()
    {
        if (cafeVisualsController != null)
        {
            cafeVisualsController.SetStatsLightning(true);
        }
        else
        {
            Statics.LogWarningSafe("CafeVisualsController not found during milky mode initialization");
        }
    }

    /// <summary>
    /// Processes automatic upgrades in milky mode
    /// </summary>
    private void ProcessMilkyModeUpgrade()
    {
        DoMilkyModeStepUp();
        CalcNextTimeMilkyUpgrade();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Calculates the next time for a milky mode upgrade
    /// </summary>
    [BurstCompile]
    public void CalcNextTimeMilkyUpgrade()
    {
        NextTimeUpgrade = Statics.CalculateNextUpgradeTime(
            Time.timeSinceLevelLoad,
            TimeUntilNextUpgradeMin,
            TimeUntilNextUpgradeMax
        );
    }

    /// <summary>
    /// Performs a milky mode step up by buying production upgrades
    /// </summary>
    [BurstCompile]
    public void DoMilkyModeStepUp()
    {
        // Note: Commented line preserved for potential future use
        // UpgradesProductionRateValue = UpgradesProductionRateValue * ProductionrateUpgradeMultipler;
        BuyUpgradeProduction(Consts.GameModeArcade.MilkyModeUpgradeAmount);
    }

    #endregion
}