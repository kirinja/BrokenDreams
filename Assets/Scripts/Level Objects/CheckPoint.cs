﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool activated;

	// Use this for initialization
	void Start ()
	{
	    activated = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            Debug.Log("CHECKPOINT");
            other.GetComponent<Controller3D>().SetSpawn();
            GameObject.Find("GameManager").GetComponent<GameManager>().SaveToMemory();
            GameObject.Find("GameManager").GetComponent<GameManager>().SaveToFiles();
        }
    }
}
