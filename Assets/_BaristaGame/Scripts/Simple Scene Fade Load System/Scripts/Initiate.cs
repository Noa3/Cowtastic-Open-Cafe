using UnityEngine;
using UnityEngine.UI;

public static class Initiate
{
    private static bool areWeFading = false;
    private static readonly object fadeLock = new object();

    public static void Fade(string scene, Color col, float multiplier)
    {
        lock (fadeLock)
        {
            if (areWeFading)
            {
                Debug.Log("Already Fading");
                return;
            }
            areWeFading = true;
        }

        // Ensure minimum fade speed to prevent infinite black screen
        float safeFadeSpeed = Mathf.Max(multiplier, 0.1f);
        CreateFaderObject(scene, col, safeFadeSpeed);
    }

    private static void CreateFaderObject(string scene, Color fadeColor, float multiplier)
    {
        var faderObject = new GameObject("Fader");

        // Setup Canvas
        var canvas = faderObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000; // Ensure it's on top

        // Add required components
        var canvasGroup = faderObject.AddComponent<CanvasGroup>();
        var image = faderObject.AddComponent<Image>();
        
        // Configure image
        image.color = fadeColor;
        image.raycastTarget = false; // Prevent blocking input after fade

        // Setup and initialize Fader
        var fader = faderObject.AddComponent<Fader>();
        InitializeFader(fader, scene, fadeColor, multiplier);
    }

    private static void InitializeFader(Fader fader, string scene, Color fadeColor, float multiplier)
    {
        fader.fadeDamp = multiplier;
        fader.fadeScene = scene;
        fader.fadeColor = fadeColor;
        fader.start = true;
        fader.InitiateFader();
    }

    public static void DoneFading()
    {
        lock (fadeLock)
        {
            areWeFading = false;
        }
        Statics.CleanUpGabarge();
    }
}