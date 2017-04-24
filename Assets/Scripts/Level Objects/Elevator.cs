﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Elevator : MonoBehaviour
{
    public GameObject[] enemies;

    private bool active;
    public GameObject Feedback1;
    public GameObject Feedback2;
    public GameObject Feedback3;
    public GameObject Feedback4;
    private GameObject[] FeedbackList;

    // Use this for initialization
    void Start ()
	{
	    active = false;
	    GetComponent<Animator>().enabled = false;
        FeedbackList = new GameObject[4];
	    FeedbackList[0] = Feedback1;
	    FeedbackList[1] = Feedback2;
	    FeedbackList[2] = Feedback3;
	    FeedbackList[3] = Feedback4;

        foreach (MeshRenderer r in Feedback1.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }

        foreach (MeshRenderer r in Feedback2.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }

        foreach (MeshRenderer r in Feedback3.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }

        foreach (MeshRenderer r in Feedback4.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
        if (active) return;
        var activate = true;
	    /*foreach (var enemy in enemies)
	    {
	        if (enemy)
	        {
	            activate = false;
	            break;
	        }
	    }*/

        // HACK pretty badly done but it will do for now
	    for (int i = 0; i < enemies.Length; ++i)
	    {
	        if (enemies[i])
	        {
	            activate = false;
                FeedbackColor(i, Color.blue);
	            //break;
	        }
	        else
	        {
	            FeedbackColor(i, Color.green);
	        }
	    }

	    if (activate)
	    {
	        active = true;
            GetComponent<Animator>().enabled = true;
            Debug.Log("Activating elevator");
	    }
	}

    private void FeedbackColor(int i, Color color)
    {
        foreach (MeshRenderer r in FeedbackList[i].GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = color;
        }
    }

    void OnEnable()
    {
        foreach (MeshRenderer r in Feedback1.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }

        foreach (MeshRenderer r in Feedback2.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }

        foreach (MeshRenderer r in Feedback3.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }

        foreach (MeshRenderer r in Feedback4.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }
    }

    void OnDisable()
    {
        if (Feedback1)
        {
            foreach (MeshRenderer r in Feedback1.GetComponentsInChildren<MeshRenderer>())
            {
                r.sharedMaterial.color = Color.blue;
            }
        }

        if (Feedback2)
        {
            foreach (MeshRenderer r in Feedback2.GetComponentsInChildren<MeshRenderer>())
            {
                r.sharedMaterial.color = Color.blue;
            }
        }

        if (Feedback3)
        {
            foreach (MeshRenderer r in Feedback3.GetComponentsInChildren<MeshRenderer>())
            {
                r.sharedMaterial.color = Color.blue;
            }
        }

        if (Feedback4)
        {
            foreach (MeshRenderer r in Feedback4.GetComponentsInChildren<MeshRenderer>())
            {
                r.sharedMaterial.color = Color.blue;
            }
        }
    }

    void OnApplicationQuit()
    {
        foreach (MeshRenderer r in Feedback1.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }

        foreach (MeshRenderer r in Feedback2.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }

        foreach (MeshRenderer r in Feedback3.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }

        foreach (MeshRenderer r in Feedback4.GetComponentsInChildren<MeshRenderer>())
        {
            r.sharedMaterial.color = Color.blue;
        }
    }
}
