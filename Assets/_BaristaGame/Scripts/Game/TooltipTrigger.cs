using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isActive = true;

    public string header;
    public string content;

    public float ShowDelay = 0.5f;

    private bool isShowingTip = false;

#if UNITY_EDITOR
    [Header("Debug Only")]
    [ReadOnly]
    public bool MouseOver = false;
#endif

    //private float timeSinceMouseIsOnElement = 0;

    //Works with gameobjects and Colliders
    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        OnPointerEnter(null);
    }

    //Works with gameobjects and Colliders
    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        OnPointerExit(null);
    }

    //For Gui Elements
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isActive && isShowingTip == false)
        {
            Invoke("ShowTooltip", ShowDelay);
            isShowingTip = true;
        }

#if UNITY_EDITOR
        MouseOver = true;
#endif

    }

    //For Gui Elements
    public void OnPointerExit(PointerEventData eventData)
    {
        CancelInvoke();
        TooltipManager.Hide();
        isShowingTip = false;

#if UNITY_EDITOR
        MouseOver = false;
#endif
    }


    private void OnMouseDown()
    {
        OnPointerExit(null);
    }

    public void ShowTooltip()
    {
        TooltipManager.Show(content, header);
    }

    public void SetTextHeader(string text)
    {
        header = text;
    }

    public void SetTextContent(string text)
    {
        content = text;
    }
}
