using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Burst;

/// <summary>
/// This is the Animation Controller for the Barista
/// </summary>

public class BaristaController : MonoBehaviour
{
    [Header("References")]
    public Animator BaristaAnimator;

    public GameObject BaristaApron;
    public Mesh ApronMesh;
    public Mesh BikiniMesh;

    public GameObject BaristaPants;
    public GameObject BaristaUnderwear;
    public GameObject BaristaPoofyPants;

    private Renderer BaristaApronRenderer;
    private List<Material> MaterialBaristaApron = new List<Material>();
    private List<Material> MaterialBaristaUnderwear = new List<Material>();

    public Material MaterialBaristaApronTop;
    public Material MaterialBaristaApronBottom;
    public Material MaterialBaristaApronUpper;
    public Material TransparentMaterial;


    public ParticleSystemRenderer[] MilkParticle;

    public SoundEffectVariation SoundVariationClothes;

    [Header("References Optional")]
    public TMP_Dropdown MilkSelector;

    [Header("Settings")]
    public MilkTypes MilkType = 0;
    public Color MilkColor = Color.white;

    public ClothColors[] BaristaColors;

    public bool ChangeRandomSeed = false;
    public bool DoAutomaticMousePos = true;

    public bool BeeingMilkedHidesClothes = true;

    [Range(0,1f)]
    public float BustSize = 0;
    [Range(0, 1f)]
    public float Fullness = 0;
    [Range(0, 1f)]
    public float Happiness = 0;
    public bool BeeingMilked = false;
    public bool Clothed = true;
    //public bool PantsOn = true;
    public bool Talking = false;
    public bool HeadPatting = false;
    public bool AutoFixOutfit = false;


    [Range(0, 1f)]
    public float MouseX = 0;
    [Range(0, 1f)]
    public float MouseY = 0;

    [Range(0, 1f)]
    public float MinToActivateFixSize = 0.175f;

    [ReadOnly]
    public bool CanFixApron = false;
    [ReadOnly]
    public bool wasOverFixApron = false;

    public static BaristaController instance;

    float currentScreenSizeX;
    float currentScreenSizeY;

    [ReadOnly]
    private bool WasClothed = true;
    [ReadOnly]
    public bool CosmeticClothed = true;

    private int currentSelectdColor = 0;
    [HideInInspector]
    public int currentBaristaTop = 0;
    //private int currentBaristaUnderwear = 0;
    private SkinnedMeshRenderer ApronSkinnedMeshRenderer;


    [Header("Not Fully Implemennted/Working On")]
    public AudioSource BaristaAnimationSoundSource;

