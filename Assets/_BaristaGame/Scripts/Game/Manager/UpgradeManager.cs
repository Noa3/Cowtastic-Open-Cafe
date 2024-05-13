using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public GameObject InitialUpgradePanel;
    public GameObject UpgradePanel;

    public bool UpgardePanelVisibility = false;

    [Header("Upgrades")]
    [ReadOnly]
    public bool HasInitialUpgrade = false;

    [Space(5)]
    //public float UpgradeProductionCost = 10;


    //public List<Upgrade> upgrades;
    [Header("Debug Only")]
#if UNITY_EDITOR
    [ReadOnly]
    public float AvailableMoney = 0;
#endif
    public static UpgradeManager Instance;

    private BaseGameMode gamemode;



    public void Awake()
    {
        Instance = this;

        //Not working
        //for (int i = 0; i < upgrades.Count; i++)
        //{
        //    upgrades[i].button.onClick.AddListener(delegate { BuyUpgrade(upgrades[i].button); });
        //}

        UpgradePanel.SetActive(UpgardePanelVisibility);
    }

    // Start is called before the first frame update
    void Start()
    {
        gamemode = BaseGameMode.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (UpgardePanelVisibility == true)
        {
#if UNITY_EDITOR
            AvailableMoney = gamemode.Money;
#endif
            UpdateButtonsVisuals();
        }
    }

    public void TogglePanelActive()
    {
        //Debug.Log("Toggle Upgrade Visibility");
        UpgardePanelVisibility = !UpgardePanelVisibility;
        if (HasInitialUpgrade == true)
        {
            UpgradePanel.SetActive(UpgardePanelVisibility);
        }
        else
        {
            InitialUpgradePanel.SetActive(UpgardePanelVisibility);
        }

    }

    public void SetPanelActive(bool state)
    {
        UpgardePanelVisibility = state;
        if (HasInitialUpgrade == true)
        {
            UpgradePanel.SetActive(state);
        }
        else
        {
            InitialUpgradePanel.SetActive(state);
        }
    }

    public void BuyUpgrade(Button button)
    {
        //for (int i = 0; i < upgrades.Count; i++)
        //{
        //    if (upgrades[i].button == button)
        //    {
        //        BaseGameMode.instance.BuyUpgrade(
        //            Mathf.RoundToInt(upgrades[i].Costs * (upgrades[i].CostsModifer * (upgrades[i].UpgradedTimes + 1))),
        //            upgrades[i].Happyness,
        //            upgrades[i].MaxBust,
        //            upgrades[i].Production
        //            );

        //        upgrades[i].UpgradedTimes++;

        //        break;
        //    }
        //}
        UpdateButtonsVisuals();
    }


    /// <summary>
    /// This will be the first Upgrade which start to let her grow
    /// </summary>
    public void BuyInitialUpgarde()
    {
        Debug.Log("Buy Upgrade: Initial");
            if (gamemode == null)
            {
                gamemode = BaseGameMode.instance;
            }
            gamemode.UpgradeCanGrow = true;
            TogglePanelActive();
            HasInitialUpgrade = true;
            InitialUpgradePanel.SetActive(false);
            UpdateButtonsVisuals();
    }

    void UpdateButtonsVisuals()
    {
        //for (int i = 0; i < upgrades.Count; i++)
        //{
        //    int upgradeCosts = Mathf.RoundToInt(upgrades[i].Costs * (upgrades[i].CostsModifer * (upgrades[i].UpgradedTimes +1 )));
        //    upgrades[i].PriceText.text = upgradeCosts + " " + Statics.CurrencySymbol;

        //    if (AvailableMoney < upgradeCosts)
        //    {
        //        upgrades[i].button.interactable = false;
        //    }
        //}
    } 
}

//[System.Serializable]
//public class Upgrade
//{
//    public Button button;
//    public TextMeshProUGUI PriceText;
//    public int Costs = 10;
//    public float CostsModifer = 2;
//    public int UpgradedTimes = 1;
//    //public int MaxUpgrades = 10;

//    public int Happyness = 0;
//    public int MaxBust = 0;
//    public int Production = 0;
//}
