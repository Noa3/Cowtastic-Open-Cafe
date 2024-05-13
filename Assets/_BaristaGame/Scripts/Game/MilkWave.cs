using Unity.Burst;
using UnityEngine;

public class MilkWave : MonoBehaviour
{
    [Header("References")]
    public Material WavesMaterial;

    [Header("Settings")]
    [Range(0,100)]
    public float fullnessMin = 90;
    [Tooltip("How long the actual level will stay until it goes down again")]
    public float ResetTimeForDecrease = 3;
    public float DecreaseSpeed  = 0.025f;
    public float HeightIncreaseDivider = 10;
    [Range(0, 1f)]
    public float GameWonMaxHeight = 0.3f;

    [Header("Settings Material")]
    public MilkTypes MilkType = 0;
    public Color MilkColor = Color.white;
    [Range(0, 1)]
    public float Height = 0;

    [Header("Debug/Info")]
    [ReadOnly]
    public bool GameWon = false;
    [ReadOnly]
    public bool CurrentlyRising = false;
    [ReadOnly]
    public float LastTimeWasRising = 0;


    public static MilkWave instance;

    private BaseGameMode gameMode;


    //Material setting name
    private const string WavesMaterialMilkType = "_Milk_Type";
    private const string WavesMaterialTiling = "_Tiling_Speed";
    private const string WavesMaterialCustomMilkColor = "_Custom_Milk_Color";
    private const string WavesMaterialHeight = "_Height";

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMode = BaseGameMode.instance;
    }

    // Update is called once per frame
    void Update()
    {
        //Is Bust Too big and game Not Won?
        CurrentlyRising = (gameMode.Fullness > fullnessMin);


        if (CurrentlyRising == true)
        {
            LastTimeWasRising = Time.timeSinceLevelLoad;


            if (GameWon == true)
            {
                if (Height != GameWonMaxHeight)
                {
                    CalcNewWaveHeight();

                    if (Height > GameWonMaxHeight)
                    {
                        Height = GameWonMaxHeight;
                    }
                }
            }
            else
            {
                if (Height != 1)
                {
                    CalcNewWaveHeight();

                    if (Height > 1)
                    {
                        Height = 1;
                    }
                }
            }

        }
        else
        {
            if (Time.timeSinceLevelLoad > (LastTimeWasRising + ResetTimeForDecrease) )
            {
                //Decrease the Height
                if (Height != 0)
                {
                    Height = Height - (DecreaseSpeed * Time.deltaTime);
                    if (Height < 0)
                    {
                        Height = 0;
                    }
                }
            }

        }


        UpdateMaterial();
    }

    private void CalcNewWaveHeight()
    {
        Height = Height + (Statics.DoubleToFloat(gameMode.ProductionRate / HeightIncreaseDivider) * Time.deltaTime);
    }

    public void UpdateMaterial()
    {
        WavesMaterial.SetFloat(WavesMaterialMilkType, (int)MilkType);

        WavesMaterial.SetColor(WavesMaterialCustomMilkColor, MilkColor);
        WavesMaterial.SetFloat(WavesMaterialHeight, Height);
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

        Debug.Log("MilkWave: " + type);
        PlayerPrefs.SetInt(Consts.PlayerPrefMilkCup, type);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMilkColorR, MilkColor.r);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMilkColorG, MilkColor.g);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMilkColorB, MilkColor.b);
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        Height = 0;
        UpdateMaterial();
    }
#endif
}
