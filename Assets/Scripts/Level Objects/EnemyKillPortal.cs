using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillPortal : MonoBehaviour
{
    public GameObject[] Enemies;

    private bool active;

	// Use this for initialization
	void Awake () {
	    transform.Find("Portal").GetComponent<HubPortal>().ShouldInitialize =
	        false;
	    transform.Find("Portal").GetComponent<HubPortal>().Hide();
    }
	
	// Update is called once per frame
	void Update () {
	    if (active) return;

	    var activate = true;

	    // HACK pretty badly done but it will do for now
	    for (int i = 0; i < Enemies.Length; ++i)
	    {
	        if (Enemies[i] && Enemies[i].GetComponent<Enemy>().Alive)
	        {
	            activate = false;
	            //break;
	        }
	    }

	    if (activate)
	    {
	        active = true;

            //if kill
	        transform.Find("Portal").gameObject.SetActive(true);
	        transform.Find("Portal").GetComponent<HubPortal>().ShouldAppear = true;
            transform.Find("Door").Find("Door Trigger").gameObject.SetActive(true);
        }
    }
}
