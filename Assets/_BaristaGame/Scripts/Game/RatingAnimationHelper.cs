using UnityEngine;
using TMPro;

/// <summary>
/// This scirpt get to the CanvasGroupObject Atached and steers the fade out stuff
/// </summary>

public class RatingAnimationHelper : MonoBehaviour
{
    public CanvasGroup RatingGroup;
    public TextMeshProUGUI RatingText;

    public float TimeShown = 3.0f;
    public float FadeSpeed = 0.3f;

    public bool WaitedForShownTime = false;
    public bool IsFading = false;
    private float TimeSinceStart = 0;

    public static RatingAnimationHelper instance;

    private void Awake()
    {
        if (RatingGroup == null)
        {
            RatingGroup = GetComponent<CanvasGroup>();
        }
        instance = this;
        RatingGroup.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        if (IsFading == true)
        {
            TimeSinceStart = TimeSinceStart + Time.deltaTime;

            if (WaitedForShownTime == false)
            {
                if (TimeSinceStart > TimeShown)
                {
                    WaitedForShownTime = true;
                    TimeSinceStart = 0;
                }
                return;
            }

            RatingGroup.alpha = Mathf.Lerp(1,0, (TimeSinceStart * FadeSpeed));

            if (RatingGroup.alpha < 0.01f)
            {
                RatingGroup.alpha = 0;
                RatingGroup.gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        RatingGroup.alpha = 1;
        IsFading = true;
        WaitedForShownTime = false;
        TimeSinceStart = 0;
    }

    public void ShowRating(string text = "")
    {
        //Debug.Log("Showrating: " + text);
        if (string.IsNullOrEmpty(text) == false)
        {
            RatingText.text = text;
        }
        RatingText.color = Color.green;
        RatingGroup.gameObject.SetActive(true);
        OnEnable();
    }

    public void ShowRating(string text, Color RatingColor)
    {
        //Debug.Log("Showrating: " + text);
        if (string.IsNullOrEmpty(text) == false)
        {
            RatingText.text = text;
        }
        RatingText.color = RatingColor;
        RatingGroup.gameObject.SetActive(true);
        OnEnable();
    }

}
