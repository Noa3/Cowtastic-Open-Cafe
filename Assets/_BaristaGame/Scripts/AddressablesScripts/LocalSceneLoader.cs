using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Unity.Burst;

namespace _BaristaGame.Scripts.AddressablesScripts
{
    /// <summary>
    /// Handles asynchronous scene loading and asset loading using Unity's Addressable Asset System.
    /// Provides loading progress UI feedback through a slider interface and scene fade transitions.
    /// Optimized for Unity 6.1 with proper error handling and resource management.
    /// 
    /// Usage:
    /// - Call LoadSceneAsync(sceneName) to load a scene with fade transition (default behavior)
    /// - Call LoadSceneWithoutFade(sceneName) to load a scene with progress display only
    /// - Call LoadAssetsAsync(assetLabels) to preload assets with progress display
    /// - Loading slider is automatically instantiated and managed
    /// </summary>
    public class LocalSceneLoader : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Bootstrap _bootstrap;

        [Header("Fade Settings")]
        [SerializeField] private Color fadeColor = Color.black;
        [SerializeField] private float fadeSpeed = 2.0f;

        [Header("Loading UI")]
        [SerializeField] private bool useAddressableLoadingScreen = false;

        [Header("Debug Info")]
        [SerializeField, ReadOnly] private bool _isLoading;
        [SerializeField, ReadOnly] private float _loadProgress;

