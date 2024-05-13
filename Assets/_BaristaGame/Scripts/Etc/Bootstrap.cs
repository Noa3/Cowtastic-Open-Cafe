using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{

    [Header("Settings")]
    public string NextScene = "";
    public bool SetDefoultCursor = true;

    private const string GameVersionString = "GameVersion";
    private void Awake()
    {
        //Checks if the Gameversion in the Storage is the same, if not delete everything and then Write again the Game Version
        if (string.Equals( PlayerPrefs.GetString(GameVersionString, "") , Consts.PrefixNewGameVersion) == false)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString(GameVersionString, Consts.PrefixNewGameVersion);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(NextScene);
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
