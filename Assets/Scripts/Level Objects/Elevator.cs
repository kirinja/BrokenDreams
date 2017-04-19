using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Elevator : MonoBehaviour
{
    public GameObject[] enemies;

    private bool active;

	// Use this for initialization
	void Start ()
	{
	    active = false;
	    GetComponent<Animator>().enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (active) return;
        var activate = true;
	    foreach (var enemy in enemies)
	    {
	        if (enemy)
	        {
	            activate = false;
	            break;
	        }
	    }
	    if (activate)
	    {
	        active = true;
            GetComponent<Animator>().enabled = true;
            Debug.Log("Activating elevator");
	    }
	}
}
