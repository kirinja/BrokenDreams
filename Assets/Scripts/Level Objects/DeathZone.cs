﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Controller3D>().Damage(9999);
            //other.GetComponentInChildren<PlayerAudio>().PlayDeathZoneSound(); // Kinda ugly and unnecessary since we already have a death sound
        }
    }
}
