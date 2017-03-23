using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour {

	private AudioSource[] sources;
	private AudioSource source1;
	//private AudioSource source2;

	public AudioClip wallSound;	//when colliding with a wall 
	//public AudioClip playerSound;
	public AudioClip moSound;
	public AudioClip hitSound;
	public AudioClip abilitySound;
	public AudioClip differentRoomSounds;

	private float volLowRange = 0.5f;
	private float volHighRange = 1.0f;

	private int hits = 0;
	private int count = 1;
	private float volume = 0.5f;


	// Use this for initialization
	void Start () {

		sources = GetComponents<AudioSource> ();
		source1 = sources[0];

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider col){
	
		if (col.gameObject.CompareTag("Wall")){
			Debug.Log ("Wall");
			hits++;
			Debug.Log ("Collision hit: " + hits);
			source1.PlayOneShot (wallSound);
		}

		if (col.gameObject.CompareTag("Movable Object")){
			source1.PlayOneShot(moSound);
		}

		if (col.gameObject.CompareTag("Hit Object")){
			source1.PlayOneShot (hitSound);
		}

		if (col.gameObject.CompareTag("Particle system")){
			source1.PlayOneShot (abilitySound);
			//col.gameObject.SetActive (false);	//deactive particle sytem Hit when colliding with the player	

		}

		if (col.gameObject.CompareTag ("Room")) {
			source1.PlayOneShot (differentRoomSounds);

		}
			
	}

	void OnTriggerExit(Collider col){
		if (col.gameObject.tag == "Room" && source1.isPlaying){
			
			//float volume = Random.Range (volLowRange, volHighRange);
			//source1.PlayOneShot(differentRoomSounds, volume);

			//source1.pitch = 0.5f;
			source1.Stop ();
		}
	}



}