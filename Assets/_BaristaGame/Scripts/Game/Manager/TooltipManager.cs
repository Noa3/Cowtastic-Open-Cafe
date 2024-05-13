using UnityEngine;
using TMPro;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class TooltipManager : MonoBehaviour
{
    [Header("References")]
    public GameObject TooltipGameobject;
    public TextMeshProUGUI HeaderText;
    public TextMeshProUGUI TooltipText;

    public RectTransform TooltipRectTransform;

    public LayoutElement layoutElemennt;

    [Header("Settings")]
    public int characterWrapLimit;
    public Vector2 WindowOffset = Vector2.zero;

    public static TooltipManager instance;

    public void Awake()
    {
        instance = this;
        HideTooltip();
    }

    public void Update()
    {
        //int headerLength = HeaderText.text.Length;
        //int contentLength = TooltipText.text.Length;

        //layoutElemennt.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;

        Vector2 position = Input.mousePosition;
        //position.x = position.x + WindowOffset.x;
        //position.y = position.y + WindowOffset.y;

        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;

        TooltipRectTransform.pivot = new Vector2(pivotX + WindowOffset.x, pivotY+ WindowOffset.y);


        TooltipGameobject.transform.position = position;

        if (Input.GetMouseButtonDown(0))
        {
            HideTooltip();
        }
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            HeaderText.gameObject.SetActive(false);
        }
        else
        {
            HeaderText.gameObject.SetActive(true);
            HeaderText.text = header;
        }

        TooltipText.text = content;

        int headerLength = HeaderText.text.Length;
        int contentLength = TooltipText.text.Length;

        layoutElemennt.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }

    public void ShowTooltip()
    {
        TooltipGameobject.SetActive(true);
    }

    public void HideTooltip()
    {
        TooltipGameobject.SetActive(false);
    }

    public static void Show(string content, string header = "")
    {
        if (instance != null)
        {
            instance.SetText(content, header);
            instance.ShowTooltip();
        }
    }

    public static void Hide()
    {
        if (instance != null)
        {
            instance.HideTooltip();
        }
    }
}
