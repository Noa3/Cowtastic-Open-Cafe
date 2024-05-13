using UnityEngine;

public class ArchievementsHelper : MonoBehaviour
{
    public void UnlockArchievement(Archievements.ArchievementID id)
    {
        Archievements.UnlockArchievement(id);
    }

    public void UnlockArchievement(int id)
    {
        Archievements.UnlockArchievement( (Archievements.ArchievementID)id );
    }
}
