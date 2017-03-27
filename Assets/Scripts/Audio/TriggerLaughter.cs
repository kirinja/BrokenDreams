using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLaughter : MonoBehaviour {

	private AudioSource source;
	public AudioClip laughter1;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource> ();
		source.clip = laughter1;
		//source.loop = true;

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter (Collider col){
		
		if (col.gameObject.CompareTag ("Room3")) {
			source.PlayOneShot (laughter1);
		}
	}
}