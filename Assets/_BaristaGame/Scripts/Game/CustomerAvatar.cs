using UnityEngine;

[CreateAssetMenu(fileName = "CustomAvatar", menuName = "CustomAvatar")]
public class CustomerAvatar : ScriptableObject
{
    public AvatarStats Stats = new AvatarStats();

    public EventBase EventToActivate;

    [Space(20)]

    public Sprite Normal;
    public Sprite Level1;
    public Sprite Level2;
    public Sprite Level3;
    public Sprite Level4;
}

[System.Serializable]
public class AvatarStats
{
    /// <summary>
    /// How big is the chance to apear, the higher the more the chance Default: 1000
    /// </summary>
    [Min(0)]
    public int Weighted = 1000;
    [Min(0)]
    public float FlusteredMultipler = 1.00f;
    public bool RemoveEventIfCustomerIsFinished = true;

    [ReadOnly][Min(0)]
    public byte CustomerEvent = 0;
}
