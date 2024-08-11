using System.Collections.Generic;
using UnityEngine;

public class MilkTypeController : MonoBehaviour
{
    public static MilkTypeController instance;

    public int lastMilkPreset;

    public UnityEngine.UI.Button[] buttons;

#if UNITY_EDITOR
    // This is an editor-only function to populate the buttons array.
    // Each scene currently has enough overrides of other scenes and prefabs
    // That it makes sense to assign buttons programmatically rather than manually.
    // Rather than adding button listeners one by one through the unity editor,
    // We use an editor only function to build a reference list of buttons
    // and then during OnAwake, add a listener to for each button.

    // To run this function, click the "Find Buttons" button in the custom
    // inspector for MilkTypeController. Note that since it is using GameObject.Find,
    // Buttons (and their Unity parents) need to be enabled to be found, though
    // they can be disabled again right after.
    public void FindButtons()
    {
        List<string> objectsToFind = new List<string>();
        objectsToFind.Add("Milk Types Box/Basic");
        objectsToFind.Add("Milk Types Box/Thick/Thick Unlocked");
        objectsToFind.Add("Milk Types Box/Creamy/Creamy");
        objectsToFind.Add("Milk Types Box/Chocolate/Chocolate");
        objectsToFind.Add("Milk Types Box/Strawberry/Strawberry");
        objectsToFind.Add("Milk Types Box/Honey/Honey");
        objectsToFind.Add("Milk Types Box/Blue/Blue");
        objectsToFind.Add("Milk Types Box/Green/Green");
        objectsToFind.Add("Milk Types Box/Raspberry/Raspberry");
        objectsToFind.Add("Milk Types Box/Rainbow/Rainbow");
        objectsToFind.Add("Milk Types Box/Space/Space");
        objectsToFind.Add("Milk Types Box/Void/Void");
        buttons = new UnityEngine.UI.Button[objectsToFind.Count];
        for (int i = 0; i < objectsToFind.Count; i++)
        {
            var go = GameObject.Find(objectsToFind[i]);
            buttons[i] = go.GetComponent<UnityEngine.UI.Button>();
        }
    }
#endif

    private void AttachToCustomizationMenuButtons()
    {
        for (int i=0; i< buttons.Length; i++)
        {
            var presetNumber = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => MilkPresetButtonClicked(presetNumber));
        }
    }

    void Awake()
    {
        instance = this;
        AttachToCustomizationMenuButtons();
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

    public void MilkPresetButtonClicked(int preset)
    {
        SetMilkByPresetAndSavePreference(preset);
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