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
	//public AudioClip abilitySound;

	private int hits = 0;
	//private float pVel;

	// Use this for initialization
	void Start () {
		sources = GetComponents<AudioSource> ();
		source1 = sources[0];
		/*source2 = sources[1];	//playersound = source2
		source2.clip = playerSound;
		source2.loop = true;*/

	}
	
	// Update is called once per frame
	void Update () {
		/*if (pVel < 0.03f) {
			
			source2.Pause ();

		} else {
			
			if (!source2.isPlaying){
				source2.PlayOneShot (playerSound);
			}
		}*/
	}

	/*void OnCollisionEnter(Collision col){
		Debug.Log ("Collision");
		if (col.gameObject.CompareTag("Wall")){
			Debug.Log ("Wall");
			source1.PlayOneShot (wallSound);

		}
	}*/

	void OnTriggerEnter(Collider col){
	
		if (col.gameObject.CompareTag("Wall")){
			//Debug.Log ("Wall");
			hits++;
			//Debug.Log ("Collision hit: " + hits);
			source1.PlayOneShot (wallSound);
		}

		if (col.gameObject.CompareTag("Movable Object")){
			source1.PlayOneShot(moSound);
		}

		if (col.gameObject.CompareTag("Hit Object")){
			source1.PlayOneShot (hitSound);
		}


		/*if (col.gameObject.CompareTag("Ability")){
			source1.PlayOneShot (abilitySound);
		}*/


	}

	void FixedUpdate(){
		//pVel = GetComponentInChildren<Rigidbody> ().velocity.magnitude;
	}
	/*
	void OnControllerColliderHit(ControllerColliderHit hit) {
		
		if (hit.gameObject.CompareTag("Wall")){
			Debug.Log ("Wall");
			hits++;
			Debug.Log ("Collision hit: " + hits);
			source1.PlayOneShot (wallSound);
		}

	}*/
}