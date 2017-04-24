using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesOnPlatform : MonoBehaviour
{
    public int Amount
    {
        get
        {
            return objects.Count;
        }
    }
    private List<GameObject> objects = new List<GameObject>();

    void Start()
    {
    }

    void Update () {

        // every frame we check if any of the gameobjects has died, this might work since it's all references?
	    foreach (GameObject g in objects)
	    {
            // remove and count down
	        if (g != null) continue;
	        objects.Remove(g);
	        break;
	    }
	}

    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // if the object isnt in the list, add it and count up the amount (amount might be unneccessary)
            if (!objects.Contains(other.gameObject))
            {
                objects.Add(other.gameObject);
            }
        }
    }

}
