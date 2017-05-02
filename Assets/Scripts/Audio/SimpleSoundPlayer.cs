using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimpleSoundPlayer : MonoBehaviour
{

    public float MinPitch = 1f;
    public float MaxPitch = 1f;
    public float MinVolume = 1f;
    public float MaxVolume = 1f;

    public void PlaySound()
    {
        GetComponent<AudioSource>().pitch = Random.Range(MinPitch, MaxPitch);
        GetComponent<AudioSource>().volume = Random.Range(MinVolume, MaxVolume);
        GetComponent<AudioSource>().Play();
    }
}
