using Unity.Burst;
using UnityEngine;
using static Archievements;

/// <summary>
/// A place where all Statistics are Collected?
/// </summary>
/// 
public class StatisticsHolder : MonoBehaviour
{
    [Header("Scene Relevant")]
    [ReadOnly]
    public float MoneyEarned = 0;
    [ReadOnly]
    public double MilkCreated = 0;
    [ReadOnly]
    public int CupsSold = 0;

    [Header("Peristent Stuff")]
    [ReadOnly]
    public double TotalTimePlayed = 0;
    [ReadOnly]
    public int TotalCupsSold = 0;
    [ReadOnly]
    public float TotalMoneyEarned = 0;
    [ReadOnly]
    public double TotalMilkProduced = 0;

    [ReadOnly]
    public bool SawAdventurer = false;
    [ReadOnly]
    public bool SawCat = false;
    [ReadOnly]
    public bool SawFairy = false;
    [ReadOnly]
    public bool SawHero = false;
    [ReadOnly]
    public bool SawKnight = false;

    [Header("Etc")]

    [ReadOnly]
    public BestTimeManager bestTimeManager;

    public static StatisticsHolder instance;

    private StatsManager stats;

    private void Awake()
    {
        instance = this;
        LoadValues();
    }

    // Start is called before the first frame update
    void Start()
    {
        bestTimeManager = BestTimeManager.instance;
        stats = StatsManager.instance;
    }

    //Update is called once per frame
    void Update()
    {
        TotalTimePlayed = TotalTimePlayed + Time.unscaledDeltaTime;
    }

    [BurstCompile]
    public void AddServedCup(int value = 1)
    {
        CupsSold = CupsSold + value;
        TotalCupsSold = TotalCupsSold + value;
    }

    [BurstCompile]
    public void AddEarnedMoney(float value)
    {
        MoneyEarned = MoneyEarned + value;
        TotalMoneyEarned = TotalMoneyEarned + value;
    }

    [BurstCompile]
    public void AddMilk(double value)
    {
        MilkCreated = MilkCreated + (value * stats.CurrentBustMultipler);
        TotalMilkProduced = TotalMilkProduced + (value * stats.CurrentBustMultipler);
    }


    [BurstCompile]
    public void LoadValues()
    {
        TotalTimePlayed = double.Parse(PlayerPrefs.GetString(Consts.PlayerPrefTimePlayedOverall, "0"));
        TotalCupsSold = PlayerPrefs.GetInt(Consts.PlayerPrefCupsSoldOverall, 0);
        TotalMoneyEarned = PlayerPrefs.GetFloat(Consts.PlayerPrefMoneyEarnedOverall, 0);
        TotalMilkProduced = double.Parse(PlayerPrefs.GetString(Consts.PlayerPrefMilkProducedOverall, "0"));

        SawAdventurer = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSawAvatarAdventurer, false.ToString()) );
        SawCat = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSawAvatarCat, false.ToString()));
        SawFairy = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSawAvatarFairy, false.ToString()));
        SawHero = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSawAvatarHero, false.ToString()));
        SawKnight = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefSawAvatarKnight, false.ToString()));
    }

    [BurstCompile]
    public void SaveValues()
    {
        CheckArchievement();
        PlayerPrefs.SetString(Consts.PlayerPrefTimePlayedOverall, TotalTimePlayed.ToString() );
        PlayerPrefs.SetInt(Consts.PlayerPrefCupsSoldOverall, TotalCupsSold);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMoneyEarnedOverall, TotalMoneyEarned);
        PlayerPrefs.SetString(Consts.PlayerPrefMilkProducedOverall, TotalMilkProduced.ToString() );

        PlayerPrefs.SetString(Consts.PlayerPrefSawAvatarAdventurer, SawAdventurer.ToString() );
        PlayerPrefs.SetString(Consts.PlayerPrefSawAvatarCat, SawCat.ToString() );
        PlayerPrefs.SetString(Consts.PlayerPrefSawAvatarFairy, SawFairy.ToString() );
        PlayerPrefs.SetString(Consts.PlayerPrefSawAvatarHero, SawHero.ToString() );
        PlayerPrefs.SetString(Consts.PlayerPrefSawAvatarKnight, SawKnight.ToString() );
    }

    [BurstCompile]
    public void CheckArchievement()
    {
        if (TotalCupsSold > 1000)
        {
            Archievements.UnlockArchievement(ArchievementID.SoldCups1000);
        }

        int minutes = (int)TotalTimePlayed / 60;
        //int seconds = (int)TotalTimePlayed % 60;
        if (minutes > 600)
        {
            Archievements.UnlockArchievement(ArchievementID.Play10h);
        }

        if (TotalMilkProduced > 100000)
        {
            Archievements.UnlockArchievement(ArchievementID.ProduceMilk100);
        }

        if (TotalMoneyEarned > 10000)
        {
            Archievements.UnlockArchievement(ArchievementID.EarnedMoney10000);
        }

        //Familiar Faces
        if (SawAdventurer == true && SawCat == true && SawFairy == true && SawHero == true && SawKnight == true)
        {
            Archievements.UnlockArchievement(Archievements.ArchievementID.Familiar_Faces);
        }
    }

    [BurstCompile]
    public void AddSeenCustomer(string name)
    {
        if (string.Equals(name, Consts.ArchievementAvatarAdventurer, System.StringComparison.OrdinalIgnoreCase))
        {
            SawAdventurer = true;
        }
        else if (string.Equals(name, Consts.ArchievementAvatarCat, System.StringComparison.OrdinalIgnoreCase))
        {
            SawCat = true;
        }
        else if (string.Equals(name, Consts.ArchievementAvatarFairy, System.StringComparison.OrdinalIgnoreCase))
        {
            SawFairy = true;
        }
        else if (string.Equals(name, Consts.ArchievementAvatarHero, System.StringComparison.OrdinalIgnoreCase))
        {
            SawHero = true;
        }
        else if (string.Equals(name, Consts.ArchievementAvatarKnight, System.StringComparison.OrdinalIgnoreCase))
        {
            SawKnight = true;
        }
    }

    public void OnDestroy()
    {
        SaveValues();
    }

    public void OnDisable()
    {
        SaveValues();
    }

    public void OnApplicationQuit()
    {
        SaveValues();
    }
}
