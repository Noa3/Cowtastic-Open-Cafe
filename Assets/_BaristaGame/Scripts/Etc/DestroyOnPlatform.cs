using UnityEngine;

/// <summary>
/// This Script is supposed to Destroy the attached Gameobject on specifyed Platform
/// </summary>

public class DestroyOnPlatform : MonoBehaviour
{
    public RuntimePlatform platformDestroyTo;

    public void Awake()
    {
        if (Application.platform == platformDestroyTo)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }


}
