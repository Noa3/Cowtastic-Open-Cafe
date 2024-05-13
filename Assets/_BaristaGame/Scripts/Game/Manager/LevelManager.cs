using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{

    //}

    private const float FadeTime = 2.0f;
    private static Color FadeColor = Color.black;

    public static void ReloadScene()
    {
        Time.timeScale = 1;
        Initiate.Fade(SceneManager.GetActiveScene().name, FadeColor, FadeTime);
    }

    public static void ChangeScene(string sceneName)
    {
        Time.timeScale = 1;
        Initiate.Fade(sceneName, FadeColor, FadeTime);
    }

    public static void ChangeSceneDirect(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
        //Initiate.Fade(sceneName, Color.black, FadeTime);
    }

    public static void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
