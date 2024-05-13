using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class StatsManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI ProductionRateText;
    public TextMeshProUGUI MaxSizeText;
    public TextMeshProUGUI FundsText;
    //Founds Animation
    public Animator FoundsAnimatior;
    public GameObject GOFoundsAdd;
    public ParticleSystem ParticleFoundsAdd;
    public GameObject GOFoundsSub;
    public ParticleSystem ParticleFoundsSub;

    public ParticleSystem ParticleHappynessAdd;
    public ParticleSystem ParticleHappynessSub;
    public ParticleSystem ParticleMaxSizeAdd;
    public ParticleSystem ParticleProductionAdd;

    public TextMeshProUGUI MilkFillText;
    public Image MilkFillImage;
    public TextMeshProUGUI HappinessText;
    public Image HappinesSprite; //Watch out this need to be Image Type Filled, or else overwork the whole mechanic

    public TextMeshProUGUI LevelText;
    public Slider ExpSlider;
    public GameObject CantLevelUp;

    [Header("Setting")]
    [Min(0)]
    public int MaxSizeAdditionalSize = 100;

    [ReadOnly]
    public bool CanGrow = false;
    [ReadOnly]
    public double ProductionRate = 150;
    [ReadOnly]
    public float MaxSize = 200;
    [Tooltip("This will be used to multiple the end value of the size to be shown on the board")]
    public float MaxSizeMultipler = 20;
    [ReadOnly]
    public float Funds = 00;

    [ReadOnly]
    public float MilkFill = 100;
    public float CurrentBust = 10;
    [Tooltip("This will be used to multiple the end value of the size to be shown on the board")]
    public float CurrentBustMultipler = 20;
    [Range(0,100)]
    [ReadOnly]
    public float Happiness = 100;

    [Min(0)]
    public float ParticleHappynessTickTime = 0.5f;

    [ReadOnly]
    [Range(0, 1f)]
    public float ExpPercent = 0;
    [ReadOnly]
    public int Level = 1;
    [ReadOnly]
    public bool CanLevelUp = true;

    public static StatsManager instance;

    private TextMeshProUGUI GOFoundsAddText;
    private TextMeshProUGUI GOFoundsSubText;

    private float ParticleHappynessLastTimePlayed = 0;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GOFoundsAddText = GOFoundsAdd.GetComponent<TextMeshProUGUI>();
        GOFoundsSubText = GOFoundsSub.GetComponent<TextMeshProUGUI>();
        GOFoundsAdd.SetActive(false);
        GOFoundsSub.SetActive(false);
    }

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        UpdateGui();
    }

    public void UpdateGui()
    {
        if (CanGrow == true)
        {
            //ProductionRateText.text = String.Format("{0:0.00}", ProductionRate);
            ProductionRateText.text = String.Format("{0:0}", ProductionRate);
            MaxSizeText.text = (Mathf.RoundToInt(MaxSize * MaxSizeMultipler) + MaxSizeAdditionalSize).ToString();
        }
        else
        {
            ProductionRateText.text = "0";
            MaxSizeText.text = MaxSizeAdditionalSize.ToString();
        }

        FundsText.text = (Mathf.Round(Funds * 100)/100).ToString("0.00");

        MilkFillText.text = Mathf.RoundToInt((CurrentBust * 100) / 100 * CurrentBustMultipler).ToString();


        MilkFillImage.fillAmount = Mathf.Lerp(MilkFillImage.fillAmount, MilkFill / 100f, Time.deltaTime * 3 );

        HappinessText.text = Mathf.RoundToInt(Happiness).ToString();

        HappinesSprite.fillAmount = Mathf.Lerp(HappinesSprite.fillAmount, Happiness / 100, Time.deltaTime * 3);

        ExpSlider.value = ExpPercent;
        LevelText.text =  Statics.LevelPreText + Level;
        CantLevelUp.SetActive(!CanLevelUp);
    }

    public void AddProductionRate()
    {
        ParticleProductionAdd.gameObject.SetActive(true);
        ParticleProductionAdd.Play();
    }

    public void AddMaxSize()
    {
        ParticleMaxSizeAdd.gameObject.SetActive(true);
        ParticleMaxSizeAdd.Play();
    }

    public void AddHappyness()
    {
        if (Time.timeSinceLevelLoad > (ParticleHappynessLastTimePlayed + ParticleHappynessTickTime))
        {
            ParticleHappynessLastTimePlayed = Time.timeSinceLevelLoad;
            ParticleHappynessAdd.gameObject.SetActive(true);
            ParticleHappynessAdd.Play();
        }
    }

    public void SubHappyness()
    {
        ParticleHappynessSub.gameObject.SetActive(true);
        ParticleHappynessSub.Play();
    }

    public void AddMoney(float value)
    {
        GOFoundsAddText.text = (Mathf.Round(value * 100) / 100).ToString("0.00");
        GOFoundsAdd.SetActive(true);
        StartCoroutine(PlayMoneyAnimation(Statics.AnimationAddMoneyTrigger, ParticleFoundsAdd, 1));
    }

    public void SubMoney(float value)
    {
        GOFoundsSubText.text = (Mathf.Round(value * 100) / 100).ToString("0.00");
        GOFoundsSub.SetActive(true);
        StartCoroutine(PlayMoneyAnimation(Statics.AnimationSubMoneyTrigger, ParticleFoundsSub,0f));
    }

    IEnumerator PlayMoneyAnimation(string TriggerName, float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);
        FoundsAnimatior.SetTrigger(TriggerName);
    }

    IEnumerator PlayMoneyAnimation(string TriggerName,ParticleSystem particle, float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);
        FoundsAnimatior.SetTrigger(TriggerName);
        if (particle != null)
        {
            particle.gameObject.SetActive(true);
            particle.Play();
        }
    }
}
