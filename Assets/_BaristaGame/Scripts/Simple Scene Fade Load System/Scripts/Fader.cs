using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [HideInInspector]
    public bool start = false;
    [HideInInspector]
    public float fadeDamp = 0.0f;
    [HideInInspector]
    public string fadeScene;
    [HideInInspector]
    public float alpha = 0.0f;
    [HideInInspector]
    public Color fadeColor;
    [HideInInspector]
    public bool isFadeIn = false;
    CanvasGroup myCanvas;
    Image bg;
    float lastTime = 0;
    bool startedLoading = false;
    
    //Set callback
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    //Remove callback
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void InitiateFader()
    {
        // Ensure time scale is properly set for fading
        if (Time.timeScale == 0)
        {
            Debug.LogWarning("Fader: Time.timeScale was 0, setting to 1 for fade transition");
            Time.timeScale = 1f;
        }

        DontDestroyOnLoad(gameObject);

        //Getting the visual elements
        if (transform.GetComponent<CanvasGroup>())
            myCanvas = transform.GetComponent<CanvasGroup>();

        if (transform.GetComponentInChildren<Image>())
        {
            bg = transform.GetComponent<Image>();
            bg.color = fadeColor;
        }
        //Checking and starting the coroutine
        if (myCanvas && bg)
        {
            myCanvas.alpha = 0.0f;
            StartCoroutine(FadeIt());
        }
        else
            Debug.LogWarning("Something is missing please reimport the package.");
    }

    IEnumerator FadeIt()
    {

        while (!start)
        {
            //waiting to start
            yield return null;
        }
        lastTime = Time.time;
        float coDelta = lastTime;
        bool hasFadedIn = false;

        while (!hasFadedIn)
        {
            coDelta = Time.time - lastTime;
            if (!isFadeIn)
            {
                //Fade in
                alpha = newAlpha(coDelta, 1, alpha);
                if (alpha == 1 && !startedLoading)
                {
                    startedLoading = true;

                    if (string.IsNullOrEmpty(fadeScene) == false)
                    {
                        StartCoroutine(LoadSceneWithFallback(fadeScene));
                    }
                }

            }
            else
            {
                //Fade out
                alpha = newAlpha(coDelta, 0, alpha);
                if (alpha == 0)
                {
                    hasFadedIn = true;
                }


            }
            lastTime = Time.time;
            myCanvas.alpha = alpha;
            yield return null;
        }

        Initiate.DoneFading();

       // Debug.Log("Your scene has been loaded , and fading in has just ended");

        Destroy(gameObject);

        yield return null;
    }


    float newAlpha(float delta, int to, float currAlpha)
    {

        switch (to)
        {
            case 0:
                currAlpha -= fadeDamp * delta;
                if (currAlpha <= 0)
                    currAlpha = 0;

                break;
            case 1:
                currAlpha += fadeDamp * delta;
                if (currAlpha >= 1)
                    currAlpha = 1;

                break;
        }

        return currAlpha;
    }

    private IEnumerator LoadSceneWithFallback(string sceneName)
    {
        var locationsHandle = Addressables.LoadResourceLocationsAsync(sceneName);
        yield return locationsHandle;

        bool hasAddressableLocation =
            locationsHandle.Status == AsyncOperationStatus.Succeeded &&
            locationsHandle.Result != null &&
            locationsHandle.Result.Count > 0;

        Addressables.Release(locationsHandle);

        if (hasAddressableLocation)
        {
            Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, true);
        }
        else
        {
            Debug.LogWarning($"Fader: Addressable key '{sceneName}' not found. Falling back to SceneManager.LoadScene.");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // Ensure time scale is reset when scene loads
        if (Time.timeScale != 1f)
        {
            Debug.LogWarning($"Fader: Time.timeScale was {Time.timeScale} when scene loaded, resetting to 1");
            Time.timeScale = 1f;
        }
        
        //We can now fade in - just set the flag, don't start a new coroutine
        isFadeIn = true;
    }
}
