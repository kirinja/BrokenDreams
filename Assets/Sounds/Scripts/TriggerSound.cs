using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour {

	private AudioSource[] sources;
	private AudioSource source1;
//	private AudioSource source2;


	public AudioClip wallSound;	//when colliding with a wall 
	public AudioClip playerSound;
	public AudioClip hitSound;
	public AudioClip abilitySound;
	public AudioClip room1;
	public AudioClip room2;
	public AudioClip room3;

	private int hits = 0;


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
			//Debug.Log ("Wall");
			hits++;
			//Debug.Log ("Collision hit: " + hits);
			source1.PlayOneShot (wallSound);
		}

		/*if (col.gameObject.CompareTag("Hit Object")){
			source1.PlayOneShot (hitSound);
		}*/

		if (col.gameObject.CompareTag("Particle system")){
			source1.PlayOneShot (abilitySound);
		}

		if (col.gameObject.CompareTag ("Room")) {
			source1.PlayOneShot (room1);

		}

		if (col.gameObject.CompareTag("Room2")){
			source1.PlayOneShot (room2);
		}

		if (col.gameObject.CompareTag("Room3")){
			source1.PlayOneShot (room3);
		}

		if (col.gameObject.tag == "Player Trigger"){
			source1.PlayOneShot (playerSound);
		}
			
	}

	void OnTriggerExit(Collider col){
		if (col.gameObject.tag == "Room" && source1.isPlaying){
			source1.Stop ();
		}

		if (col.gameObject.tag == "Room2" && source1.isPlaying){
			source1.Stop ();
		}

		if (col.gameObject.tag == "Room3" && source1.isPlaying){
			source1.Stop ();
		}
	}



}