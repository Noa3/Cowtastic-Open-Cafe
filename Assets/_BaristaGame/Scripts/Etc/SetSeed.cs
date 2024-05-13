using UnityEngine;
using TMPro;

public class SetSeed : MonoBehaviour
{
    public TMP_InputField input;

    private int newSeed;
    // Start is called before the first frame update
    void Start()
    {
        newSeed = Statics.GetRandomRange(100, int.MaxValue);

        if (input != null)
        {
            input.text = newSeed.ToString();
        }
        SeedSet(newSeed.ToString());
    }

    public void SeedSet(string seed)
    {
        if (string.IsNullOrWhiteSpace(seed) == true)
        {
            PlayerPrefs.SetInt(Consts.PlayerPrefRandomSeed, newSeed);
        }
        else
        {
            PlayerPrefs.SetInt(Consts.PlayerPrefRandomSeed, Mathf.Abs(int.Parse(seed)));
        }
    }
}
