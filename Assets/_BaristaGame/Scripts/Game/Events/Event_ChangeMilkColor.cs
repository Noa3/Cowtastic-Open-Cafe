using UnityEngine;

/// <summary>
/// Take a look to the Milk Change Buttons in the Customize Menu for the values
/// </summary>

public class Event_ChangeMilkColor : EventBase
{
    [Header("Event Settings")]

    [Min(0)]
    public int milkPreset = 0;

    public bool AfterEventRestoreOrginal = true;

    private int OriginalMilkPreset = 0;

    public override void SetEventState(bool state)
    {
        if (state == true)
        {
            OriginalMilkPreset = MilkTypeController.instance.lastMilkPreset;

            MilkTypeController.instance.SetMilkByPreset(milkPreset);
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
        MilkTypeController.instance.SetMilkByPreset(OriginalMilkPreset);
    }
}
