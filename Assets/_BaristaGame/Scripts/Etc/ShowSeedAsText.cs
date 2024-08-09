using TMPro;
using UnityEngine;

/// <summary>
/// Shows the Random Seed curently set on a text component
/// </summary>
public class ShowSeedAsText : MonoBehaviour
{
    private TextMeshProUGUI SeedText;

    private void Start()
    {
        SeedText = GetComponent<TextMeshProUGUI>();
        if (SeedText != null)
        {
            SeedText.text = PlayerPrefs.GetInt(Consts.PlayerPrefCurrentSeed,-1).ToString();
        }
        else
        {
            Debug.LogWarning("TMP Text component not found, are you sure this is the right object?", gameObject);
        }
    }
}
