using System;
using System.Collections;
using System.Collections.Generic;
using Prime31.TransitionKit;
using UnityEngine;

public class CreditsAutoScroller : MonoBehaviour
{
    public float ScrollSpeed = 10.0f;
    public GameObject EndOfCreditsObject;

    public float WaitTime = 3.0f;
    private float _timer;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (_timer >= WaitTime)
	    {
	        var endY = Mathf.Abs(EndOfCreditsObject.transform.localPosition.y);
	        var curY = transform.localPosition.y;
	        //Debug.Log(endY + "\t" + curY + "\t" + (Mathf.Abs(endY - curY))); 
	        if (Mathf.Abs((endY) - curY) <= 10.0f)
	        {
	            // GOOD EMPTY BLOCK
                // we've reached the end of the credits
	            if (Input.anyKeyDown)
	            {
                    // transition to start if we press any key after the text has stopped scrolling
                    var gameManager = GameManager.Instance;
                    //gameManager.BeatLevel(SceneManager.GetActiveScene().name);
                    gameManager.SaveToMemory();
                    gameManager.SaveToFiles();


                    var fishEye = new FishEyeTransition()
                    {
                        nextScene = "Start",
                        duration = 5.0f,
                        size = 0.2f,
                        zoom = 100.0f,
                        colorSeparation = 0.1f
                    };
                    TransitionKit.instance.transitionWithDelegate(fishEye);
                }

	        }
	        else
	            transform.Translate(0, ScrollSpeed * Time.deltaTime, 0);
	    }
	    _timer += Time.deltaTime;
	}
}
