using UnityEngine;


public class TriggerSound : MonoBehaviour
{
    public float CollisionVolume = 1f;
    public AudioClip abilitySound;
    //public AudioClip checkpoints;
    public AudioClip deadzones;

    public AudioClip room3;
    private AudioSource source1;
    private AudioSource source2;

    private AudioSource[] sources;

    private float targetAudio;

    public AudioClip wallSound;


    // Use this for initialization
    private void Start()
    {
        sources = GetComponents<AudioSource>();
        source1 = sources[0];
        source2 = sources[1];

        source2.loop = true;
    }


    // Update is called once per frame
    private void Update()
    {
    }


    private void OnTriggerEnter(Collider col)
    {
        //if (col.gameObject.CompareTag("Wall")){
        //	source1.PlayOneShot (wallSound);
        //}

        if (col.gameObject.CompareTag("Particle system")) source1.PlayOneShot(abilitySound);

        //if (col.gameObject.CompareTag("Checkpoint")){
        //	source1.PlayOneShot (checkpoints);
        //}

        if (col.gameObject.CompareTag("Deadzones sound")) source1.PlayOneShot(deadzones);

        if (col.gameObject.CompareTag("Room3"))
        {
            targetAudio = 1;
            source2.clip = room3;
            source2.Play();
        }
    }


    private void OnTriggerExit(Collider col)
    {
        targetAudio = 0;

        if (col.gameObject.CompareTag("Room3") && source1.isPlaying) source2.Stop();
    }


    public void PlayCollisionSound(float volume)
    {
        source1.PlayOneShot(wallSound, volume * CollisionVolume);
    }
}