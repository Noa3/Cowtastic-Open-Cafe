using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

public class CameraPan : MonoBehaviour
{

    //how much should the camera pan to the side
    public float MaxPanX = 2;
    public float MaxPanY = 0.5f;

    float currentScreenSizeX;
    float currentScreenSizeY;

    private float2 screenSize;
    private float3 orgPos;

    public static CameraPan instance;

    private void Awake()
    {
        instance = this;
        orgPos = transform.position;
    }

    public void OnDisable()
    {
        transform.position = orgPos;
    }

    [BurstCompile]
    private void UpdateScreenSize()
    {
        screenSize = new float2(Screen.width, Screen.height);
    }

    //Old logic
    //private void LateUpdate()
    //{
    //    currentScreenSizeX = Screen.width;
    //    currentScreenSizeY = Screen.height;

    //    Vector3 mousePos = Input.mousePosition;
    //    Vector3 newPos = new Vector3(
    //        orgPos.x + (((mousePos.x / currentScreenSizeX) * 2.0f - 1.0f) * MaxPanX),
    //        orgPos.y + (((mousePos.y / currentScreenSizeY) * 2.0f - 1.0f) * MaxPanY),
    //        orgPos.z);

    //    transform.position = newPos;
    //}

    private void LateUpdate()
    {
        UpdateScreenSize();

        float2 mousePos = new float2(Input.mousePosition.x, Input.mousePosition.y);
        float2 normalizedMousePos = mousePos / screenSize;
        float2 scaledMousePos = (normalizedMousePos * 2f) - new float2(1f, 1f);

        float3 newPos = orgPos + new float3(
            scaledMousePos.x * MaxPanX,
            scaledMousePos.y * MaxPanY,
            0f);

        transform.position = newPos;
    }
}
