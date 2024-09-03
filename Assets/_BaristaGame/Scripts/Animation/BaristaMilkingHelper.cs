using UnityEngine;

public class BaristaMilkingHelper : MonoBehaviour
{
    public static BaristaMilkingHelper instance;

    private BaseGameMode gamemode;

    public static float WaitBetweenMilkings = 0.2f;

    private float TimelastMilking = 0;
    private bool MilkingFailed = false;

    private void Start()
    {
        instance = this;
        gamemode = BaseGameMode.instance;
    }

    void OnMouseDown()
    {
        KeyBindingManager.instance.MouseDown();
        StartMilking();
    }

    public void StartMilking()
    {
        if (Time.timeSinceLevelLoad > (TimelastMilking + WaitBetweenMilkings) && MilkingFailed == false)
        {
            gamemode.TryMilking = true;
        }
        else
        {
            MilkingFailed = true;
        }
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

    public void StopMilking()
    {
        gamemode.TryMilking = false;

        if (MilkingFailed == false)
        {
            TimelastMilking = Time.timeSinceLevelLoad;
        }

        MilkingFailed = false;
    }

    private void OnMouseUp()
    {
        KeyBindingManager.instance.MouseUp();
        StopMilking();
    }
}
