using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Elevator : MonoBehaviour
{
    public float ElevatorWaitTime = 2.0f;
    public GameObject[] buttons;

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

        // HACK pretty badly done but it will do for now
	    for (int i = 0; i < buttons.Length; ++i)
	    {
	        if (buttons[i] && !buttons[i].GetComponent<ElevatorButton>().Active)
	            activate = false;
	    }

	    if (activate)
	    {
            // TODO add a slight delay possibly
	        active = true;
	        StartCoroutine("StartElevator");
            
	    }
	}

    private IEnumerator StartElevator()
    {
        yield return new WaitForSeconds(ElevatorWaitTime);
        GetComponent<Animator>().enabled = true;
    }
    
}
