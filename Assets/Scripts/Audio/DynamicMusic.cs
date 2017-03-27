using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMusic : MonoBehaviour {

	public float bmp = 153.0f;
	public int numBeatsPerSegments = 8;
	public AudioClip[] clips = new AudioClip[2];
	private int flip = 0;
	private double nextEventTime;
	private AudioSource[] audioSources = new AudioSource[2];
	private bool running = true;

	// Use this for initialization
	void Start () {
		int i = 0;
		while (i < 2){
			GameObject child = new GameObject ("Player");
			child.transform.parent = gameObject.transform;
			audioSources [i] = child.AddComponent<AudioSource> ();
			i++;
		}

		//sätt första triggtid, dvs om två sekunder
		nextEventTime = AudioSettings.dspTime;

		//sätt "aktiv uppspelning"
		running = true;
	}
	
	// Update is called once per frame
	void Update () {
		//kontrollera att uppspelning är aktiverad
		if (!running)
			return;

		//läs av ljudsystemklockan
		double time = AudioSettings.dspTime;

		//kontrollera om det är dags att förbereda triggning av nästa ljudfil[""ha en sekunds "horisont"]
		if (time + 1.0f > nextEventTime){
			// ange vilken ljudfil som ska gälla
			audioSources[flip].clip = clips[flip];

			//ange framtida trigggning av ljudfilen
			audioSources[flip].PlayScheduled(nextEventTime);
			Debug.Log ("Scheduled source" + flip + " to start at time " + nextEventTime);

			//beräkna nästa triggögonblick
			nextEventTime += 60.0f / bmp * numBeatsPerSegments;
			//nextEventTime += audioSources[flip].clip.length;
			//förbered byte av AudioSource (player) till nästa triggning
			flip = 1 - flip;
		}

	}
}
