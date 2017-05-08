using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    public GameObject LinkedWall;
    private DestroyableWall _destroyableWall;
	// Use this for initialization
	void Start ()
	{
        _destroyableWall = GetComponent<DestroyableWall>();
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (_destroyableWall.Destroyed)
	        LinkedWall.GetComponent<Animator>().enabled = true;
	}
}
