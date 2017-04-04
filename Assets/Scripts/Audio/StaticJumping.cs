using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticJumping : MonoBehaviour {

	private AudioSource source;
	public AudioClip[] clip;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayStaticJumpSound(){

		int range = Random.Range (1, clip.Length);
		source.clip = clip [range];
		source.PlayOneShot (source.clip);
		clip [range] = clip [0];
		clip [0] = source.clip;
	}

	public void PlayStaticDashSound(){

		int range = Random.Range (1, clip.Length);
		source.clip = clip [range];
		source.PlayOneShot (source.clip);
		clip [range] = clip [0];
		clip [0] = source.clip;
	}

	public void PlayStaticPushSound(){

		int range = Random.Range (1, clip.Length);
		source.clip = clip [range];
		source.PlayOneShot (source.clip);
		clip [range] = clip [0];
		clip [0] = source.clip;
	}

	public void PlayStaticHitSound(){

		int range = Random.Range (1, clip.Length);
		source.clip = clip [range];
		source.PlayOneShot (source.clip);
		clip [range] = clip [0];
		clip [0] = source.clip;
	}

	public void PlayMovingAroundSound(){

		int range = Random.Range (1, clip.Length);
		source.clip = clip [range];
		source.PlayOneShot (source.clip);
		clip [range] = clip [0];
		clip [0] = source.clip;
	}
}
