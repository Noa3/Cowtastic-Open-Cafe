using UnityEngine;

public class CafeVisualsController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    //[Header("Settings")]



    public bool StatsLightning
    {
        set
        {
            if (value == true)
            {
                animator.SetBool(Statics.CaveVisualsStatsLightning, true);
            }
            else
            {
                animator.SetBool(Statics.CaveVisualsStatsLightning, false);
            }
        }
    }


    // Update is called once per frame
    //void Update()
    //{
    //    animator.SetBool(Statics.CaveVisualsStatsLightning, StatsLightning);
    //}

    public void SetStatsLightning(bool on)
    {
        StatsLightning = on;
    }
}
