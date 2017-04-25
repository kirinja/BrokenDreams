using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingWall : Attackable {
    public GameObject StandingWall, FallenWall;

	// Use this for initialization
	void Start () {
        gameObject.SetActive(true);
        StandingWall.SetActive(true);
        FallenWall.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Damage(int damage = 1)
    {
        gameObject.SetActive(false);
        StandingWall.SetActive(false);
        FallenWall.SetActive(true);
    }
}
