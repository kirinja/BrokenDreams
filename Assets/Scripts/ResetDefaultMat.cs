﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDefaultMat : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    GetComponent<Renderer>().sharedMaterial.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
