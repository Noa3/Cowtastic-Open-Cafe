using Unity.Burst;
using UnityEngine;

/// <summary>
/// This script will be added to the Coffee-mashines and Milk/-other Bottles
/// Manages The unlocking logic and what happens if its Pressed
/// </summary>

public class FillingTool : MonoBehaviour
{
    [Header("References")]
    public GameObject ToolLocked;
    public GameObject ToolUnlocked;
    [Tooltip("Get Played when the ingredient gets Unlocked")]
    public SoundEffectVariation UnlockSoundVariation;
    public bool UnlockSoundVariationChangePitch = false;

    [Tooltip("Play Toppings as OneShot while Ingreedens are looping")]
    public SoundEffectVariation SoundVariation;

    [Header("Settings")]
    public bool isTopping = false;

    public Toppings CupToppings = Toppings.Boba;

    public Fillings MashineFilling = Fillings.Coffee;

    //public int LevelToUnlock = 1;
    public int MoneyToUnlock = 50;

    public bool Unlocked = true;

    [Header("Logic")]
    public bool IgnoreJustUnlockedLogic = true;

    private bool CupFilledWithTopping = false;

    private bool FillCup = false;
    private BaseGameMode gamemode;
    private bool justUnlock = false;
    private UpgradeManager upgradeManager;



    private void Awake()
    {
        SetUnlockedState(Unlocked);
    }

    // Start is called before the first frame update
    void Start()
    {
        gamemode = BaseGameMode.instance;
        upgradeManager = UpgradeManager.Instance;

        if (gamemode == null || upgradeManager == null)
        {
            enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MyUpdate();
    }

    [BurstCompile]
    private void MyUpdate()
    {
        if (upgradeManager != null && upgradeManager.UpgardePanelVisibility == false && FillCup == true && Unlocked == true && CupFilledWithTopping == false)
        {
            //Debug.Log("Mouse!");
            if (isTopping == true)
            {
                gamemode.cupController.FillCup(CupToppings);
                SoundVariation.PlayRandomOneShot(true); //Play Toppings OneShot
                CupFilledWithTopping = true;
            }
            else
            {
                gamemode.cupController.FillCup(MashineFilling);
                SoundVariation.PlayRandomLoop(); //Play Toppings OneShot
            }
        }
    }

    //private void FixedUpdate()
    //{
    //    if (gamemode.Level >= LevelToUnlock)
    //    {
    //        Unlocked = true;
    //    }
    //    else
    //    {
    //        Unlocked = false;
    //    }
    //    SetUnlockedState(Unlocked);
    //}

    [BurstCompile]
    private void UnlockTool()
    {
        if (gamemode != null && gamemode.Money >= MoneyToUnlock)
        {
            //gamemode.Money = gamemode.Money - MoneyToUnlock;
            gamemode.SubMoney(MoneyToUnlock);

            //Play Animation & Sound?
            if (UnlockSoundVariation != null)
            {
                UnlockSoundVariation.PlayRandomOneShot(UnlockSoundVariationChangePitch);
            }

            SetUnlockedState(true);
        }
    }

    [BurstCompile]
    public void SetUnlockedState(bool unlocked)
    {
        Unlocked = unlocked;
        ToolLocked.SetActive(!unlocked);
        ToolUnlocked.SetActive(unlocked);
    }

    [BurstCompile]
    public void TryUnlock()
    {
        if (Unlocked == false)
        {
            UnlockTool();
            if (IgnoreJustUnlockedLogic == false)
            {
                justUnlock = true;
            }

        }
    }

    void OnMouseDown()
    {

        if (upgradeManager != null && upgradeManager.UpgardePanelVisibility == false)
        {
            if (IgnoreJustUnlockedLogic == true)
            {
                if (Unlocked == true)
                {
                    FillCup = true;
                }
            }
            else
            {
                if (Unlocked == true && justUnlock == false)
                {
                    FillCup = true;
                }
                /*else
                {
                    UnlockTool();
                    justUnlock = true;
                }*/
            }

        }
    }

    private void OnMouseDrag()
    {
        if (Unlocked == true && justUnlock == false)
        {
            FillCup = true;
        }
    }

    private void OnMouseUp()
    {
        FillCup = false;

        if (IgnoreJustUnlockedLogic == false)
        {
            justUnlock = false;
        }

        SoundVariation.EndLoop();
        CupFilledWithTopping = false;
    }

}
