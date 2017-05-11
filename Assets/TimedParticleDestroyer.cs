using UnityEngine;

public class TimedParticleDestroyer : MonoBehaviour
{
	void Update ()
    {
        if (!GetComponent<ParticleSystem>().isPlaying)
        {
            Destroy(gameObject);
        }
	}
}
