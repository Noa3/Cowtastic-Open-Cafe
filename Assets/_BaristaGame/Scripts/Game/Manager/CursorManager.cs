using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// CursorManager that persists across scene changes with mobile platform support
/// </summary>
public class CursorManager : MonoBehaviour
{
    [Header("References")]
    public Texture2D CursorDefault;
    public Texture2D CursorInteract;
    public Texture2D CursorMouseDown;

    [Header("Settings")]
    public bool ChangeCursor = true;
    [Tooltip("Time in seconds after last mouse input before hiding cursor on mobile")]
    public float MobileMouseTimeout = 3f;

    public static CursorManager instance;

    private int UILayer;
    private CursorState currentState = CursorState.Default;

    // Mobile platform detection and mouse input tracking
    private bool isMobilePlatform;
    private bool hasMouseInput = false;
    private float lastMouseInputTime = 0f;
    private Vector2 lastMousePosition;
    private bool cursorVisibleOnMobile = false;

    private void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (instance != null)
        {
            // If an instance already exists, destroy this one
            DestroyImmediate(gameObject);
            return;
        }

        // Set this as the singleton instance and persist across scenes
        instance = this;
        DontDestroyOnLoad(gameObject);

        UILayer = LayerMask.NameToLayer("UI");

        // Detect if we're running on a mobile platform
        DetectMobilePlatform();

        // Initialize cursor based on platform
        InitializeCursor();

        // Subscribe to scene loading events (modern Unity approach)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void DetectMobilePlatform()
    {
        // Check for mobile platforms using compile-time and runtime detection
#if UNITY_ANDROID || UNITY_IOS
            isMobilePlatform = true;
#else
        // Runtime check for additional mobile platforms or when platform switching in editor
        isMobilePlatform = Application.platform == RuntimePlatform.Android ||
                          Application.platform == RuntimePlatform.IPhonePlayer ||
                          Application.isMobilePlatform;
#endif

        // In editor, treat as desktop unless specifically testing mobile
#if UNITY_EDITOR
        // You can override this for testing by uncommenting the line below
        // isMobilePlatform = true;
#endif
    }

    private void InitializeCursor()
    {
        if (ChangeCursor)
        {
            if (isMobilePlatform)
            {
                // On mobile, start with cursor hidden
                Cursor.visible = false;
                cursorVisibleOnMobile = false;
            }
            else
            {
                // On desktop, show cursor normally
                Cursor.SetCursor(CursorDefault, Vector2.zero, CursorMode.ForceSoftware);
                Cursor.visible = true;
            }
        }

        lastMousePosition = Input.mousePosition;
    }

    /// <summary>
    /// Called when a new scene is loaded - modern Unity approach
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Refresh UI layer in case it changed in the new scene
        UILayer = LayerMask.NameToLayer("UI");

        // Ensure cursor state is maintained across scenes
        if (ChangeCursor && !isMobilePlatform)
        {
            // Reapply cursor on desktop platforms
            setCursor(currentState);
        }
    }

    private void Update()
    {
        if (!ChangeCursor) return;

        if (isMobilePlatform)
        {
            HandleMobileCursor();
        }
        else
        {
            HandleDesktopCursor();
        }
    }

    private void HandleMobileCursor()
    {
        // Check for mouse movement or input on mobile
        bool mouseInputDetected = CheckForMouseInput();

        if (mouseInputDetected)
        {
            hasMouseInput = true;
            lastMouseInputTime = Time.time;

            // Show cursor and update state if not already visible
            if (!cursorVisibleOnMobile)
            {
                ShowCursorOnMobile();
            }

            // Update cursor state based on input
            UpdateCursorState();
        }
        else if (hasMouseInput && Time.time - lastMouseInputTime > MobileMouseTimeout)
        {
            // Hide cursor after timeout
            HideCursorOnMobile();
        }
        else if (cursorVisibleOnMobile)
        {
            // Update cursor state while visible
            UpdateCursorState();
        }
    }

    private void HandleDesktopCursor()
    {
        // Original desktop behavior
#if UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButton(0))
        {
            setCursor(CursorState.MouseDown);
        }
        //else if (IsPointerOverUIElement() == true) // commented out due to performance
        //{
        //    setCursor(CursorState.Interact);
        //}
        else
        {
            setCursor(CursorState.Default);
        }
#endif
    }

    private bool CheckForMouseInput()
    {
        Vector2 currentMousePosition = Input.mousePosition;

        // Check for mouse movement
        bool mouseMoved = Vector2.Distance(currentMousePosition, lastMousePosition) > 1f;

        // Check for mouse button input
        bool mouseButtonInput = Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) ||
                               Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);

        // Check for scroll wheel input
        bool scrollInput = Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01f;

        lastMousePosition = currentMousePosition;

        return mouseMoved || mouseButtonInput || scrollInput;
    }

    private void UpdateCursorState()
    {
        if (Input.GetMouseButton(0))
        {
            setCursor(CursorState.MouseDown);
        }
        //else if (IsPointerOverUIElement() == true) // commented out due to performance
        //{
        //    setCursor(CursorState.Interact);
        //}
        else
        {
            setCursor(CursorState.Default);
        }
    }

    private void ShowCursorOnMobile()
    {
        if (!cursorVisibleOnMobile)
        {
            Cursor.SetCursor(CursorDefault, Vector2.zero, CursorMode.ForceSoftware);
            Cursor.visible = true;
            cursorVisibleOnMobile = true;
            currentState = CursorState.Default;
        }
    }

    private void HideCursorOnMobile()
    {
        if (cursorVisibleOnMobile)
        {
            Cursor.visible = false;
            cursorVisibleOnMobile = false;
            hasMouseInput = false;
        }
    }

    public void setCursor(CursorState mode)
    {
        // On mobile, only set cursor if it should be visible
        if (isMobilePlatform && !cursorVisibleOnMobile)
        {
            return;
        }

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
            case CursorState.Disabled:
                Cursor.visible = false;
                return;
        }

        Cursor.SetCursor(tex, Vector2.zero, CursorMode.ForceSoftware);

        // Ensure cursor is visible when setting a state (except Disabled)
        if (!Cursor.visible && mode != CursorState.Disabled)
        {
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Static variant for setCursor, find automatically the instance and set cursor
    /// </summary>
    public static void SetCursor(CursorState mode)
    {
        if (instance != null)
        {
            instance.setCursor(mode);
        }
    }

    /// <summary>
    /// Force show cursor (useful for UI interactions on mobile)
    /// </summary>
    public static void ForceShowCursor()
    {
        if (instance != null && instance.isMobilePlatform)
        {
            instance.ShowCursorOnMobile();
            instance.lastMouseInputTime = Time.time;
        }
    }

    /// <summary>
    /// Force hide cursor
    /// </summary>
    public static void ForceHideCursor()
    {
        if (instance != null)
        {
            if (instance.isMobilePlatform)
            {
                instance.HideCursorOnMobile();
            }
            else
            {
                instance.setCursor(CursorState.Disabled);
            }
        }
    }

    /// <summary>
    /// Check if we're currently on a mobile platform
    /// </summary>
    public static bool IsMobilePlatform()
    {
        return instance != null ? instance.isMobilePlatform : false;
    }

    /// <summary>
    /// Manually destroy the CursorManager (useful for cleanup)
    /// </summary>
    public static void DestroyInstance()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
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

    private void OnDestroy()
    {
        // Unsubscribe from scene events to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Clean up the static reference when destroyed
        if (instance == this)
        {
            instance = null;
        }
    }
}

public enum CursorState
{
    Default,
    Interact,
    MouseDown,
    Disabled
}