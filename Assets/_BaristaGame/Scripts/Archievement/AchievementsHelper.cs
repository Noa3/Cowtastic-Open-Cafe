using UnityEngine;

public class AchievementsHelper : MonoBehaviour
{
    public void UnlockAchievements(Achievements.AchievementsID id)
    {
        Achievements.UnlockAchievements(id);
    }

    public void UnlockArchievement(int id)
    {
        Achievements.UnlockAchievements((Achievements.AchievementsID)id);
    }

    // Neue Methoden für häufige Achievement Checks
    public void CheckTimeBasedAchievements()
    {
        CheckEarlyBirdAchievement();
        CheckNightOwlAchievement();
        //CheckWeekendWarriorAchievement();
    }

    private void CheckEarlyBirdAchievement()
    {
        System.DateTime now = System.DateTime.Now;
        if (now.Hour >= 6 && now.Hour <= 10)
        {
            // Track early morning play sessions
        }
    }

    private void CheckNightOwlAchievement()
    {
        System.DateTime now = System.DateTime.Now;
        if (now.Hour >= 0 && now.Hour <= 6)
        {
            Achievements.UnlockAchievements(Achievements.AchievementsID.Night_Owl);
        }
    }

    private void CheckSeasonalAchievements()
    {
        System.DateTime now = System.DateTime.Now;

        // Halloween Achievement
        if (now.Month == 10 && now.Day == 31)
            Achievements.UnlockAchievements(Achievements.AchievementsID.Halloween_Special);

        // Christmas Achievement  
        if (now.Month == 12 && (now.Day >= 24 && now.Day <= 26))
            Achievements.UnlockAchievements(Achievements.AchievementsID.Christmas_Special);
    }
}