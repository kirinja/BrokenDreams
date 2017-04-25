using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool activated;

	// Use this for initialization
	void Start ()
	{
	    activated = false;
        transform.Find("Checkpoint_circle").GetComponent<ParticleSystem>().Stop();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            other.GetComponent<Controller3D>().SetSpawn(transform.position);
            transform.Find("Checkpoint_circle").GetComponent<ParticleSystem>().Play();
            var gm = GameManager.Get();
            if (!gm) return;
            gm.SaveToMemory();
            gm.SaveToFiles();
            gm.UseCheckPoint = true;
        }
    }
}
