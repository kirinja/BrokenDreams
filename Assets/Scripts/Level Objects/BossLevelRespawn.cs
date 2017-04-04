using System.Collections;
using System.Collections.Generic;
using Prime31.TransitionKit;
using UnityEngine;

public class BossLevelRespawn : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    var g = GameObject.FindGameObjectWithTag("Player");
        g.GetComponent<Controller3D>().SetSpawn(transform.position);;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var fishEye = new FishEyeTransition()
            {
                nextScene = "Boss_01",
                duration = 0.5f,
                size = 0.4f,
                zoom = 100.0f,
                colorSeparation = 0.15f
            };
            TransitionKit.instance.transitionWithDelegate(fishEye);
        }
    }
}
