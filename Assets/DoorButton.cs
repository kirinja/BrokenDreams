using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public GameObject Door;

    private bool used;

	// Use this for initialization
	void Start ()
	{
	    used = false;
        Door.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Movable Object") && !used)
        {
            Debug.Log("Button activate");
            DestroyObject(Door);
            used = true;
        }
    }
}
