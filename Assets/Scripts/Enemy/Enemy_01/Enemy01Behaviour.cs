using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Behaviour : MonoBehaviour {

    public Vector2 vec;
    public Collider col;

	// Use this for initialization
	void Start () {
        col = GetComponent<Collider>();
	}
	
	// UpdateTime is called once per frame
	void Update () {
        this.transform.Translate(vec);
	}

    public void OnCollisionEnter(Collision other)
    {
        vec *= -1;
    }




}
