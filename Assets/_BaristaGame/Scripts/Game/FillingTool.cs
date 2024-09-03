using System.Collections.Generic;
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

    private bool CupFilledWithTopping = false;

    private bool FillCup = false;
    private BaseGameMode gamemode;
    private bool justUnlock = false;
    private UpgradeManager upgradeManager;

    public static Dictionary<KeyBindingManager.BindableActions, FillingTool> fillingTools;
    public static Dictionary<KeyBindingManager.BindableActions, UnityEngine.UI.Image> fillingToolGlows;

    private void AddFillingToolToStaticList(KeyBindingManager.BindableActions key)
    {
        fillingTools.Add(key, this);
        fillingToolGlows.Add(key, transform.Find("KeyboardGlow").GetComponent<UnityEngine.UI.Image>());
    }

    private void Awake()
    {
        SetUnlockedState(Unlocked);
        if (fillingTools == null)
        {
            fillingTools = new Dictionary<KeyBindingManager.BindableActions, FillingTool>();
            fillingToolGlows = new Dictionary<KeyBindingManager.BindableActions, UnityEngine.UI.Image>();
        }

        if (isTopping)
        {
            switch(CupToppings)
            {
                default:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.boba);
                    break;
                case Toppings.Sprinkles:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.sprinkles);
                    break;
                case Toppings.CaramelSauce:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.caramelSauce);
                    break;
                case Toppings.ChocolateSauce:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.chocolateSauce);
                    break;
                case Toppings.WhipedCream:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.whippedCream);
                    break;
                case Toppings.Ice:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.ice);
                    break;
            }
        }
        else
        {
            switch (MashineFilling)
            {
                default:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.coffee);
                    break;
                case Fillings.Tea:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.tea);
                    break;
                case Fillings.Espresso:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.espresso);
                    break;
                case Fillings.Sugar:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.sugar);
                    break;
                case Fillings.Chocolate:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.chocolate);
                    break;
                case Fillings.Milk:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.milk);
                    break;
                case Fillings.Cream:
                    AddFillingToolToStaticList(KeyBindingManager.BindableActions.cream);
                    break;
            }
        }
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
        }
    }

    void OnMouseDown()
    {
        if (upgradeManager == null)
            return;

        if (upgradeManager.UpgardePanelVisibility == true)
            return;

        if (Unlocked == false)
            return;

        StartFilling();
        KeyBindingManager.instance.MouseDown();
    }

    private void OnMouseDrag()
    {
        if (Unlocked == true && justUnlock == false)
        {
            FillCup = true;
        }
    }

    public void StartFillingOrTryUnlock()
    {
        if (Unlocked == false)
        {
            UnlockTool();
        }
        else
        {
            StartFilling();
        }
    }

    public void StartFilling()
    {
        FillCup = true;
    }

    public void StopFilling()
    {
        FillCup = false;

        SoundVariation.EndLoop();
        CupFilledWithTopping = false;
    }

    private void OnMouseUp()
    {
        StopFilling();
        KeyBindingManager.instance.MouseUp();
    }

    private void OnDestroy()
    {
        fillingTools = null;
        fillingToolGlows = null;
    }
}
