using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearTimeText : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    var totalTime = GameManager.Instance.GameTime;
	    var mins = Mathf.FloorToInt(totalTime / 60);
	    var seconds = Mathf.RoundToInt(totalTime % 60);
	    GetComponent<Text>().text = "Cleared game in " + mins + " minutes and " + seconds + " seconds!";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
