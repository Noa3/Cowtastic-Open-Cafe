using UnityEngine;

/// <summary>
/// Take a look to the Milk Change Buttons in the Customize Menu for the values
/// </summary>

public class Event_ChangeMilkColor : EventBase
{
    [Header("Event Settings")]

    [Min(0)]
    public int milkPreset = 0;

    //[Min(0)][Tooltip("Take a look at the Milk Change Buttons for value ChangeMilkWithColor")]
    //public int MilkWithColor = 0;
    //[Min(0)][Tooltip("Take a look at the Milk Change Buttons for value ChangeMilk")]
    //public int MilkColor = 0;

    public bool AfterEventRestoreOrginal = true;

    CupController CupController;
    BaristaController BaristaController;
    MilkWave milkWave;

    private Color OrginalMilkColor = Color.white;
    private MilkTypes OrginalMilkType = MilkTypes.Milk;

    private void Awake()
    {
        CupController = CupController.instance;
        BaristaController = BaristaController.instance;
        milkWave = MilkWave.instance;
    }

    // Start is called before the first frame update
    //new void Start()
    //{
    //    base.Start();
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public override void SetEventState(bool state)
    {
        if (state == true)
        {
            OrginalMilkColor = CupController.MilkColor;
            OrginalMilkType = CupController.MilkType;

            if (CupController != null)
            {
                CupController.SetMilkPreset(milkPreset);
            }
            if (BaristaController != null)
            {
                BaristaController.SetMilkPreset(milkPreset);
            }
            if (milkWave != null)
            {
                milkWave.SetMilkPreset(milkPreset);
            }
        }
        else
        {
            if (AfterEventRestoreOrginal == true)
            {
                RestoreOrginalMilk();
            }
        }
    }

    public void OnDestroy()
    {
        if (AfterEventRestoreOrginal == true)
        {
            RestoreOrginalMilk();
        }
    }

    public void OnApplicationQuit()
    {
        if (AfterEventRestoreOrginal == true)
        {
            RestoreOrginalMilk();
        }
    }

    public void RestoreOrginalMilk()
    {
        //CupController.MilkColor = OrginalMilkColor;
        //CupController.ChangeMilk(OrginalMilkType);
        //BaristaController.MilkColor = OrginalMilkColor;
        //BaristaController.ChangeMilk(OrginalMilkType);
    }
}
