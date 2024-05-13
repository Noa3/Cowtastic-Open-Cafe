using UnityEngine;

public class ArcardeIntroHelperPopupHelper : MonoBehaviour
{

    public bool showIntro = true;

    private void Awake()
    {
        showIntro = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefShowIntroPopup, "true" ));

        if (showIntro == false)
        {
            ClosePopup();
        }
        else
        {
            Time.timeScale= 0f;
        }
    }

    public void SetShowNextTime(bool b)
    {
        PlayerPrefs.SetString(Consts.PlayerPrefShowIntroPopup,b.ToString());
    }

    public void ClosePopup()
    {
        Time.timeScale = 1f;
        Destroy(this.gameObject);
    }
}
