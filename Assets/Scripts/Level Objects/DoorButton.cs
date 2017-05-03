using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorButton : MonoBehaviour
{
    public GameObject[] Doors;
    public AudioClip ActivateSound, DeactivateSound;
    public float ActivateVolume = 1f;
    public float DeactivateVolume = 1f;

    private int count;

	// Use this for initialization
	void Start ()
	{
	    foreach (var door in Doors)
	    {
	        door.SetActive(true);
        }
        
	    count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Movable Object") || other.CompareTag("Player"))
        {
            if (++count > 0)
            {
                var source = GetComponent<AudioSource>();
                source.clip = ActivateSound;
                source.volume = ActivateVolume;
                source.Play();
                foreach (var door in Doors)
                {
                    door.SetActive(false);
                }
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Movable Object") || other.CompareTag("Player"))
        {
            if (--count <= 0)
            {
                var source = GetComponent<AudioSource>();
                source.clip = DeactivateSound;
                source.volume = DeactivateVolume;
                source.Play();
                foreach (var door in Doors)
                {
                    door.SetActive(true);
                }
            }
        }
    }
}
