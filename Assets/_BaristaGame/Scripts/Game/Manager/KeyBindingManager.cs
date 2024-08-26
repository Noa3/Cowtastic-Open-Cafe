using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class KeyBindingManager : MonoBehaviour
{
    public static KeyBindingManager instance;

    private bool keyBindingEnabled = true;
    private KeyCode currentKeyCode = KeyCode.None;
    private KeyCode queuedKeyCode = KeyCode.None;
    private FillingTool fillingTool;
    private UnityEngine.UI.Image fillingToolGlow;

    public enum BindableActions
    {
        headpat, breastMilk,
        boba, sprinkles, caramelSauce, chocolateSauce,
        coffee, tea, espresso, sugar, chocolate,
        milk, cream, whippedCream, ice,
        finishOrder, resetCup,

        productionUpgrade, sizeUpgrade, happinessPurchase,
        productionDowngrade, milkNowUpgrade, toleranceUpgrade,
        initialUpgrade
    }

    private Dictionary<KeyCode, BindableActions> bindings = new Dictionary<KeyCode, BindableActions>();

    public void EnableKeyBinding()
    {
        keyBindingEnabled = true;
    }

    public void DisableKeyBinding()
    {
        StopCurrentAction();
        keyBindingEnabled = false;
    }

    private void Awake()
    {
        instance = this;

        if (!keyBindingEnabled)
        {
            return;
        }

        LoadKeyBindings();
    }

    private void Start()
    {
        var buttonUpgrades = FindObjectsOfType<ButtonUpgrade>(true);
        foreach (var item in buttonUpgrades)
        {
            item.Initialize();
        }
    }

    private void LoadKeyBindings()
    {
        //KeyCode code = (KeyCode)System.Enum.Parse(typeof(KeyCode), item.Key);
        bindings.Add(KeyCode.S, BindableActions.headpat);
        bindings.Add(KeyCode.X, BindableActions.breastMilk);

        bindings.Add(KeyCode.T, BindableActions.boba);
        bindings.Add(KeyCode.Y, BindableActions.sprinkles);
        bindings.Add(KeyCode.U, BindableActions.caramelSauce);
        bindings.Add(KeyCode.I, BindableActions.chocolateSauce);
        bindings.Add(KeyCode.F, BindableActions.coffee);
        bindings.Add(KeyCode.G, BindableActions.tea);
        bindings.Add(KeyCode.H, BindableActions.espresso);
        bindings.Add(KeyCode.J, BindableActions.sugar);
        bindings.Add(KeyCode.K, BindableActions.chocolate);
        bindings.Add(KeyCode.V, BindableActions.milk);
        bindings.Add(KeyCode.B, BindableActions.cream);
        bindings.Add(KeyCode.N, BindableActions.whippedCream);
        bindings.Add(KeyCode.M, BindableActions.ice);

        bindings.Add(KeyCode.Space, BindableActions.finishOrder);
        bindings.Add(KeyCode.Backspace, BindableActions.resetCup);

        bindings.Add(KeyCode.Alpha1, BindableActions.productionUpgrade);
        bindings.Add(KeyCode.Alpha2, BindableActions.sizeUpgrade);
        bindings.Add(KeyCode.Alpha3, BindableActions.happinessPurchase);
        bindings.Add(KeyCode.Alpha4, BindableActions.productionDowngrade);
        bindings.Add(KeyCode.Alpha5, BindableActions.milkNowUpgrade);
        bindings.Add(KeyCode.Alpha6, BindableActions.toleranceUpgrade);
    }

    private bool IsButtonUpgrade(BindableActions bindableAction)
    {
        switch (bindableAction)
        {
            default:
                return false;
            case BindableActions.productionUpgrade:
                return true;
            case BindableActions.sizeUpgrade:
                return true;
            case BindableActions.happinessPurchase:
                return true;
            case BindableActions.productionDowngrade:
                return true;
            case BindableActions.milkNowUpgrade:
                return true;
            case BindableActions.toleranceUpgrade:
                return true;
        }
    }

    private void StartAction(KeyCode keyCode)
    {
        currentKeyCode = keyCode;
        if (bindings[currentKeyCode] == BindableActions.breastMilk)
        {
            BaristaMilkingHelper.instance.StartMilking();
            return;
        }
        if (bindings[currentKeyCode] == BindableActions.headpat)
        {
            BaristaHeadPatHelper.instance.StartHeadPat();
            return;
        }
        if (bindings[currentKeyCode] == BindableActions.finishOrder)
        {
            OrderManager.instance.OrderFinished();
            return;
        }
        if (bindings[currentKeyCode] == BindableActions.resetCup)
        {
            CupController.instance.ResetCup();
            BaristaTalkManager.instance.TryBaristaEventCupReset();
            return;
        }

        if (IsButtonUpgrade(bindings[currentKeyCode]))
        {
            var upgrade = ButtonUpgrade.buttonUpgrades[BindableActions.initialUpgrade];
            if (upgrade.UpgradedTimes > 0)
            {
                upgrade = ButtonUpgrade.buttonUpgrades[bindings[currentKeyCode]];
            }
            upgrade.OnClick();
            return;
        }

        fillingTool = FillingTool.fillingTools[bindings[currentKeyCode]];
        fillingToolGlow = FillingTool.fillingToolGlows[bindings[currentKeyCode]];
        if(fillingTool != null)
        {
            fillingTool.StartFillingOrTryUnlock();
            fillingToolGlow.enabled = true;
            return;
        }
    }

    private void StopCurrentAction()
    {
        if(fillingTool != null)
        {
            fillingTool.StopFilling();
            fillingToolGlow.enabled = false;
            fillingTool = null;
            fillingToolGlow = null;
        }
        if (currentKeyCode != KeyCode.None)
        {
            if (bindings[currentKeyCode] == BindableActions.breastMilk)
            {
                BaristaMilkingHelper.instance.StopMilking();
            }
            else if (bindings[currentKeyCode] == BindableActions.headpat)
            {
                BaristaHeadPatHelper.instance.StopHeadPat();
            }
            currentKeyCode = KeyCode.None;
        }
        if (queuedKeyCode != KeyCode.None)
        {
            StartAction(queuedKeyCode);
            queuedKeyCode = KeyCode.None;
        }
    }

    private void ProcessInput()
    {
        if (!keyBindingEnabled)
        {
            queuedKeyCode = KeyCode.None;
            StopCurrentAction();
            return;
        }

        if (currentKeyCode != KeyCode.None && !Input.GetKey(currentKeyCode))
        {
            StopCurrentAction();
        }
        if (queuedKeyCode != KeyCode.None && !Input.GetKey(queuedKeyCode))
        {
            queuedKeyCode = KeyCode.None;
        }

        foreach (var item in bindings)
        {
            if (currentKeyCode != KeyCode.None && queuedKeyCode != KeyCode.None)
            {
                break;
            }

            if(Input.GetKey(item.Key))
            {
                if(currentKeyCode == KeyCode.None)
                {
                    StartAction(item.Key);
                }
                else
                {
                    if(currentKeyCode != item.Key)
                    {
                        queuedKeyCode = item.Key;
                    }
                }
            }
        }
    }

    private void Update()
    {
        ProcessInput();
    }
}
