using UnityEngine;
using TMPro;
using System.Text;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Localization;

public class CupController : MonoBehaviour
{
    [Header("References")]
    public MeshRenderer CupFiling;
    public Animator CupAnimator;
    public TooltipTrigger CupTooltip;

    public LocalizedString StringEsspresso;
    public LocalizedString StringCoffee;
    public LocalizedString StringCocolateSauce;
    public LocalizedString StringTea;
    public LocalizedString StringRegularMilk;
    public LocalizedString StringBreastMilk;
    public LocalizedString StringVanillaCreamer;
    public LocalizedString StringSugar;
    public LocalizedString StringIce;
    public LocalizedString StringBoba;
    public LocalizedString StringWhipedCream;
    public LocalizedString StringCaramelSauce;
    public LocalizedString StringCocaPowder;
    public LocalizedString StringCandies;

    [Header("Settings")]
    public MilkTypes MilkType = 0;
    public Color MilkColor = Color.white;
    public float FillVolume = 0.2f;
    [Tooltip("This multiples/devides the time for the cup filling from the breast milk")]
    public float BreatsMilkCupFillMultipler = 0.33f;

    //[ReadOnly()]
    [Range(0,1f)]
    public float Fullness = 0;

    [Header("References Optional")]
    public TMP_Dropdown MilkSelector;

    [Header("Filling")]
    [Range(0, 1f)]
    [ReadOnly]
    public float Espresso = 0;
    [Range(0, 1f)]
    [ReadOnly]
    public float Coffee = 0;
    [Range(0, 1f)]
    [ReadOnly]
    public float Chocolate = 0;
    [Range(0, 1f)]
    [ReadOnly]
    public float Tea = 0;
    [Range(0, 1f)]
    [ReadOnly]
    public float Milk = 0;
    [Range(0, 1f)]
    [ReadOnly]
    public float BreastMilk = 0;
    [Range(0, 1f)]
    [ReadOnly]
    public float Cream = 0;
    [Range(0, 1f)]
    [ReadOnly]
    public float Sugar = 0;

    [Header("Extras")]
    public bool Ice = false;
    public bool Boba = false;

    [Header("Toppings")]
    public bool WhippedCream = false;
    public bool CaramelSauce = false;
    public bool ChocolateSauce = false;
    public bool Sprinkles = false;


    private Material cupFillingMaterial;

    public static CupController instance;

    //This will add volume in ml and used to be calc the percentage of cupfilling
    private float volume_Espresso = 0;
    private float volume_Coffee = 0;
    private float volume_Chocolate = 0;
    private float volume_Tea = 0;
    private float volume_Milk = 0;
    private float volume_BreastMilk = 0;
    private float volume_Cream = 0;
    private float volume_Sugar = 0;


    private void Awake()
    {
        instance = this;
        cupFillingMaterial = CupFiling.material;

        if (MilkSelector != null)
        {
            //Debug.Log("Cup: " + PlayerPrefs.GetInt(Statics.PlayerPrefMilkCup, 0));
            MilkSelector.value = PlayerPrefs.GetInt(Consts.PlayerPrefMilkCup,0);

            MilkColor = new Color(PlayerPrefs.GetFloat(Consts.PlayerPrefMilkColorR, MilkColor.r), PlayerPrefs.GetFloat(Consts.PlayerPrefMilkColorG, MilkColor.g), PlayerPrefs.GetFloat(Consts.PlayerPrefMilkColorB, MilkColor.b));


            ChangeMilk(MilkSelector.value);
        }
    }

    private void CreateLocalizationEvents()
    {

    }

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        //Fullness = Espresso + Coffee + Chocolate + Tea + Milk + BreastMilk + Cream + Sugar;
        UpdateShader();

