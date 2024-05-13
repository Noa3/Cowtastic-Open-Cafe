using UnityEngine;


public class InteractiveElement : MonoBehaviour
{
    private SoundEffectManager soundManager;
    private CursorManager cursorManager;


    public void Start()
    {
        GetManager();
    }

    public void PlaySelect()
    {
        if (soundManager == null)
        {
            GetManager();
        }
        soundManager.PlayMenuSelection();

        if (cursorManager != null)
        {
            cursorManager.setCursor(CursorState.Interact);
        }
    }

    public void PlayClick()
    {
        soundManager.PlayMouseClick();
    }

    private void OnMouseExit()
    {
        cursorManager.setCursor(CursorState.Default);
    }

    private void GetManager()
    {
        soundManager = SoundEffectManager.instance;
        cursorManager = CursorManager.instance;
    }
}
