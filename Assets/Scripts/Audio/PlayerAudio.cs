using UnityEngine;


public class PlayerAudio : MonoBehaviour
{
    public float CollisionVolume = 1f;
    public float AbilityVolume = 1f;
    public float PickupVolume = 1f;
    public float DeathZoneVolume = 1f;
    public float DeathVolume = 1f;
    public AudioClip WallSound;
    public AudioClip[] AbilityUseSounds;
    public AudioClip PickupSound;
    public AudioClip DeathZoneSound;
    public AudioClip DeathSound;

    //public AudioClip Room3;

    private AudioSource _effectSource;
    private AudioSource _ambientSource;


    // Use this for initialization
    private void Start()
    {
        var sources = GetComponents<AudioSource>();
        _effectSource = sources[0];
        _ambientSource = sources[1];

        _ambientSource.loop = true;
    }


    // Update is called once per frame
    private void Update()
    {
    }


    private void OnTriggerEnter(Collider col)
    {
        /*if (col.gameObject.CompareTag("Room3"))
        {
            _ambientSource.clip = Room3;
            _ambientSource.Play();
        }*/
    }


    private void OnTriggerExit(Collider col)
    {
        //if (col.gameObject.CompareTag("Room3") && _ambientSource.isPlaying) _ambientSource.Stop();
    }


    public void PlayCollisionSound(float volume)
    {
        _effectSource.PlayOneShot(WallSound, volume * CollisionVolume);
    }


    public void PlayAbilitySound()
    {
        int range = Random.Range(1, AbilityUseSounds.Length);
        var temp = AbilityUseSounds[range];
        _effectSource.PlayOneShot(temp, AbilityVolume);
        AbilityUseSounds[range] = AbilityUseSounds[0];
        AbilityUseSounds[0] = temp;
    }


    public void PlayPickupSound()
    {
        _effectSource.PlayOneShot(PickupSound, PickupVolume);
    }


    public void PlayDeathZoneSound()
    {
        _effectSource.PlayOneShot(DeathZoneSound, DeathZoneVolume);
    }


    public void PlayDeathSound()
    {
        _effectSource.PlayOneShot(DeathSound, DeathVolume);
    }
}