using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour {

	private AudioSource[] sources;
	private AudioSource source1;

	public AudioClip wallSound;	//when colliding with a wall 
	public AudioClip playerSound;

	// Use this for initialization
	void Start () {
		sources = GetComponents<AudioSource> ();
		source1 = sources[0];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col){
		Debug.Log ("Collision");
		if (col.gameObject.CompareTag("Wall")){
			Debug.Log ("Wall");
			source1.PlayOneShot (wallSound);

		}
	}
}