    //Trigger
    public bool SupriseGrowth {
        set
        {
            if (value == true)
            {
                DoSupriseGrowth();
            }
            return;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;

            WasClothed = Clothed;
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        if (BaristaAnimator == null)
        {
            BaristaAnimator = GetComponent<Animator>();
        }

        if (MilkSelector != null)
        {
            MilkColor = new Color(PlayerPrefs.GetFloat(Consts.PlayerPrefMilkColorR, MilkColor.r), PlayerPrefs.GetFloat(Consts.PlayerPrefMilkColorG, MilkColor.g), PlayerPrefs.GetFloat(Consts.PlayerPrefMilkColorB, MilkColor.b));

            //Debug.Log("barista: " + PlayerPrefs.GetInt(Statics.PlayerPrefMilkBarista, 0));
            MilkSelector.value = PlayerPrefs.GetInt(Consts.PlayerPrefMilkBarista, 0);
            ChangeMilk(MilkSelector.value);
        }

        ApronSkinnedMeshRenderer = BaristaApron.GetComponent<SkinnedMeshRenderer>();

        BaristaApronRenderer = BaristaApron.GetComponent<Renderer>();
        MaterialBaristaApron.AddRange(BaristaApronRenderer.materials);
        MaterialBaristaUnderwear.Add(BaristaPants.GetComponent<Renderer>().material);
        MaterialBaristaUnderwear.Add(BaristaUnderwear.GetComponent<Renderer>().material);

        SetBaristaColor(0);
    }

    // Update is called once per frame
    void Update()
    {
        MyUpdate();
    }

    [BurstCompile]
    public virtual void MyUpdate()
    {
        if (Time.timeScale > 0)
        {

            if (BustSize > MinToActivateFixSize)
            {
                wasOverFixApron = true;
            }

            if (wasOverFixApron == true && BustSize < MinToActivateFixSize && BaristaApron.activeSelf == true)
            {
                CanFixApron = true;
            }
            else
            {
                CanFixApron = false;
            }

            BaristaAnimator.SetFloat(Consts.BustSize, BustSize);
            BaristaAnimator.SetFloat(Consts.BaristaFullness, Fullness);
            BaristaAnimator.SetFloat(Consts.Happiness, Happiness);

            BaristaAnimator.SetBool(Consts.BeingMilked, BeeingMilked);

            //if (BeeingMilkedHidesClothes == true && BeeingMilked == true)
            //{
            //    BaristaAnimator.SetBool(Statics.Clothed, false);
            //    if (WasClothed != false)
            //    {
            //        WasClothed = false;
            //        SoundVariationClothes.PlayRandomOneShot();
            //    }
            //}
            //else
            //{
            //    BaristaAnimator.SetBool(Statics.Clothed, Clothed);
            //    if (WasClothed != Clothed)
            //    {
            //        WasClothed = Clothed;
            //        SoundVariationClothes.PlayRandomOneShot();
            //    }
            //}

            if (currentBaristaTop >= 0 && Clothed == true)
            {
                BaristaAnimator.SetBool(Consts.Clothed, true);
            }
            else
            {
                BaristaAnimator.SetBool(Consts.Clothed, false);
            }



            BaristaAnimator.SetBool(Consts.Talking, Talking);
            BaristaAnimator.SetBool(Consts.BaristaHeadPat, HeadPatting);
            BaristaAnimator.SetBool(Consts.BaristaAutoFixOutfit, (AutoFixOutfit && BaristaApron.activeSelf == true));

            BaristaAnimator.SetFloat(Consts.BaristaMouseX, MouseX);
            BaristaAnimator.SetFloat(Consts.BaristaMouseY, MouseY);

            //BaristaPants.SetActive(PantsOn);
        }
    }

    /// <summary>
    /// Important for the eye tracking of the mouse
    /// </summary>
    private void LateUpdate()
    {
        if (DoAutomaticMousePos)
        {
            currentScreenSizeX = Screen.width;
            currentScreenSizeY = Screen.height;

            Vector3 mousePos = Input.mousePosition;
            Vector2 newPos = new Vector3(
                (mousePos.x / currentScreenSizeX),
                (mousePos.y / currentScreenSizeY));

            MouseX = newPos.x;
            MouseY = newPos.y;
        }

    }

    public virtual void DoSupriseGrowth()
    {
        BaristaAnimator.SetTrigger(Consts.SurpriseGrowth);
    }

    public virtual void DoMiniSupriseGrowth()
    {
        BaristaAnimator.SetTrigger(Consts.MiniSurpriseGrowth);
    }
    public virtual void DoFixOutfit()
    {
        wasOverFixApron = false;
        CanFixApron = false;

        BaristaAnimator.SetTrigger(Consts.BaristaFixApron);
    }

    public virtual void DoReset()
    {
        BaristaAnimator.SetTrigger(Consts.BaristaReset);
    }

    public virtual void DoGoodEnd()
    {
        BaristaAnimator.SetTrigger(Consts.BaristaGoodEnd);
    }

    public virtual void DoBadEnd()
    {
        BaristaAnimator.SetTrigger(Consts.BaristaBadEnd);
    }

    public void SetAutoFixOutfit(bool state)
    {
        AutoFixOutfit = state;
    }

    public virtual void FixedUpdate()
    {
        if (ChangeRandomSeed == true)
        {
            BaristaAnimator.SetFloat(Consts.Random, Statics.GetRandomRange(0f, 1f));
        }

        CheckArchievemnt();
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

    //public void ChangeMilk(MilkTypes type)
    //{
    //    ChangeMilk(type);
    //}
    public void ChangeMilk(MilkTypes type)
    {
        ChangeMilk(type);
    }
    public void ChangeMilk(int type)
    {
        MilkType = (MilkTypes)type;
        SetMilkParticle(type);
    }

    public void ChangeMilkWithColor(int MilkColorCnt)
    {
        int type = 0;
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

        MilkType = (MilkTypes)0;
        SetMilkParticle(type);
    }

    private void SetMilkParticle(int type = 0)
    {
        for (int i = 0; i < MilkParticle.Length; i++)
        {
            MilkParticle[i].material.SetColor(Consts.CupShader_MilkTypeColor, MilkColor);
            MilkParticle[i].material.SetInt(Consts.CupShader_MilkType, type);

            if (MilkParticle[i].trailMaterial != null)
            {
                MilkParticle[i].trailMaterial.SetColor(Consts.CupShader_MilkTypeColor, MilkColor);
                MilkParticle[i].trailMaterial.SetInt(Consts.CupShader_MilkType, type);
            }

            //ParticleSystem ps = MilkParticle[i].GetComponent<ParticleSystem>();
            //ParticleSystem.TrailModule trailModule = ps.trails;
            //trailModule.colorOverLifetime = MilkColor;
            //trailModule.colorOverTrail = MilkColor;
        }

        PlayerPrefs.SetInt(Consts.PlayerPrefMilkBarista, type);
        Debug.Log("Barista: " + type);
    }

    public void SetUnderwearType(int type = 0) // -1 is nude
    {
        BaristaPants.SetActive(false);
        BaristaUnderwear.SetActive(false);
        BaristaPoofyPants.SetActive(false);

        //currentBaristaUnderwear = type;

        if (type == 0) // Pants
        {
            BaristaPants.SetActive(true);
        }
        else if (type == 1) //Underwear
        {
            BaristaUnderwear.SetActive(true);
        }
        else if (type == 2) //PuffyPants
        {
            BaristaPoofyPants.SetActive(true);
        }

    }

    public void SetBaristaTop(int type = 0) //-1 is nude
    {
        currentBaristaTop = type;

        if (type >= 0)
        {
            BaristaApron.SetActive(true);

            if (type == 0) //Orginal Apron
            {
                ApronSkinnedMeshRenderer.sharedMesh = ApronMesh;
                BaristaApronRenderer.materials = new Material[] { MaterialBaristaApronBottom, MaterialBaristaApronUpper, MaterialBaristaApronTop };
            }
            if (type == 1) //Bottom half removed
            {
                ApronSkinnedMeshRenderer.sharedMesh = ApronMesh;
                BaristaApronRenderer.materials = new Material[] { TransparentMaterial, MaterialBaristaApronUpper, MaterialBaristaApronTop };
            }
            if (type == 2) //Bikini
            {
                ApronSkinnedMeshRenderer.sharedMesh = BikiniMesh;
                BaristaApronRenderer.materials = new Material[] { MaterialBaristaApronBottom, MaterialBaristaApronUpper, MaterialBaristaApronTop };
            }

            MaterialBaristaApron = new List<Material> (BaristaApronRenderer.materials);
            SetBaristaColor(currentSelectdColor);
        }
        else
        {
            BaristaApron.SetActive(false);
        }
    }

    const string _Color = "_Color";
    const string _Apron_Color_Shifts = "_Apron_Color_Shifts";

    public void SetBaristaColor(int value = 0) //0 is default
    {
        currentSelectdColor = value;

        for (int i = 0; i < MaterialBaristaApron.Count; i++)
        {
            MaterialBaristaApron[i].SetVector(_Apron_Color_Shifts, BaristaColors[value].Clothes);
        }

        for (int i = 0; i < MaterialBaristaUnderwear.Count; i++)
        {
            MaterialBaristaUnderwear[i].SetColor(_Color, BaristaColors[value].Underwear);
        }
    }

    public void CheckArchievemnt()
    {
        if (BustSize > 0.99f)
        {
            Archievements.UnlockArchievement(Archievements.ArchievementID.Supersized);
        }
    }
}

[System.Serializable]
public struct ClothColors
{
    public Vector3 Clothes;
    public Color Underwear;
}

//Colors From kerb
//Default Green -
//Apron Color Shifts: 0,0,1
//Pants Color: 1D1E25

//Cyan -
//Apron Color Shifts: 0.06,0.2,2
//Pants Color: 6FD7C4

//Blue -
//Apron Color Shifts: 0.15,0.1,1.75
//Pants Color: 83C8A8

//Dark Blue -
//Apron Color Shifts: 0.15,-0.075,1.2
//Pants Color: 2C493D

//Purple -
//Apron Color Shifts: 0.34,-0.05,1.4
//Pants Color: 5E7D69

//Magenta -
//Apron Color Shifts: 0.4,0.1,1.75
//Pants Color: D7ADC1

//Red -
//Apron Color Shifts: 0.5,0,2
//Pants Color: 8070A6

//Dark Red -
//Apron Color Shifts: 0.5,-0.15,1.5
//Pants Color: 3C3257

//Orange -
//Apron Color Shifts: 0.62,0.1,2
//Pants Color: 3A5E1F

//Yellow -
//Apron Color Shifts: 0.68,0.15,2.5
//Pants Color: FFFCFE

//Pink White -
//Apron Color Shifts: 0.3,0.25,9
//Pants Color: FFDAF2

//Blue White -
//Apron Color Shifts: 0.1,0.6,0.9
//Pants Color: F1F6BD

//Gray -
//Apron Color Shifts: 0.15,0.3,0.3
//Pants Color: 5F6F81

//Dark -
//Apron Color Shifts: 0.2,-0.15,1
//Pants Color: 072F33

//Void -
//Apron Color Shifts: 0,-1,1
//Pants Color: 000000
