using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public GameObject Door;

	// Use this for initialization
	void Start ()
	{
        Door.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Movable Object"))
        {
            Debug.Log("Button activate");
            Door.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Movable Object"))
        {
            Debug.Log("Button activate");
            Door.SetActive(true);
        }
    }
}
