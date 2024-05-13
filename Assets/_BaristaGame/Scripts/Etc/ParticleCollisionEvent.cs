using UnityEngine;
using UnityEngine.Events;

public class ParticleCollisionEvent : MonoBehaviour
{
    public float SoundChance = 0.5f;

    public UnityEvent OnParticleCollisionEvent;

    private void OnParticleCollision(GameObject other)
    {
        if (SoundChance >= Random.value)
        {
            OnParticleCollisionEvent.Invoke();
        }
    }
}
