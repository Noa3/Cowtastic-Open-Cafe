using UnityEngine;

/// <summary>
/// This Script is supposed to Destroy the attached Gameobject on specified Platforms
/// </summary>

public class DestroyOnPlatform : MonoBehaviour
{
    [SerializeField]
    private RuntimePlatform[] platformsToDestroyOn;

    public void Awake()
    {
        if (platformsToDestroyOn != null && platformsToDestroyOn.Length > 0)
        {
            for (int i = 0; i < platformsToDestroyOn.Length; i++)
            {
                if (Application.platform == platformsToDestroyOn[i])
                {
                    DestroyImmediate(gameObject);
                    return;
                }
            }
        }

        Destroy(this);
    }
}