using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CollisionAnimatorStopper : MonoBehaviour
{
    public LayerMask CollisionMask;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Movable Object"))
        {
            GetComponent<Animator>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Movable Object"))
        {
            GetComponent<Animator>().enabled = true;
        }
    }
}