        //Update the Tooltip
        if (CupTooltip != null)
        {
            UpdateTooltip();
        }
    }

    public void UpdateShader()
    {
        cupFillingMaterial.SetFloat(Consts.CupShader_Fullness, Fullness);

        cupFillingMaterial.SetInt(Consts.CupShader_MilkType, (int)MilkType);
        cupFillingMaterial.SetColor(Consts.CupShader_MilkTypeColor, MilkColor);


        cupFillingMaterial.SetFloat(Consts.CupShader_Espresso, Espresso);
        cupFillingMaterial.SetFloat(Consts.CupShader_Coffee, Coffee);
        cupFillingMaterial.SetFloat(Consts.CupShader_Chocolate, Chocolate);
        cupFillingMaterial.SetFloat(Consts.CupShader_Tea, Tea);
        cupFillingMaterial.SetFloat(Consts.CupShader_Milk, Milk);
        cupFillingMaterial.SetFloat(Consts.CupShader_BreastMilk, BreastMilk);
        cupFillingMaterial.SetFloat(Consts.CupShader_Cream, Cream);
        cupFillingMaterial.SetFloat(Consts.CupShader_Sugar, Sugar);

        cupFillingMaterial.SetInt(Consts.CupShader_Ice, Ice ? 1 : 0);
        cupFillingMaterial.SetFloat(Consts.CupShader_Boba, Boba ? 1 : 0);
        cupFillingMaterial.SetFloat(Consts.CupShader_WhippedCream, WhippedCream ? 1 : 0);
        cupFillingMaterial.SetFloat(Consts.CupShader_CaramelSauce, CaramelSauce ? 1 : 0);
        cupFillingMaterial.SetFloat(Consts.CupShader_ChocolateSauce, ChocolateSauce ? 1 : 0);
        cupFillingMaterial.SetFloat(Consts.CupShader_Sprinkles, Sprinkles ? 1 : 0);


    }

    public void UpdateTooltip()
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
        #endregion

        StringBuilder tooltipContent = new StringBuilder();

        //bool isFirstPercentage = true;
        if (Chocolate > 0.01f)
        {
            tooltipContent.AppendLine(Statics.Chocolate + ": " + (Mathf.Round((Chocolate * Fullness * 100))).ToString() + "%");
        }

        if (Milk > 0.01f)
        {
            tooltipContent.AppendLine(Statics.Milk + ": " + (Mathf.Round((Milk * Fullness * 100))).ToString() + "%");
        }

        if (BreastMilk > 0.01f)
        {
            tooltipContent.AppendLine(Statics.BreastMilk + ": " + (Mathf.Round((BreastMilk * Fullness * 100))).ToString() + "%");
        }

        if (Tea > 0.01f)
        {
            tooltipContent.AppendLine(Statics.Tea + ": " + (Mathf.Round((Tea * Fullness * 100))).ToString() + "%");
        }

        if (Cream > 0.01f)
        {
            tooltipContent.AppendLine(Statics.Cream + ": " + (Mathf.Round( (Cream * Fullness * 100))).ToString() + "%");
        }

        if (Espresso > 0.01f)
        {
            tooltipContent.AppendLine(Statics.Espresso + ": " + (Mathf.Round( (Espresso * Fullness * 100))).ToString() + "%");
        }

        if (Sugar > 0.01f)
        {
            tooltipContent.AppendLine(Statics.Sugar + ": " + (Mathf.Round( (Sugar * Fullness * 100))).ToString() + "%");
        }

        if (Coffee > 0.01f)
        {
            tooltipContent.AppendLine(Statics.Coffee + ": " + (Mathf.Round( (Coffee * Fullness * 100))).ToString() + "%");
        }

        if (Boba == true)
        {

            tooltipContent.AppendLine(Statics.Boba);
        }

        if (Ice == true)
        {
            tooltipContent.AppendLine(Statics.Ice);
        }

        if (WhippedCream == true)
        {
            tooltipContent.AppendLine(Statics.WhippedCream);
        }

        if (ChocolateSauce == true)
        {
            tooltipContent.AppendLine(Statics.ChocolateSauce);
        }

        if (CaramelSauce == true)
        {
            tooltipContent.AppendLine(Statics.CaramelSauce);
        }

        if (Sprinkles == true)
        {
            tooltipContent.AppendLine(Statics.Sprinkles);
        }

        CupTooltip.content = tooltipContent.ToString();
    }

    /// <summary>
    /// Will be called per Frame from the Update Monobehaivior
    /// </summary>
    /// <param name="filling"></param>
    public void FillCup(Fillings filling)
    {
        if (Fullness >= 1)
        {
            Fullness = 1;
            return;
        }

        float volume = FillVolume * Time.deltaTime;

        //Debug.Log("Fill " + volume);

        switch (filling)
        {
            case Fillings.Espresso:
                volume_Espresso = volume_Espresso + volume;
                break;
            case Fillings.Coffee:
                volume_Coffee = volume_Coffee + volume;
                break;
            case Fillings.Chocolate:
                volume_Chocolate = volume_Chocolate + volume;
                break;
            case Fillings.Tea:
                volume_Tea = volume_Tea + volume;
                break;
            case Fillings.Milk:
                volume_Milk = volume_Milk + volume;
                break;
            case Fillings.BreastMilk:
                volume = volume * BreatsMilkCupFillMultipler;
                volume_BreastMilk = volume_BreastMilk + volume;
                break;
            case Fillings.Cream:
                volume_Cream = volume_Cream + volume;
                break;
            case Fillings.Sugar:
                volume_Sugar = volume_Sugar + volume;
                break;
            default:
                break;
        }

        Fullness = Fullness + volume;

        float volumeAll = volume_Espresso + volume_Coffee + volume_Chocolate + volume_Tea + volume_Milk + volume_BreastMilk + volume_Cream + volume_Sugar;
        Espresso = volume_Espresso / volumeAll;
        Coffee = volume_Coffee / volumeAll;
        Chocolate = volume_Chocolate / volumeAll;
        Tea = volume_Tea / volumeAll;
        Milk = volume_Milk / volumeAll;
        BreastMilk = volume_BreastMilk / volumeAll;
        Cream = volume_Cream / volumeAll;
        Sugar = volume_Sugar / volumeAll;

        //CheckMaxValues();

        CheckArchievement();
    }

    public void FillCup(Toppings topping)
    {
        switch (topping)
        {
            case Toppings.Ice:
                Ice = true;
                break;
            case Toppings.Boba:
                Boba = true;
                break;
            case Toppings.WhipedCream:
                WhippedCream = true;
                break;
            case Toppings.CaramelSauce:
                CaramelSauce = true;
                break;
            case Toppings.ChocolateSauce:
                ChocolateSauce = true;
                break;
            case Toppings.Sprinkles:
                Sprinkles = true;
                break;
            default:
                break;
        }


        CheckArchievement();
    }

    public int CountUsedFillings()
    {
        int valuesToCalc = 0;
        if (Espresso != 0)
        {
            valuesToCalc++;
        }
        if (Coffee != 0)
        {
            valuesToCalc++;
        }
        if (Chocolate != 0)
        {
            valuesToCalc++;
        }
        if (Tea != 0)
        {
            valuesToCalc++;
        }
        if (Milk != 0)
        {
            valuesToCalc++;
        }
        if (BreastMilk != 0)
        {
            valuesToCalc++;
        }
        if (Cream != 0)
        {
            valuesToCalc++;
        }
        if (Sugar != 0)
        {
            valuesToCalc++;
        }

        return valuesToCalc;
    }

    public int CountUsedToppings()
    {
        int retVal = 0;
        if (Ice)
        {
            retVal++;
        }
        if (Boba)
        {
            retVal++;
        }
        if (WhippedCream)
        {
            retVal++;
        }
        if (CaramelSauce)
        {
            retVal++;
        }
        if (ChocolateSauce)
        {
            retVal++;
        }
        if (Sprinkles)
        {
            retVal++;
        }

        return retVal;
    }

    [BurstCompile]
    private void CheckMaxValues()
    {
        Fullness = math.clamp(Fullness,0,1);
        Espresso = math.clamp(Espresso, 0, 1);
        Coffee = math.clamp(Coffee, 0, 1);
        Chocolate = math.clamp(Chocolate, 0, 1);
        Tea = math.clamp(Tea, 0, 1);
        Milk = math.clamp(Milk, 0, 1);
        BreastMilk = math.clamp(BreastMilk, 0, 1);
        Cream = math.clamp(Cream, 0, 1);
        Sugar = math.clamp(Sugar, 0, 1);
    }

    [BurstCompile]
    public void ResetCup()
    {
        Fullness = 0;
        Espresso = 0;
        Coffee = 0;
        Chocolate = 0;
        Tea = 0;
        Milk = 0;
        BreastMilk = 0;
        Cream = 0;
        Sugar = 0;

        Ice = false;
        Boba = false;
        WhippedCream = false;
        CaramelSauce = false;
        ChocolateSauce = false;
        Sprinkles = false;

        volume_Espresso = 0;
        volume_Coffee = 0;
        volume_Chocolate = 0;
        volume_Tea = 0;
        volume_Milk = 0;
        volume_BreastMilk = 0;
        volume_Cream = 0;
        volume_Sugar = 0;
    }

    public void SetMilkPreset(int preset = 0)
    {
        switch (preset)
        {
            default:
            case 0:
                //Basic
                ChangeMilkWithColor(0);
                ChangeMilk(0);
                break;
            case 1:
                //Thick
                ChangeMilkWithColor(1);
                break;
            case 2:
                //Creamy
                ChangeMilkWithColor(2);
                break;
            case 3:
                //Chocolate
                ChangeMilkWithColor(3);
                break;
            case 4:
                //Strawberry
                ChangeMilkWithColor(0);
                ChangeMilk(3);
                break;
            case 5:
                //Honey
                ChangeMilkWithColor(0);
                ChangeMilk(2);
                break;
            case 6:
                //Blue
                ChangeMilkWithColor(4);
                break;
            case 7:
                //Green
                ChangeMilkWithColor(5);
                break;
            case 8:
                //Raspberry
                ChangeMilkWithColor(7);
                break;
            case 9:
                //Rainbow
                ChangeMilkWithColor(0);
                ChangeMilk(4);
                break;
            case 10:
                //Space
                ChangeMilkWithColor(0);
                ChangeMilk(1);
                break;
            case 11:
                //Void
                ChangeMilkWithColor(6);
                break;
        }
    }

    [BurstCompile]
    public void ChangeMilk(MilkTypes type)
    {
        ChangeMilk(type);
    }

    [BurstCompile]
    public void ChangeMilk(int type)
    {
        MilkType = (MilkTypes)type;
        MilkColor = Color.white;
        //Debug.Log("Cup: " + type);
        PlayerPrefs.SetInt(Consts.PlayerPrefMilkCup, type);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMilkColorR, MilkColor.r);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMilkColorG, MilkColor.g);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMilkColorB, MilkColor.b);
    }

    [BurstCompile]
    public void ChangeMilkWithColor(int MilkColorCnt)
    {
        int type = 0;
        MilkType = (MilkTypes)type;

        switch (MilkColorCnt)
        {
            case 1:
                MilkColor = Statics.MilkColor_Thick;
                break;
            case 2:
                MilkColor = Statics.MilkColor_Creamy;
                break;
            case 3:
                MilkColor = Statics.MilkColor_Chocolate;
                break;
            case 4:
                MilkColor = Statics.MilkColor_Blue;
                break;
            case 5:
                MilkColor = Statics.MilkColor_Green;
                break;
            case 6:
                MilkColor = Statics.MilkColor_Void;
                break;
            case 7:
                MilkColor = Statics.MilkColor_Raspberry;
                break;
            default:
                MilkColor = Color.white;
                break;
        }

        Debug.Log("Cup: " + type);
        PlayerPrefs.SetInt(Consts.PlayerPrefMilkCup, type);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMilkColorR, MilkColor.r);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMilkColorG, MilkColor.g);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMilkColorB, MilkColor.b);
    }

    public void DoFadeAway()
    {
        CupAnimator.SetTrigger(Statics.CupFadeAwayTrigger);
    }

    public void CheckArchievement()
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
        #endregion

        if (Chocolate > 0 && Milk > 0 && Tea > 0 && Cream > 0 && Espresso > 0 && Sugar > 0 && Coffee > 0 &&
            Boba == true && Ice && true && WhippedCream == true && ChocolateSauce == true && CaramelSauce == true && Sprinkles == true)
        {
            Archievements.UnlockArchievement(Archievements.ArchievementID.One_OfEverything);
        }

    }

}
public enum Fillings
{
    Espresso,
    Coffee,
    Chocolate,
    Tea,
    Milk,
    BreastMilk,
    Cream,
    Sugar
}

public enum Toppings
{
    Ice,
    Boba,
    WhipedCream,
    CaramelSauce,
    ChocolateSauce,
    Sprinkles
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
