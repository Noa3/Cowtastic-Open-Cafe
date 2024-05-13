using UnityEngine;

public class PermanentUnlock : MonoBehaviour
{

    [Header("References")]
    public GameObject LockedItem;
    public GameObject UnlockedItem;

    [Header("Settings")]
    [Tooltip("This is Used to save the values")]
    public string UnlockId = "";
    public bool CanBePurchased = true;
    public float UnlockCosts = 50;
    [Tooltip("How many times you need to beat the game to unlock this")]
    public int UnlockPlaythroughs = 0;

    [Header("Debug/Etc")]
    public bool Unlocked = false;

    private BaseGameMode gameMode;

    public const string UnlockedPrefString = "Unlocked_";

    private void Awake()
    {
        Unlocked = PlayerPrefs.GetInt(UnlockedPrefString + UnlockId, 0) == 1 ? true : false ;
        Unlock(Unlocked);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMode = BaseGameMode.instance;
    }

    public void BuyUnlock()
    {
        if(CanBePurchased == true)
        {
            if (gameMode.Money >= UnlockCosts)
            {
                gameMode.Money = gameMode.Money - UnlockCosts;

                Unlock();
            }
        }
    }

    public void Unlock(bool value = true)
    {
        Unlocked = value;
        PlayerPrefs.SetInt(UnlockedPrefString + UnlockId, (value == true) ? 1 : 0);
        LockedItem.SetActive(!value);
        UnlockedItem.SetActive(value);

        CheckArchivements();
    }

    const string TopOnly = "TopOnly";
    const string Bikini = "Bikini";
    const string NoneApron = "NoneApron";
    const string PoofyPantsPants = "PoofyPantsPants";
    const string UnderWearPants = "UnderWearPants";
    const string NonePants = "NonePants";
    public static void CheckArchivements()
    {
        //Wardrobe Archievement
        bool TypeIDTopOnly = false; //TopOnly
        bool TypeIDBikini = false; //Bikini
        bool TypeIDNoneApron = false; //NoneApron

        bool TypeIDPoofyPantsPants = false; //PoofyPantsPants
        bool TypeIDUnderWearPants = false; //UnderWearPants
        bool TypeIDNonePants = false; //NonePants

        PermanentUnlock[] unlocks = FindObjectsOfType<PermanentUnlock>();
        for (int i = 0; i < unlocks.Length; i++)
        {
            string typeID = unlocks[i].UnlockId;
            if (typeID.Equals(TopOnly))
            {
                TypeIDTopOnly = unlocks[i].Unlocked;
            }
            else if (typeID.Equals(Bikini))
            {
                TypeIDBikini = unlocks[i].Unlocked;
            }
            else if (typeID.Equals(NoneApron))
            {
                TypeIDNoneApron = unlocks[i].Unlocked;
            }
            else if (typeID.Equals(PoofyPantsPants))
            {
                TypeIDPoofyPantsPants = unlocks[i].Unlocked;
            }
            else if (typeID.Equals(UnderWearPants))
            {
                TypeIDUnderWearPants = unlocks[i].Unlocked;
            }
            else if (typeID.Equals(NonePants))
            {
                TypeIDNonePants = unlocks[i].Unlocked;
            }
        }

        if (TypeIDTopOnly == true && TypeIDBikini == true && TypeIDNoneApron == true && TypeIDPoofyPantsPants == true && TypeIDUnderWearPants == true && TypeIDNonePants == true)
        {
            Archievements.UnlockArchievement(Archievements.ArchievementID.Full_Wardrobe);
        }


    }

}
