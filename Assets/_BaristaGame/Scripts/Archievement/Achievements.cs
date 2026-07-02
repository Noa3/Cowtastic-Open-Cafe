using UnityEngine;

public static class Achievements
{
    public static void UnlockAchievements(AchievementsID id)
    {
        PlayerPrefs.SetString(Consts.PlayerPrefPrefix + id.ToString(), true.ToString());

        // Debug-Ausgabe für Achievement-Freischaltung
        Debug.Log($"Achievement unlocked: {id}");

        // Speichern um Datenverlust zu vermeiden
        PlayerPrefs.Save();
    }

    public static bool IsAchievementUnlocked(AchievementsID id)
    {
        return bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefPrefix + id.ToString(), false.ToString()));
    }

    public static void CheckStatBasedAchievements()
    {
        // Cups Sold Achievements
        int cupsSold = PlayerPrefs.GetInt(Consts.PlayerPrefCupsSoldOverall, 0);
        if (cupsSold >= 1000 && !IsAchievementUnlocked(AchievementsID.SoldCups1000))
            UnlockAchievements(AchievementsID.SoldCups1000);
        if (cupsSold >= 5000 && !IsAchievementUnlocked(AchievementsID.SoldCups5000))
            UnlockAchievements(AchievementsID.SoldCups5000);
        if (cupsSold >= 10000 && !IsAchievementUnlocked(AchievementsID.SoldCups10000))
            UnlockAchievements(AchievementsID.SoldCups10000);

        // Money Earned Achievements
        float moneyEarned = PlayerPrefs.GetFloat(Consts.PlayerPrefMoneyEarnedOverall, 0);
        if (moneyEarned >= 10000 && !IsAchievementUnlocked(AchievementsID.EarnedMoney10000))
            UnlockAchievements(AchievementsID.EarnedMoney10000);
        if (moneyEarned >= 50000 && !IsAchievementUnlocked(AchievementsID.EarnedMoney50000))
            UnlockAchievements(AchievementsID.EarnedMoney50000);
        if (moneyEarned >= 100000 && !IsAchievementUnlocked(AchievementsID.EarnedMoney100000))
            UnlockAchievements(AchievementsID.EarnedMoney100000);

        // Milk Production Achievements
        double milkProduced = double.Parse(PlayerPrefs.GetString(Consts.PlayerPrefMilkProducedOverall, "0"));
        if (milkProduced >= 100 && !IsAchievementUnlocked(AchievementsID.ProduceMilk100))
            UnlockAchievements(AchievementsID.ProduceMilk100);
        if (milkProduced >= 500 && !IsAchievementUnlocked(AchievementsID.ProduceMilk500))
            UnlockAchievements(AchievementsID.ProduceMilk500);
        if (milkProduced >= 1000 && !IsAchievementUnlocked(AchievementsID.ProduceMilk1000))
            UnlockAchievements(AchievementsID.ProduceMilk1000);

        // Playtime Achievements
        double playtime = double.Parse(PlayerPrefs.GetString(Consts.PlayerPrefTimePlayedOverall, "0"));
        double playtimeHours = playtime / 3600; // Convert seconds to hours
        if (playtimeHours >= 10 && !IsAchievementUnlocked(AchievementsID.Play10h))
            UnlockAchievements(AchievementsID.Play10h);
        if (playtimeHours >= 25 && !IsAchievementUnlocked(AchievementsID.Play25h))
            UnlockAchievements(AchievementsID.Play25h);
        if (playtimeHours >= 50 && !IsAchievementUnlocked(AchievementsID.Play50h))
            UnlockAchievements(AchievementsID.Play50h);

        // Customer Achievements
        int customers = PlayerPrefs.GetInt(Consts.PlayerPrefCustomersOverall, 0);
        if (customers >= 500 && !IsAchievementUnlocked(AchievementsID.ServedCustomers500))
            UnlockAchievements(AchievementsID.ServedCustomers500);
        if (customers >= 1000 && !IsAchievementUnlocked(AchievementsID.ServedCustomers1000))
            UnlockAchievements(AchievementsID.ServedCustomers1000);
        if (customers >= 2500 && !IsAchievementUnlocked(AchievementsID.ServedCustomers2500))
            UnlockAchievements(AchievementsID.ServedCustomers2500);
    }

    public static void CheckDifficultyAchievements()
    {
        // Check if all difficulty modes are completed
        bool casualWon = PlayerPrefs.GetInt(Consts.PlayerPrefSceneWon + "Casual", 0) == 1;
        bool normalWon = PlayerPrefs.GetInt(Consts.PlayerPrefSceneWon + "Normal", 0) == 1;
        bool hardWon = PlayerPrefs.GetInt(Consts.PlayerPrefSceneWon + "Hard", 0) == 1;
        bool chaosWon = PlayerPrefs.GetInt(Consts.PlayerPrefSceneWon + "Chaos", 0) == 1;
        bool ultraChaosWon = PlayerPrefs.GetInt(Consts.PlayerPrefSceneWon + "UltraChaos", 0) == 1;

        if (casualWon && !IsAchievementUnlocked(AchievementsID.Casual_Master))
            UnlockAchievements(AchievementsID.Casual_Master);
        if (normalWon && !IsAchievementUnlocked(AchievementsID.Normal_Master))
            UnlockAchievements(AchievementsID.Normal_Master);
        if (hardWon && !IsAchievementUnlocked(AchievementsID.Hard_Master))
            UnlockAchievements(AchievementsID.Hard_Master);
        if (chaosWon && !IsAchievementUnlocked(AchievementsID.Chaos_Difficulty))
            UnlockAchievements(AchievementsID.Chaos_Difficulty);
        if (ultraChaosWon && !IsAchievementUnlocked(AchievementsID.Ultra_Chaos))
            UnlockAchievements(AchievementsID.Ultra_Chaos);

        // All difficulties completed
        if (casualWon && normalWon && hardWon && chaosWon && !IsAchievementUnlocked(AchievementsID.Complete_Master))
            UnlockAchievements(AchievementsID.Complete_Master);
    }

    public static void CheckSpeedAchievements()
    {
        // Best time achievements for different difficulties
        float bestTimeNormal = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeNormal, -1);
        float bestTimeHard = PlayerPrefs.GetFloat(Consts.PlayerPrefBestTimeHard, -1);

        if (bestTimeNormal > 0 && bestTimeNormal <= 300 && !IsAchievementUnlocked(AchievementsID.Speed_Runner_Normal)) // 5 minutes
            UnlockAchievements(AchievementsID.Speed_Runner_Normal);
        if (bestTimeHard > 0 && bestTimeHard <= 600 && !IsAchievementUnlocked(AchievementsID.Speed_Runner_Hard)) // 10 minutes
            UnlockAchievements(AchievementsID.Speed_Runner_Hard);
    }

    public enum AchievementsID
    {
        // Original achievements
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
        EarnedMoney10000 = 12,

        // New progressive achievements
        SoldCups5000 = 13,
        SoldCups10000 = 14,
        Play25h = 15,
        Play50h = 16,
        ProduceMilk500 = 17,
        ProduceMilk1000 = 18,
        EarnedMoney50000 = 19,
        EarnedMoney100000 = 20,

        // Customer service achievements
        ServedCustomers500 = 21,
        ServedCustomers1000 = 22,
        ServedCustomers2500 = 23,

        // Difficulty completion achievements
        Casual_Master = 24,
        Normal_Master = 25,
        Hard_Master = 26,
        Complete_Master = 27, // Complete all difficulties

        // Speed achievements
        Speed_Runner_Normal = 28, // Complete Normal mode in under 5 minutes
        Speed_Runner_Hard = 29,   // Complete Hard mode in under 10 minutes

        // Special achievements
        Perfect_Day = 30,         // Complete a level with 100% happiness
        Efficiency_Expert = 31,   // Serve 50 customers without making a mistake
        Milk_Connoisseur = 32,    // Use all milk types in a single session
        Perfectionist = 33,       // Get perfect scores on 10 orders in a row
        Early_Bird = 34,          // Play the game for 7 consecutive days
        Night_Owl = 35,           // Play between 12 AM and 6 AM
        Weekend_Warrior = 36,     // Play on both Saturday and Sunday

        // Avatar achievements
        Avatar_Collector = 37,    // Meet all avatar types
        Fashion_Forward = 38,     // Change outfits 100 times
        Style_Icon = 39,          // Unlock all clothing combinations

        // Seasonal/Special events
        Halloween_Special = 40,   // Play during Halloween period
        Christmas_Special = 41,   // Play during Christmas period
        New_Year_Special = 42,    // Play during New Year period

        // Advanced gameplay
        Big_Spender = 44,         // Spend 10000 on upgrades
        Upgrade_Master = 45,      // Max out all upgrades
        Resource_Manager = 46,    // Never run out of ingredients in a level
        Customer_Favorite = 47,   // Get tipped by 100 different customers
        Barista_Legend = 48       // Ultimate achievement - unlock everything else
    }
}