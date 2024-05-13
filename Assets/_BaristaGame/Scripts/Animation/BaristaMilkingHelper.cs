using UnityEngine;

public class BaristaMilkingHelper : MonoBehaviour
{

    //private BaristaController barsita;
    private BaseGameMode gamemode;

    public static float WaitBetweenMilkings = 0.2f;

    private float TimelastMilking = 0;
    private bool MilkingFailed = false;

    private void Start()
    {
        //barsita = BaristaController.instance;
        gamemode = BaseGameMode.instance;
    }

    void OnMouseDown()
    {
        //Debug.Log("Pressed somethhing!");

        if (Time.timeSinceLevelLoad > (TimelastMilking + WaitBetweenMilkings) && MilkingFailed == false)
        {
            //barsita.BeeingMilked = true;
            gamemode.TryMilking = true;
        }
        else
        {
            MilkingFailed = true;
        }

        //barsita.BeeingMilked = true;
        //gamemode.BeeingMilked = true;
    }

    private void OnMouseDrag()
    {
        if (MilkingFailed == false)
        {
            //barsita.BeeingMilked = true;
            gamemode.TryMilking = true;
        }

        //barsita.BeeingMilked = true;
        //gamemode.BeeingMilked = true;
    }

    private void OnMouseUp()
    {
        //barsita.BeeingMilked = false;
        gamemode.TryMilking = false;

        if (MilkingFailed == false)
        {
            TimelastMilking = Time.timeSinceLevelLoad;
        }


        MilkingFailed = false;
    }
}
