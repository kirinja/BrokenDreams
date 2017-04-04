using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour {

	private AudioSource[] sources;
	private AudioSource source1;
	private AudioSource source2;

	public AudioClip wallSound;	
	public AudioClip abilitySound;
	public AudioClip checkpoints;
	public AudioClip deadzones;

	public AudioClip room0;
	public AudioClip room1;
	public AudioClip room2;
	public AudioClip room3;

	private float targetAudio = 0; 
	// Use this for initialization
	void Start () {

		sources = GetComponents<AudioSource> ();
		source1 = sources[0];
		source2 = sources[1];

		source2.loop = true;

	}
	
	// Update is called once per frame
	void Update () {

	}
		
	void OnTriggerEnter(Collider col){

		if (col.gameObject.CompareTag("Wall")){
			source1.PlayOneShot (wallSound);
		}

		if (col.gameObject.CompareTag("Particle system")){
			source1.PlayOneShot (abilitySound);
		}

		if (col.gameObject.CompareTag("Checkpoint")){
			source1.PlayOneShot (checkpoints);
		}

		if (col.gameObject.CompareTag("Deadzones sound")){
			source1.PlayOneShot (deadzones);
		}

		if (col.gameObject.CompareTag("Room0")){
			targetAudio = 1;
			source2.clip = room0;
			source2.Play ();
		}

		if (col.gameObject.CompareTag ("Room")) {
			targetAudio = 1;
			source2.clip = room1;
			source2.Play ();
		}

		if (col.gameObject.CompareTag("Room2")){
			targetAudio = 1;
			source2.clip = room2;
			source2.Play ();
		}

		if (col.gameObject.CompareTag("Room3")){
			targetAudio = 1;
			source2.clip = room3;
			source2.Play ();
		}
	}

	void OnTriggerExit(Collider col){
		targetAudio = 0;

		if (col.gameObject.tag == "Room0" && source1.isPlaying){
			source2.Stop ();
		}

		if (col.gameObject.tag == "Room" && source1.isPlaying){
			source2.Stop ();
		}

		if (col.gameObject.tag == "Room2" && source1.isPlaying){
			source2.Stop ();
		}

		if (col.gameObject.tag == "Room3" && source1.isPlaying){
			source2.Stop ();
		}
	}
}