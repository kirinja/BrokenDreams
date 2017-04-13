using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public GameObject[] Doors;

    private int count;

	// Use this for initialization
	void Start ()
	{
	    foreach (var door in Doors)
	    {
	        door.SetActive(true);
        }
        
	    count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Movable Object") || other.CompareTag("Player"))
        {
            if (++count > 0)
            {
                foreach (var door in Doors)
                {
                    door.SetActive(false);
                }
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Movable Object") || other.CompareTag("Player"))
        {
            if (--count <= 0)
            {
                foreach (var door in Doors)
                {
                    door.SetActive(true);
                }
            }
        }
    }
}
