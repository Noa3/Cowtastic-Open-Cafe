using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ExternFilesManager : MonoBehaviour
{
#if (UNITY_EDITOR || UNITY_WEBGL || UNITY_ANDROID)
    [Tooltip("This are the avatars for platforms which cant use StreamingAssets")]
    public List<CustomerAvatar> PlatformSpecificAvatars;
#endif

    [Tooltip("Will be used to be able, to add events to the Customers")]
    public List<EventBase> PlatformSpecificEvents = new List<EventBase> { null };

    private OrderManager orderManager;

    // Constants for file paths
    private static readonly string[] REQUIRED_PNG_FILES = { "/1.png", "/2.png", "/3.png", "/4.png", "/5.png" };
    private const string STAT_JSON_FILE = "/stat.json";
    private const string AVATAR_SEPARATOR = "Avatar ";

    public void Awake()
    {
        // Nur die Referenz setzen, aber noch nicht die Avatare laden
        orderManager = GetComponent<OrderManager>();
    }

    private void Start()
    {
        // Warten bis OrderManager vollständig initialisiert ist
        if (orderManager == null)
        {
            Debug.LogError("OrderManager component not found!");
            return;
        }

#if (UNITY_EDITOR || UNITY_WEBGL || UNITY_ANDROID)
        Debug.Log("Load Specific Avatars...");
        AddSpecificAvatars(PlatformSpecificAvatars);
#else
        Debug.Log("Load Asset Avatars...");
        AddStreamingAssetsAvatars();
#endif

        // Nach dem Laden der Avatare zerstören
        Destroy(this);
    }

    public void AddStreamingAssetsAvatars()
    {
        if (orderManager == null)
        {
            Debug.LogError("OrderManager is null!");
            return;
        }

        if (orderManager.RandomCustomAvatars == null)
        {
            Debug.LogError("RandomCustomAvatars list is null!");
            return;
        }

        string assetsPath = Application.streamingAssetsPath;
        if (!Directory.Exists(assetsPath))
        {
            Debug.LogWarning($"StreamingAssets path does not exist: {assetsPath}");
            return;
        }

        string[] directories = Directory.GetDirectories(assetsPath, "*", SearchOption.TopDirectoryOnly);
        Debug.Log($"Found {directories.Length} directories in StreamingAssets");

        foreach (string directory in directories)
        {
            string folderName = Path.GetFileName(directory);

            if (TryCreateAvatarFromDirectory(directory, folderName, out CustomerAvatar avatar))
            {
                orderManager.RandomCustomAvatars.Add(avatar);
                Debug.Log($"Successfully added avatar: {avatar.name}");
            }
            else
            {
                Debug.LogWarning($"Failed to create avatar from directory: {folderName}");
            }
        }

        Debug.Log($"Total avatars loaded: {orderManager.RandomCustomAvatars.Count}");
    }

    private bool TryCreateAvatarFromDirectory(string directoryPath, string folderName, out CustomerAvatar avatar)
    {
        avatar = null;

        // Check if all required PNG files exist
        if (!AllRequiredFilesExist(directoryPath))
        {
            return false;
        }

        avatar = CustomerAvatar.CreateInstance<CustomerAvatar>();
        avatar.name = AVATAR_SEPARATOR + folderName;

        // Load sprites
        LoadAvatarSprites(avatar, directoryPath);

        // Load or create stats
        LoadOrCreateAvatarStats(avatar, directoryPath);

        return true;
    }

    private bool AllRequiredFilesExist(string directoryPath)
    {
        return REQUIRED_PNG_FILES.All(fileName => File.Exists(directoryPath + fileName));
    }

    private void LoadAvatarSprites(CustomerAvatar avatar, string directoryPath)
    {
        avatar.Normal = LoadNewSprite(directoryPath + REQUIRED_PNG_FILES[0]);
        avatar.Level1 = LoadNewSprite(directoryPath + REQUIRED_PNG_FILES[1]);
        avatar.Level2 = LoadNewSprite(directoryPath + REQUIRED_PNG_FILES[2]);
        avatar.Level3 = LoadNewSprite(directoryPath + REQUIRED_PNG_FILES[3]);
        avatar.Level4 = LoadNewSprite(directoryPath + REQUIRED_PNG_FILES[4]);
    }

    private void LoadOrCreateAvatarStats(CustomerAvatar avatar, string directoryPath)
    {
        string statFilePath = directoryPath + STAT_JSON_FILE;

        if (File.Exists(statFilePath))
        {
            try
            {
                string contents = File.ReadAllText(statFilePath);
                AvatarStats stats = JsonUtility.FromJson<AvatarStats>(contents);
                avatar.Stats = stats;

                if (stats.CustomerEvent < PlatformSpecificEvents.Count)
                {
                    avatar.EventToActivate = PlatformSpecificEvents[stats.CustomerEvent];
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load stats from {statFilePath}: {ex.Message}");
                CreateDefaultStatsFile(statFilePath);
            }
        }
        else
        {
            CreateDefaultStatsFile(statFilePath);
        }
    }

    private void CreateDefaultStatsFile(string filePath)
    {
        var defaultStats = new AvatarStats();
        string jsonContent = JsonUtility.ToJson(defaultStats, true);

        try
        {
            File.WriteAllText(filePath, jsonContent);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to create default stats file at {filePath}: {ex.Message}");
        }
    }

    public void AddSpecificAvatars(List<CustomerAvatar> customers)
    {
        if (customers != null && orderManager != null)
        {
            orderManager.RandomCustomAvatars.AddRange(customers);
            Debug.Log($"Added {customers.Count} platform-specific avatars");
        }
    }

    /// <summary>
    /// Load a PNG or JPG file from disk to a Texture2D
    /// Returns null if load fails
    /// </summary>
    /// <param name="filePath">Path to the image file</param>
    /// <returns>Loaded texture or null if failed</returns>
    public static Texture2D LoadTexture(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"File not found: {filePath}");
            return null;
        }

        try
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);

            if (texture.LoadImage(fileData))
            {
                return texture;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load texture from {filePath}: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Converts a Texture2D to a sprite, assign this texture to a new sprite and return its reference
    /// </summary>
    /// <param name="texture">Source texture</param>
    /// <param name="pixelsPerUnit">Pixels per unit for the sprite</param>
    /// <param name="spriteType">Mesh type for the sprite</param>
    /// <returns>Created sprite or null if texture is null</returns>
    public static Sprite ConvertTextureToSprite(Texture2D texture, float pixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        if (texture == null)
        {
            return null;
        }

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), pixelsPerUnit, 0, spriteType);
    }

    /// <summary>
    /// Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
    /// </summary>
    /// <param name="filePath">Path to the image file</param>
    /// <param name="pixelsPerUnit">Pixels per unit for the sprite</param>
    /// <param name="spriteType">Mesh type for the sprite</param>
    /// <returns>Created sprite or null if loading failed</returns>
    public static Sprite LoadNewSprite(string filePath, float pixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        Texture2D spriteTexture = LoadTexture(filePath);
        return ConvertTextureToSprite(spriteTexture, pixelsPerUnit, spriteType);
    }
}
