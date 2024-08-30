using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class KeyBindingManager : MonoBehaviour
{
    public static KeyBindingManager instance;

    private bool mouseIsDown = false;
    private bool gameIsPaused = false;
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

    public void MouseDown()
    {
        mouseIsDown = true;
        UpdateKeyBindingEnable();
    }

    public void MouseUp()
    {
        mouseIsDown=false;
        UpdateKeyBindingEnable();
    }

    public void Paused()
    {
        gameIsPaused = true;
        UpdateKeyBindingEnable();
    }

    public void UnPaused()
    {
        gameIsPaused = false;
        UpdateKeyBindingEnable();
    }

    public void UpdateKeyBindingEnable()
    {
        keyBindingEnabled = true;
        if (mouseIsDown)
        {
            StopCurrentAction();
            keyBindingEnabled = false;
            return;
        }
        if (gameIsPaused)
        {
            StopCurrentAction();
            keyBindingEnabled = false;
            return;
        }
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

    public void LoadKeyBindings()
    {
        bindings = new Dictionary<KeyCode, BindableActions>();
        foreach (BindableActions item in Enum.GetValues(typeof(BindableActions)))
        {
            var playerPrefsKey = "KeyBind:" + item.ToString();
            if (PlayerPrefs.HasKey(playerPrefsKey))
            {
                var keyCodeAsString = PlayerPrefs.GetString(playerPrefsKey);
                KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeAsString);
                bindings.Add(keyCode, item);
            }
        }
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
