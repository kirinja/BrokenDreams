using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class PortalAudio : MonoBehaviour
{
    public AudioClip Clip;
    public bool Loop = true;
    private AudioSource _source;

	// Use this for initialization
	void Start ()
	{
	    _source = GetComponent<AudioSource>();
	    _source.clip = Clip;
	    _source.loop = Loop;
	    _source.time = Random.Range(0, Clip.length);
        _source.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
