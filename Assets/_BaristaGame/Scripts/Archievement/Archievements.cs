using UnityEngine;

public static class Archievements
{
    public static void UnlockArchievement(ArchievementID id)
    {
        PlayerPrefs.SetString(Consts.PlayerPrefPrefix + id.ToString(), true.ToString());
    }

    public enum ArchievementID
    {
        Familiar_Faces = 0,
        One_OfEverything = 1,
        Full_Wardrobe = 2,
        Chaos_Difficulty = 3,
        Sandbox_Mode = 4,
        Ultra_Chaos = 5,
        Make_Waves = 6,
        Supersized = 7,
        Holiday = 8,
        SoldCups1000 = 9,
        Play10h = 10,
        ProduceMilk100 = 11,
        EarnedMoney10000 = 12
    }
}


