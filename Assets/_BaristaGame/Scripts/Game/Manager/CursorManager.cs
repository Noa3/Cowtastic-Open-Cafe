using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The whole script need to be overworked it eats heavy performance
/// </summary>

public class CursorManager : MonoBehaviour
{
    [Header("References")]
    public Texture2D CursorDefault;
    public Texture2D CursorInteract;
    public Texture2D CursorMouseDown;

    [Header("Setting")]
    public bool ChangeCursor = true;

    public static CursorManager instance;

    int UILayer;
    private CursorState currentState = CursorState.Default;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        if (ChangeCursor == true)
        {
            Cursor.SetCursor(CursorDefault, Vector2.zero, CursorMode.ForceSoftware);
        }


        UILayer = LayerMask.NameToLayer("UI");
    }

#if UNITY_STANDALONE || UNITY_WEBGL
    private void Update()
    {
        if (ChangeCursor == true)
        {
            if (Input.GetMouseButton(0))
            {
                setCursor(CursorState.MouseDown);
            }
            //else if (IsPointerOverUIElement() == true) // need to much power
            //{
            //    setCursor(CursorState.Interact);
            //}
            else
            {
                setCursor(CursorState.Default);
            }


        }

    }
#endif

    public void setCursor(CursorState mode)
    {
        if (currentState == mode)
        {
            return;
        }

        currentState = mode;

        Texture2D tex = CursorDefault;

        switch (mode)
        {
            case CursorState.Default:
                tex = CursorDefault;
                break;
            case CursorState.Interact:
                tex = CursorInteract;
                break;
            case CursorState.MouseDown:
                tex = CursorMouseDown;
                break;
            default:
                break;
        }

        Cursor.SetCursor(tex, Vector2.zero, CursorMode.ForceSoftware);
    }

    /// <summary>
    /// Static variant for setCursor, find automaticly the instance and set cursor
    /// </summary>
    /// <param name="mode"></param>
    public static void SetCursor(CursorState mode)
    {
        CursorManager.instance.setCursor(mode);
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}

public enum CursorState
{
    Default,
    Interact,
    MouseDown,
    Disabled
}
