using UnityEngine;
using TMPro;

public class SetSeed : MonoBehaviour
{
    public TMP_InputField input;

    private int chosenSeed;
    // Start is called before the first frame update
    void Start()
    {
        var chooseRandomSeed = true;
        if(PlayerPrefs.HasKey(Consts.PlayerPrefRandomSeed))
        {
            chosenSeed = PlayerPrefs.GetInt(Consts.PlayerPrefRandomSeed);
            if(chosenSeed >= 0)
            {
                chooseRandomSeed = false;
                PlayerPrefs.SetInt(Consts.PlayerPrefCurrentSeed, chosenSeed);
            }
        }
        if (chooseRandomSeed)
        {
            chosenSeed = Statics.GetRandomRange(0, int.MaxValue);
            PlayerPrefs.SetInt(Consts.PlayerPrefCurrentSeed, chosenSeed);
        }

        input.text = chosenSeed.ToString();
    }

    public void SeedSet(string seed)
    {
        if (string.IsNullOrWhiteSpace(seed) == true)
        {
            chosenSeed = Statics.GetRandomRange(0, int.MaxValue);
            PlayerPrefs.SetInt(Consts.PlayerPrefRandomSeed, -1);
        }
        else
        {
            chosenSeed = Mathf.Abs(int.Parse(seed));
            PlayerPrefs.SetInt(Consts.PlayerPrefRandomSeed, chosenSeed);
        }
        PlayerPrefs.SetInt(Consts.PlayerPrefCurrentSeed, chosenSeed);

        input.text = chosenSeed.ToString();
    }
}
