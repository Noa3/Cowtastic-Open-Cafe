using UnityEngine;
using UnityEngine.Events;

public class EventHelper : MonoBehaviour
{
    public UnityEvent[] EventsOneShot;

#if UNITY_EDITOR
    public bool DebugMessages = false;
#endif

    public void CallEventOneShot(int number = 0)
    {
#if UNITY_EDITOR
        if (DebugMessages == true)
        {
            Debug.Log("Play Event: " + number, gameObject);
        }
#endif
        EventsOneShot[number].Invoke();
    }



}
