using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    [Header("Pan Settings")]
    [Tooltip("Maximum horizontal pan distance")]
    public float MaxPanX = 2f;

    [Tooltip("Maximum vertical pan distance")]
    public float MaxPanY = 0.5f;

    [Header("Mobile Settings")]
    [Tooltip("Enable gyroscope-based camera panning on mobile")]
    public bool enableGyroscopePan = true;

    [Tooltip("Sensitivity for gyroscope panning")]
    [Range(0.1f, 5f)]
    public float gyroscopeSensitivity = 1.5f;

    [Tooltip("Deadzone for gyroscope input (ignore small movements)")]
    [Range(0f, 0.5f)]
    public float gyroscopeDeadzone = 0.05f;

    [Tooltip("Smoothing for gyroscope input")]
    [Range(0.1f, 1f)]
    public float gyroscopeSmoothing = 0.8f;

    [Header("Aspect Ratio Management")]
    [Tooltip("Desired game area width in world units")]
    public float desiredGameAreaWidth = 10f;

    [Tooltip("Desired game area height in world units")]
    public float desiredGameAreaHeight = 6f;

    [Tooltip("Minimum orthographic size")]
    public float minOrthographicSize = 3f;

    [Tooltip("Maximum orthographic size")]
    public float maxOrthographicSize = 8f;

    [Tooltip("Minimum field of view for perspective cameras")]
    [Range(10f, 90f)]
    public float minFieldOfView = 30f;

    [Tooltip("Maximum field of view for perspective cameras")]
    [Range(30f, 120f)]
    public float maxFieldOfView = 80f;

    [Tooltip("Padding around game area (percentage)")]
    [Range(0f, 0.3f)]
    public float gameAreaPadding = 0.1f;

    [Header("Performance")]
    [Tooltip("Smoothing factor for camera movement")]
    [Range(0.1f, 1f)]
    public float smoothing = 0.8f;

    private Camera cameraComponent;
    private float2 screenSize;
    private float3 orgPos;
    private float3 targetPos;
    private float3 currentPos;

    // Original camera settings
    private float originalOrthographicSize;
    private float originalFieldOfView;

    // Current camera settings
    private float currentOrthographicSize;
    private float currentFieldOfView;

    private bool isPerspectiveCamera;
    private bool isMobilePlatform;

    // Cache for performance
    private int lastScreenWidth;
    private int lastScreenHeight;

    // Mobile sensor support
    private bool gyroscopeSupported;
    private Quaternion initialDeviceRotation;
    private Vector3 smoothedGyroInput;
    private bool sensorsInitialized;

    public static CameraPan Instance { get; private set; }

    // Compatibility alias for the open-source variant which references the lowercase 'instance'.
    public static CameraPan instance => Instance;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        cameraComponent = GetComponent<Camera>();
        if (cameraComponent == null)
        {
            Debug.LogError("CameraPan: Camera component not found!");
            return;
        }

        InitializeCamera();
        InitializeMobileSensors();
        UpdateScreenSize();
        AdjustCameraForAspectRatio();
        AdjustPanLimitsForAspectRatio();
        ResetPosition();
    }

    private void InitializeCamera()
    {
        orgPos = transform.position;
        currentPos = orgPos;
        targetPos = orgPos;

        isPerspectiveCamera = !cameraComponent.orthographic;
        originalOrthographicSize = cameraComponent.orthographicSize;
        originalFieldOfView = cameraComponent.fieldOfView;
        currentOrthographicSize = originalOrthographicSize;
        currentFieldOfView = originalFieldOfView;

        isMobilePlatform = Application.isMobilePlatform;
    }

    private void InitializeMobileSensors()
    {
        if (!isMobilePlatform) return;

        gyroscopeSupported = SystemInfo.supportsGyroscope;
        if (gyroscopeSupported && enableGyroscopePan)
        {
            Input.gyro.enabled = true;
            Invoke(nameof(CalibrateGyroscope), 0.5f);
        }
    }

    private void CalibrateGyroscope()
    {
        if (gyroscopeSupported && Input.gyro.enabled)
        {
            initialDeviceRotation = Input.gyro.attitude;
            smoothedGyroInput = Vector3.zero;
            sensorsInitialized = true;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (isMobilePlatform && gyroscopeSupported)
        {
            Input.gyro.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (cameraComponent != null)
        {
            ResetPosition();
        }
    }

    private void OnDisable()
    {
        if (cameraComponent != null)
        {
            ResetPosition();
        }
    }

    [BurstCompile]
    private void UpdateScreenSize()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            screenSize = new float2(Screen.width, Screen.height);
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;

            AdjustCameraForAspectRatio();
            AdjustPanLimitsForAspectRatio();
        }
    }

    private void AdjustCameraForAspectRatio()
    {
        if (cameraComponent == null) return;

        float currentAspect = screenSize.x / screenSize.y;
        float paddedWidth = desiredGameAreaWidth * (1f + gameAreaPadding);
        float paddedHeight = desiredGameAreaHeight * (1f + gameAreaPadding);

        if (isPerspectiveCamera)
        {
            AdjustPerspectiveCameraFOV(currentAspect, paddedWidth, paddedHeight);
        }
        else
        {
            AdjustOrthographicCameraSize(currentAspect, paddedWidth, paddedHeight);
        }
    }

    private void AdjustPerspectiveCameraFOV(float currentAspect, float paddedWidth, float paddedHeight)
    {
        float distanceToGameArea = math.abs(orgPos.z);
        if (distanceToGameArea < 0.1f) distanceToGameArea = 10f;

        float requiredFOVForHeight = 2f * math.atan(paddedHeight / (2f * distanceToGameArea)) * Mathf.Rad2Deg;
        float widthAtDistance = paddedWidth / currentAspect;
        float requiredFOVForWidth = 2f * math.atan(widthAtDistance / (2f * distanceToGameArea)) * Mathf.Rad2Deg;

        float newFOV = math.max(requiredFOVForHeight, requiredFOVForWidth);
        newFOV = math.clamp(newFOV, minFieldOfView, maxFieldOfView);

        currentFieldOfView = newFOV;
        cameraComponent.fieldOfView = currentFieldOfView;
    }

    private void AdjustOrthographicCameraSize(float currentAspect, float paddedWidth, float paddedHeight)
    {
        float requiredSizeForWidth = paddedWidth / (2f * currentAspect);
        float requiredSizeForHeight = paddedHeight / 2f;

        float newOrthographicSize = math.max(requiredSizeForWidth, requiredSizeForHeight);
        newOrthographicSize = math.clamp(newOrthographicSize, minOrthographicSize, maxOrthographicSize);

        currentOrthographicSize = newOrthographicSize;
        cameraComponent.orthographicSize = currentOrthographicSize;
    }

    private void AdjustPanLimitsForAspectRatio()
    {
        Vector2 visibleArea = GetVisibleWorldArea();
        float extraWidth = math.max(0f, visibleArea.x - desiredGameAreaWidth);
        float extraHeight = math.max(0f, visibleArea.y - desiredGameAreaHeight);

        MaxPanX = math.clamp(extraWidth * 0.4f, 0.2f, 3f);
        MaxPanY = math.clamp(extraHeight * 0.4f, 0.1f, 2f);
    }

    [BurstCompile]
    private float3 CalculateTargetPosition(float2 inputPos)
    {
        float2 normalizedInputPos = (inputPos / screenSize) * 2f - new float2(1f);
        return orgPos + new float3(
            normalizedInputPos.x * MaxPanX,
            normalizedInputPos.y * MaxPanY,
            0f);
    }

    [BurstCompile]
    private float3 CalculateTargetPositionFromSensorInput(Vector2 sensorInput)
    {
        return orgPos + new float3(
            sensorInput.x * MaxPanX,
            sensorInput.y * MaxPanY,
            0f);
    }

    private void LateUpdate()
    {
        UpdateScreenSize();

        if (isMobilePlatform)
        {
            HandleMobileInput();
        }
        else
        {
            HandleDesktopInput();
        }
    }

    private void HandleMobileInput()
    {
        if (enableGyroscopePan && gyroscopeSupported && sensorsInitialized)
        {
            Vector2 gyroInput = GetGyroscopeInput();
            if (gyroInput.magnitude > gyroscopeDeadzone)
            {
                targetPos = CalculateTargetPositionFromSensorInput(gyroInput);
                ApplyCameraMovement();
                return;
            }
        }

        // If no sensor input, keep camera at original position
        targetPos = orgPos;
        ApplyCameraMovement();
    }

    private Vector2 GetGyroscopeInput()
    {
        if (!gyroscopeSupported || !Input.gyro.enabled)
            return Vector2.zero;

        Quaternion currentRotation = Input.gyro.attitude;
        Quaternion deltaRotation = Quaternion.Inverse(initialDeviceRotation) * currentRotation;
        Vector3 deltaEuler = deltaRotation.eulerAngles;

        // Convert to [-180, 180] range
        if (deltaEuler.x > 180f) deltaEuler.x -= 360f;
        if (deltaEuler.z > 180f) deltaEuler.z -= 360f;

        Vector3 rawInput = new Vector3(-deltaEuler.z, deltaEuler.x, 0f) * gyroscopeSensitivity * 0.01f;
        smoothedGyroInput = Vector3.Lerp(smoothedGyroInput, rawInput, gyroscopeSmoothing * Time.deltaTime * 10f);

        return new Vector2(
            Mathf.Clamp(smoothedGyroInput.x, -1f, 1f),
            Mathf.Clamp(smoothedGyroInput.y, -1f, 1f)
        );
    }

    private void HandleDesktopInput()
    {
        float2 mousePos = new float2(Input.mousePosition.x, Input.mousePosition.y);
        targetPos = CalculateTargetPosition(mousePos);
        ApplyCameraMovement();
    }

    [BurstCompile]
    private void ApplyCameraMovement()
    {
        if (smoothing < 1f)
        {
            currentPos = math.lerp(currentPos, targetPos, smoothing * Time.deltaTime * 10f);
        }
        else
        {
            currentPos = targetPos;
        }
        
        // Ensure Z position never changes
        currentPos.z = orgPos.z;
        transform.position = currentPos;
    }

    // Public methods
    public void ResetPosition()
    {
        transform.position = orgPos;
        currentPos = orgPos;
        targetPos = orgPos;

        if (isMobilePlatform && sensorsInitialized)
        {
            CalibrateGyroscope();
        }
    }

    public void RecalibrateSensors()
    {
        if (isMobilePlatform)
        {
            CalibrateGyroscope();
        }
    }

    public void SetGyroscopePanEnabled(bool enabled)
    {
        enableGyroscopePan = enabled;
        if (isMobilePlatform && gyroscopeSupported)
        {
            Input.gyro.enabled = enabled;
            if (enabled)
            {
                CalibrateGyroscope();
            }
        }
    }

    public void SetPanLimits(float maxX, float maxY)
    {
        MaxPanX = maxX;
        MaxPanY = maxY;
    }

    public void SetDesiredGameArea(float width, float height)
    {
        desiredGameAreaWidth = width;
        desiredGameAreaHeight = height;
        AdjustCameraForAspectRatio();
        AdjustPanLimitsForAspectRatio();
    }

    public Vector2 GetVisibleWorldArea()
    {
        if (cameraComponent == null) return Vector2.zero;

        if (isPerspectiveCamera)
        {
            float distanceToGameArea = math.abs(orgPos.z);
            if (distanceToGameArea < 0.1f) distanceToGameArea = 10f;

            float height = 2f * math.tan(currentFieldOfView * 0.5f * Mathf.Deg2Rad) * distanceToGameArea;
            float width = height * (screenSize.x / screenSize.y);
            return new Vector2(width, height);
        }
        else
        {
            float height = currentOrthographicSize * 2f;
            float width = height * (screenSize.x / screenSize.y);
            return new Vector2(width, height);
        }
    }

    public void SetGameAreaPadding(float padding)
    {
        gameAreaPadding = math.clamp(padding, 0f, 0.5f);
        AdjustCameraForAspectRatio();
        AdjustPanLimitsForAspectRatio();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? orgPos : transform.position;

        // Draw pan boundaries
        Gizmos.DrawWireCube(center, new Vector3(MaxPanX * 2f, MaxPanY * 2f, 0f));

        // Draw desired game area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, new Vector3(desiredGameAreaWidth, desiredGameAreaHeight, 0f));

        // Draw padded game area
        float paddedWidth = desiredGameAreaWidth * (1f + gameAreaPadding);
        float paddedHeight = desiredGameAreaHeight * (1f + gameAreaPadding);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, new Vector3(paddedWidth, paddedHeight, 0f));

        // Draw current visible area if camera exists
        if (Application.isPlaying && cameraComponent != null)
        {
            Vector2 visibleArea = GetVisibleWorldArea();
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(center, new Vector3(visibleArea.x, visibleArea.y, 0f));
        }
    }
#endif
}