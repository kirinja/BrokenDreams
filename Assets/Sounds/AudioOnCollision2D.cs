using UnityEngine;
using System.Collections;

public class AudioOnCollision2D : MonoBehaviour {

	private AudioSource source;
	public AudioClip wall_hit_sound;
	public AudioClip movable_object_sound;
	public AudioClip push_ability_sound;

	void Start(){
		source = GetComponent<AudioSource> ();
	}
		
	void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.CompareTag("Wall")){
			source.PlayOneShot(wall_hit_sound);
		}

		if (collision.gameObject.CompareTag("Movable Object")){
			source.PlayOneShot (movable_object_sound);
		}

		if (collision.gameObject.CompareTag("Push Ability")){
			collision.gameObject.SetActive (false);
			source.PlayOneShot (push_ability_sound);
		}
	}

/*	void OnCollisionStay(Collision col){

		if (!source.isPlaying && col.relativeVelocity.magnitude >= 2){
			source.volume = col.relativeVelocity.magnitude / 20;
			Debug.Log (col.relativeVelocity.magnitude);
			source.PlayOneShot (clip);
		}
	}*/
}
