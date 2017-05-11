using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsAutoScroller : MonoBehaviour
{
    public float ScrollSpeed = 10.0f;
    public GameObject EndOfCreditsObject;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var endY = Mathf.Abs(EndOfCreditsObject.transform.localPosition.y);
	    var curY = transform.localPosition.y;
        //Debug.Log(endY + "\t" + curY + "\t" + (Mathf.Abs(endY - curY))); 
	    if (Mathf.Abs((endY) - curY) <= 10.0f)
	    {
            // GOOD EMPTY BLOCK
	    }
	    else
	        transform.Translate(0, ScrollSpeed * Time.deltaTime, 0);
	}
}
