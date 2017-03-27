using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour {

	private AudioSource[] sources;
	private AudioSource source1;
//	private AudioSource source2;	till spelarens fotstegljud


	public AudioClip wallSound;	//when colliding with a wall 
	public AudioClip playerSound;
	public AudioClip hitSound;
	public AudioClip abilitySound;
	public AudioClip room1;
	public AudioClip room2;
	public AudioClip checkpoints;

	private int hits = 0;


	// Use this for initialization
	void Start () {

		sources = GetComponents<AudioSource> ();
		source1 = sources[0];
		//source2 = sources [1];	sätts till spelarens fotsteg
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
			

		if (col.gameObject.tag == "Player Trigger"){
			source1.PlayOneShot (playerSound);
		}

		if (col.gameObject.CompareTag("Checkpoint")){
			source1.PlayOneShot (checkpoints);
		}
			
	}

	void OnTriggerExit(Collider col){
		if (col.gameObject.tag == "Room" && source1.isPlaying){
			source1.Stop ();
		}

		if (col.gameObject.tag == "Room2" && source1.isPlaying){
			source1.Stop ();
		}
	}
}