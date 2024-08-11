using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class ExternFilesManager : MonoBehaviour
{
#if (UNITY_EDITOR || UNITY_WEBGL || UNITY_ANDROID) //UNITY_EDITOR || 
    [Tooltip("This are the avatars for platforms which cant use StreamingAssets")]
    public List<CustomerAvatar> PlatformSpecificAvatars;
#endif


    [Tooltip("Will be used to be able, to add events to the Customers")]
    public List<EventBase> PlatformSpecificEvents = new List<EventBase>() { null };

    private OrderManager orderManager;

    string AssetsPath = Application.streamingAssetsPath;


    private static string stringPng1 = "/1.png";
    private static string stringPng2 = "/2.png";
    private static string stringPng3 = "/3.png";
    private static string stringPng4 = "/4.png";
    private static string stringPng5 = "/5.png";

    private static string stringStatJson = "/stat.json";

    public void Awake()
    {
        orderManager = GetComponent<OrderManager>();

#if (UNITY_EDITOR || UNITY_WEBGL || UNITY_ANDROID) //UNITY_EDITOR || 
        Debug.Log("Load Specific Avatars...");
        AddSpecificAvatars(PlatformSpecificAvatars);
#else
        Debug.Log("Load Asset Avatars...");
        AddStreamingAssetsAvatars();
#endif

        Destroy(this);
    }

    private const string AvatarSeperator = "Avatar ";
    public void AddStreamingAssetsAvatars()
    {
        if (orderManager != null)
        {
            string[] dirs = Directory.GetDirectories(AssetsPath, "*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < dirs.Length; i++)
            {
                //NotWorking
                //DirectoryInfo FolderInfo = new DirectoryInfo(Path.GetDirectoryName(dirs[i]));
                //string Foldername = FolderInfo.Name;

                string[] trimmed = dirs[i].Split("\\");
                string Foldername = trimmed[trimmed.Length - 1];

                CustomerAvatar avatar;
                if (File.Exists(dirs[i] + stringPng1) == true && File.Exists(dirs[i] + stringPng2) == true && File.Exists(dirs[i] + stringPng3) == true && File.Exists(dirs[i] + stringPng4) == true && File.Exists(dirs[i] + stringPng5) == true)
                {
                    avatar = CustomerAvatar.CreateInstance<CustomerAvatar>();
                    avatar.name = AvatarSeperator + Foldername;

                    avatar.Normal = LoadNewSprite(dirs[i] + stringPng1);
                    avatar.Level1 = LoadNewSprite(dirs[i] + stringPng2);
                    avatar.Level2 = LoadNewSprite(dirs[i] + stringPng3);
                    avatar.Level3 = LoadNewSprite(dirs[i] + stringPng4);
                    avatar.Level4 = LoadNewSprite(dirs[i] + stringPng5);

                    if (File.Exists(dirs[i] + stringStatJson))
                    {
                        string contents = File.ReadAllText((dirs[i] + stringStatJson));
                        AvatarStats stats = new AvatarStats();
                        stats = JsonUtility.FromJson<AvatarStats>(contents);

                        //avatar.Stats.Weighted = stats.Weighted;
                        //avatar.Stats.FlusteredMultipler = stats.FlusteredMultipler;
                        avatar.Stats = stats;

                        avatar.EventToActivate = PlatformSpecificEvents[stats.CustomerEvent];
                    }
                    else
                    {

                        AvatarStats stats = new AvatarStats();
                        string contents = JsonUtility.ToJson(stats, true);
                        File.WriteAllText(dirs[i] + stringStatJson, contents);
                    }

                    orderManager.RandomCustomAvatars.Add(avatar);
                }


            }

        }

        orderManager.RandomCustomAvatars = orderManager.RandomCustomAvatars.OrderBy(x => x.name).ToList();
    }

    public void AddSpecificAvatars(List<CustomerAvatar> customers)
    {
        orderManager.RandomCustomAvatars.AddRange(customers);
        orderManager.RandomCustomAvatars = orderManager.RandomCustomAvatars.OrderBy(x => x.name).ToList();
    }

    /// <summary>
    /// Load a PNG or JPG file from disk to a Texture2D
    /// Returns null if load fails
    /// </summary>
    /// <param name="FilePath"></param>
    /// <returns></returns>
    public static Texture2D LoadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }

    /// <summary>
    /// Converts a Texture2D to a sprite, assign this texture to a new sprite and return its reference
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="PixelsPerUnit"></param>
    /// <param name="spriteType"></param>
    /// <returns></returns>
    public static Sprite ConvertTextureToSprite(Texture2D texture, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        Sprite NewSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

        return NewSprite;
    }

    /// <summary>
    /// Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
    /// </summary>
    /// <param name="FilePath"></param>
    /// <param name="PixelsPerUnit"></param>
    /// <param name="spriteType"></param>
    /// <returns></returns>
    public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        Texture2D SpriteTexture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

        return NewSprite;
    }
}
