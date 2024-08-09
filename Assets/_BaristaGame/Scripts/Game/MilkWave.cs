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
    public int MilkType = 0;
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
        WavesMaterial.SetFloat(WavesMaterialMilkType, MilkType);

        WavesMaterial.SetColor(WavesMaterialCustomMilkColor, MilkColor);
        WavesMaterial.SetFloat(WavesMaterialHeight, Height);
    }

    public void SetMilkTypeAndColor(int milkType, Color milkColor)
    {
        MilkColor = milkColor;
        MilkType = milkType;
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        Height = 0;
        UpdateMaterial();
    }
#endif
}
