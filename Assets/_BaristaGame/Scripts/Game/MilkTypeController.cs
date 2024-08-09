using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;

public class MilkTypeController : MonoBehaviour
{
    public static MilkTypeController instance;

    public int lastMilkPreset;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // LoadPreferences leads to a usage of the "instance" handles for CupController, BaristaController, and MilkWave.
        // The "instance" handles are initialized during the Awake() functions of those classes.
        // Therefore, to guarantee we don't access those handles prior to their initialization,
        // LoadPreference() is called in Start() which is guaranteed to run after all the Awake() calls are done.
        LoadPreference();
    }

    public void LoadPreference()
    {
        int preset = PlayerPrefs.GetInt(Consts.PlayerPrefMilkPreset, 0);
        SetMilkByPreset(preset);
    }

    public void SetMilkByPresetAndSavePreference(int preset)
    {
        SetMilkByPreset(preset);
        SaveLastSetMilkAsPreference();
    }

    public void SetMilkByPreset(int preset = 0)
    {
        lastMilkPreset = preset;

        int milkType;

        // The non-default cases in this switch statement have special effects on them (like rainbow).
        // All other milks are recolors of type 0.
        switch (preset)
        {
            default:
                milkType = 0;
                break;
            case 4:
                // Strawberry
                milkType = 3;
                break;
            case 5:
                // Honey
                milkType = 2;
                break;
            case 9:
                //Rainbow
                milkType = 4;
                break;
            case 10:
                //Space
                milkType = 1;
                break;
        }

        Color milkColor;

        // All presets with special effects default to white (their coloration is handled by the effects)
        switch (preset)
        {
            default:
                milkColor = Color.white;
                break;
            case 1:
                //Thick
                milkColor = Statics.MilkColor_Thick;
                break;
            case 2:
                //Creamy
                milkColor = Statics.MilkColor_Creamy;
                break;
            case 3:
                //Chocolate
                milkColor = Statics.MilkColor_Chocolate;
                break;
            case 6:
                //Blue
                milkColor = Statics.MilkColor_Blue;
                break;
            case 7:
                //Green
                milkColor = Statics.MilkColor_Green;
                break;
            case 8:
                //Raspberry
                milkColor = Statics.MilkColor_Raspberry;
                break;
            case 11:
                //Void
                milkColor = Statics.MilkColor_Void;
                break;
        }

        CupController.instance.SetMilkTypeAndColor(milkType, milkColor);
        BaristaController.instance.SetMilkTypeAndColor(milkType, milkColor);
        MilkWave.instance.SetMilkTypeAndColor(milkType, milkColor);
    }

    public void SaveLastSetMilkAsPreference()
    {
        PlayerPrefs.SetInt(Consts.PlayerPrefMilkPreset, lastMilkPreset);
    }
}

public enum MilkTypes
{
    Milk = 0,
    Galaxy = 1,
    Lava = 2,
    Raspberry = 3,
    Rainbow = 4,
    Strawberry = 5,
}