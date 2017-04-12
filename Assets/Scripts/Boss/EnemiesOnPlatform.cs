using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesOnPlatform : MonoBehaviour
{
    public int Amount { get; set; }

    // On trigger exit doesnt fire if we destroy the enemy, since we destroy the collider before it moves out of the trigger zone
    // we should check every frame if a gameobject is in the trigger zone and if it isnt then we decrease the counter
    private List<GameObject> objects;

	// Use this for initialization
	void Start ()
	{
        objects = new List<GameObject>();
	    Amount = 0;
	}
	
	// Update is called once per frame
	void Update () {

        // every frame we check if any of the gameobjects has died, this might work since it's all references?
	    foreach (GameObject g in objects)
	    {
            // remove and count down
	        if (g != null) continue;
	        objects.Remove(g);
	        Amount--;
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
                Amount++;
            }
        }
    }
}
