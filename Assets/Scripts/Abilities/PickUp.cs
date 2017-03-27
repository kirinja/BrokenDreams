using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    var g = GameObject.FindGameObjectWithTag("Player");
	    foreach (Ability a in g.GetComponent<PlayerAttributes>().Abilities)
	    {
	        if (a.name.Equals(this.name))
                Destroy(gameObject);
	    }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            var abb = Resources.Load<Ability>("Abilities\\" + this.name); // Unsafe
            other.gameObject.GetComponent<PlayerAttributes>().Abilities.Add(abb);
            Destroy(gameObject, 0.0f);
            other.GetComponent<Controller3D>().RefreshMaterial();
        }
    }
}
