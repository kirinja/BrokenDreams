using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase3VerticalMovement : MonoBehaviour
{
    public Transform Target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position += new Vector3(0.0f, Target.position.y, 0.0f);
	}
}
