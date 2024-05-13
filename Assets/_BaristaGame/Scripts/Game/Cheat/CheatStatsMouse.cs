using UnityEngine;
using TMPro;

public class CheatStatsMouse : MonoBehaviour
{
    [Header("Automatic References")]


    [Header("Text References")]
    public TextMeshProUGUI ScreenStats;
    public TextMeshProUGUI MouseX;
    public TextMeshProUGUI MouseY;
    public TextMeshProUGUI MouseXCenterOffset;
    public TextMeshProUGUI MouseYCenterOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        int currentScreenSizeX = Screen.width;
        int currentScreenSizeY = Screen.height;

        ScreenStats.text = currentScreenSizeX + "x" + currentScreenSizeY;
        Vector3 mousePos = Input.mousePosition;
        MouseX.text = mousePos.x.ToString();
        MouseY.text = mousePos.y.ToString();

        MouseXCenterOffset.text = (mousePos.x - (currentScreenSizeX / 2)).ToString();
        MouseYCenterOffset.text = (mousePos.y - (currentScreenSizeY / 2)).ToString();
    }
}
