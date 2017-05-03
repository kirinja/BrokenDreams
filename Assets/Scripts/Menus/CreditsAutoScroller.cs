using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsAutoScroller : MonoBehaviour
{
    public float ScrollSpeed = 10.0f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(0, ScrollSpeed * Time.deltaTime, 0);	
	}
}
