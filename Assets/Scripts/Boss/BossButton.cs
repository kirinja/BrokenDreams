using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossButton : MonoBehaviour
{
    public bool Active;
    public GameObject Feedback;
	// Use this for initialization
	void Start ()
	{
	    if (!Feedback)
	        throw new ArgumentNullException("Feedback cant be null, please assign a feedback");
        foreach (MeshRenderer r in Feedback.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }
    }

    void OnEnable()
    {
        foreach (MeshRenderer r in Feedback.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }
    }

    void OnDisable()
    {
        if (!Feedback) return;
        foreach (MeshRenderer r in Feedback.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }
    }

    void OnApplicationQuit()
    {
        foreach (MeshRenderer r in Feedback.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player") || other.CompareTag("Movable Object"))
    //    {
    //        Active = true;

    //        foreach (MeshRenderer r in Feedback.GetComponentsInChildren<MeshRenderer>())
    //        {
    //            r.sharedMaterial.color = Color.green;
    //        }
    //    }
    //}

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Movable Object"))
        {
            Active = false;

            foreach (MeshRenderer r in Feedback.GetComponentsInChildren<MeshRenderer>())
            {
                r.sharedMaterial.color = Color.blue;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Movable Object"))
        {
            Active = true;

            foreach (MeshRenderer r in Feedback.GetComponentsInChildren<MeshRenderer>())
            {
                r.sharedMaterial.color = Color.green;
            }
        }
    }
}
