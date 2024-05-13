using UnityEngine;
using static Archievements;

public class ArchievementItem : MonoBehaviour
{
    [Header("References")]
    public GameObject HiddenObject;
    public GameObject UnlockedObject;

    [Header("Settings")]
    public ArchievementID id;

    public bool isHidden = false;

    [Header("Debug/Etc.")]
    [ReadOnly]
    public bool Unlocked;


    void OnEnable()
    {
        Unlocked = bool.Parse( PlayerPrefs.GetString(Consts.PlayerPrefPrefix + id.ToString(), Unlocked.ToString()) );
        if (Unlocked == true)
        {
            UnlockArchievement();
        }
        else
        {
            SetHiddenState(isHidden);
        }
    }

    public void UnlockArchievement()
    {
        SetHiddenState(false);
        UnlockedObject.SetActive(true);
    }

    private void SetHiddenState(bool state)
    {
        if (HiddenObject != null)
        {
            HiddenObject.SetActive(state);
            isHidden = state;
        }
    }
}