        // Private fields for loading operations
        private AsyncOperationHandle _loadHandle;
        private Slider _sliderLoading;
        private GameObject _sliderLoadingGameObject;
        private CancellationTokenSource _cancellationTokenSource;

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeCancellationToken();
        }

        private void Update()
        {
            if (_isLoading && _loadHandle.IsValid())
            {
                UpdateLoadingProgress();
            }
        }

        private void OnDestroy()
        {
            CleanupResources();
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Loads a scene asynchronously with fade transition (default behavior)
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        public void LoadSceneAsync(string sceneName)
        {
            LoadSceneAsync(sceneName, fadeColor, fadeSpeed);
        }

        /// <summary>
        /// Loads a scene asynchronously with customizable fade transition
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <param name="customFadeColor">Color for the fade effect</param>
        /// <param name="customFadeSpeed">Speed of the fade transition</param>
        public void LoadSceneAsync(string sceneName, Color customFadeColor, float customFadeSpeed)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Statics.LogErrorSafe(Consts.ErrorMessages.InvalidSceneName);
                return;
            }

            try
            {
                // Use the Initiate class to start the fade transition
                Initiate.Fade(sceneName, customFadeColor, customFadeSpeed);
            }
            catch (Exception ex)
            {
                Statics.LogErrorSafe($"{Consts.ErrorMessages.UnexpectedError} loading scene with fade '{sceneName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Loads a scene asynchronously without fade transition, with progress display
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        public async void LoadSceneWithoutFade(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Statics.LogErrorSafe(Consts.ErrorMessages.InvalidSceneName);
                return;
            }

            try
            {
                if (!await EnsureSliderObjectExists())
                {
                    Statics.LogWarningSafe("LocalSceneLoader: Loading UI unavailable. Continuing scene load without progress slider.");
                }

                StartLoading();
                _loadHandle = Addressables.LoadSceneAsync(sceneName);
                await _loadHandle.Task;

                if (_loadHandle.Status == AsyncOperationStatus.Failed)
                {
                    Statics.LogErrorSafe($"{Consts.ErrorMessages.SceneLoadFailed}: {sceneName}");
                }
            }
            catch (OperationCanceledException)
            {
                Statics.LogWarningSafe($"{Consts.ErrorMessages.OperationCancelled}: {sceneName}");
            }
            catch (Exception ex)
            {
                Statics.LogErrorSafe($"{Consts.ErrorMessages.UnexpectedError} loading scene '{sceneName}': {ex.Message}");
            }
            finally
            {
                StopLoading();
            }
        }

        /// <summary>
        /// Loads asset resource locations asynchronously with progress display
        /// </summary>
        /// <param name="assets">Array of asset label references to load</param>
        /// <returns>True if loading succeeded, false otherwise</returns>
        public async Task<bool> LoadAssetsAsync(AssetLabelReference[] assets)
        {
            if (Statics.IsArrayNullOrEmpty(assets))
            {
                Statics.LogErrorSafe(Consts.ErrorMessages.InvalidAssetArray);
                return false;
            }

            try
            {
                if (!await EnsureSliderObjectExists())
                {
                    Statics.LogWarningSafe("LocalSceneLoader: Loading UI unavailable. Continuing asset loading without progress slider.");
                }

                StartLoading();
                _loadHandle = Addressables.LoadResourceLocationsAsync(assets);
                await _loadHandle.Task;

                if (_loadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    return true;
                }

                Statics.LogErrorSafe(Consts.ErrorMessages.ResourceLocationLoadFailed);
                return false;
            }
            catch (OperationCanceledException)
            {
                Statics.LogWarningSafe(Consts.ErrorMessages.AssetLoadCancelled);
                return false;
            }
            catch (Exception ex)
            {
                Statics.LogErrorSafe($"{Consts.ErrorMessages.UnexpectedError} loading assets: {ex.Message}");
                return false;
            }
            finally
            {
                StopLoading();
            }
        }

        /// <summary>
        /// Cancels the current loading operation if active
        /// </summary>
        public void CancelLoading()
        {
            if (_isLoading)
            {
                _cancellationTokenSource?.Cancel();
                StopLoading();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes cancellation token for operation management
        /// </summary>
        private void InitializeCancellationToken()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Ensures the loading slider UI exists, creating it if necessary
        /// </summary>
        /// <returns>True if slider is available, false if creation failed</returns>
        private async Task<bool> EnsureSliderObjectExists()
        {
            if (_sliderLoadingGameObject != null)
            {
                return true;
            }

            return await CreateSliderObject();
        }

        /// <summary>
        /// Creates and initializes the loading slider UI from addressables
        /// </summary>
        /// <returns>True if creation succeeded, false otherwise</returns>
        private async Task<bool> CreateSliderObject()
        {
            if (!useAddressableLoadingScreen)
            {
                return CreateFallbackSliderObject();
            }

            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(Consts.AddressableKeys.LoadingScreen);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
                {
                    _sliderLoadingGameObject = Instantiate(handle.Result);
                    _sliderLoading = _sliderLoadingGameObject.GetComponentInChildren<Slider>();

                    if (_sliderLoading == null)
                    {
                        Statics.LogErrorSafe(Consts.ErrorMessages.SliderComponentNotFound);
                        Statics.SafeDestroy(_sliderLoadingGameObject);
                        _sliderLoadingGameObject = null;
                        return false;
                    }

                    // Set initial state
                    _sliderLoadingGameObject.SetActive(false);
                    _sliderLoading.value = 0f;

                    return true;
                }

                Statics.LogErrorSafe(Consts.ErrorMessages.LoadingScreenNotFound);
                return CreateFallbackSliderObject();
            }
            catch (Exception ex)
            {
                Statics.LogErrorSafe($"{Consts.ErrorMessages.SliderCreationFailed}: {ex.Message}");
                return CreateFallbackSliderObject();
            }
        }

        /// <summary>
        /// Creates a minimal fallback loading UI if the Addressables loading screen is unavailable.
        /// </summary>
        /// <returns>True if fallback UI was created successfully, false otherwise.</returns>
        private bool CreateFallbackSliderObject()
        {
            try
            {
                _sliderLoadingGameObject = new GameObject("LoadingScreen_Fallback");

                var canvas = _sliderLoadingGameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10000;

                _sliderLoadingGameObject.AddComponent<CanvasScaler>();
                _sliderLoadingGameObject.AddComponent<GraphicRaycaster>();

                var sliderRoot = new GameObject("LoadingSlider");
                sliderRoot.transform.SetParent(_sliderLoadingGameObject.transform, false);
                var sliderRect = sliderRoot.AddComponent<RectTransform>();
                sliderRect.anchorMin = new Vector2(0.5f, 0.1f);
                sliderRect.anchorMax = new Vector2(0.5f, 0.1f);
                sliderRect.sizeDelta = new Vector2(500f, 40f);
                sliderRect.anchoredPosition = Vector2.zero;

                var bg = new GameObject("Background");
                bg.transform.SetParent(sliderRoot.transform, false);
                var bgRect = bg.AddComponent<RectTransform>();
                bgRect.anchorMin = Vector2.zero;
                bgRect.anchorMax = Vector2.one;
                bgRect.offsetMin = Vector2.zero;
                bgRect.offsetMax = Vector2.zero;
                var bgImage = bg.AddComponent<Image>();
                bgImage.color = new Color(0f, 0f, 0f, 0.6f);

                var fillArea = new GameObject("Fill Area");
                fillArea.transform.SetParent(sliderRoot.transform, false);
                var fillAreaRect = fillArea.AddComponent<RectTransform>();
                fillAreaRect.anchorMin = Vector2.zero;
                fillAreaRect.anchorMax = Vector2.one;
                fillAreaRect.offsetMin = new Vector2(10f, 10f);
                fillAreaRect.offsetMax = new Vector2(-10f, -10f);

                var fill = new GameObject("Fill");
                fill.transform.SetParent(fillArea.transform, false);
                var fillRect = fill.AddComponent<RectTransform>();
                fillRect.anchorMin = Vector2.zero;
                fillRect.anchorMax = Vector2.one;
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
                var fillImage = fill.AddComponent<Image>();
                fillImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);

                _sliderLoading = sliderRoot.AddComponent<Slider>();
                _sliderLoading.targetGraphic = fillImage;
                _sliderLoading.fillRect = fillRect;
                _sliderLoading.minValue = 0f;
                _sliderLoading.maxValue = 1f;
                _sliderLoading.value = 0f;
                _sliderLoading.direction = Slider.Direction.LeftToRight;

                _sliderLoadingGameObject.SetActive(false);

                Statics.LogWarningSafe("LocalSceneLoader: Using fallback loading slider UI.");
                return true;
            }
            catch (Exception ex)
            {
                Statics.LogErrorSafe($"LocalSceneLoader fallback slider creation failed: {ex.Message}");
                Statics.SafeDestroy(_sliderLoadingGameObject);
                _sliderLoadingGameObject = null;
                _sliderLoading = null;
                return false;
            }
        }

        /// <summary>
        /// Starts the loading process and shows UI
        /// </summary>
        [BurstCompile]
        private void StartLoading()
        {
            _isLoading = true;
            _loadProgress = 0f;

            if (_sliderLoadingGameObject != null)
            {
                _sliderLoadingGameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Stops the loading process and hides UI
        /// </summary>
        [BurstCompile]
        private void StopLoading()
        {
            _isLoading = false;
            _loadProgress = 0f;

            if (_sliderLoadingGameObject != null)
            {
                _sliderLoadingGameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the loading progress display
        /// </summary>
        [BurstCompile]
        private void UpdateLoadingProgress()
        {
            _loadProgress = _loadHandle.PercentComplete;

            if (_sliderLoading != null)
            {
                _sliderLoading.value = _loadProgress;
            }
        }

        /// <summary>
        /// Cleans up resources and cancels operations
        /// </summary>
        private void CleanupResources()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();

                if (_loadHandle.IsValid())
                {
                    Addressables.Release(_loadHandle);
                }

                Statics.SafeDestroy(_sliderLoadingGameObject);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error during LocalSceneLoader cleanup: {ex.Message}");
            }
        }

        #endregion
    }
}