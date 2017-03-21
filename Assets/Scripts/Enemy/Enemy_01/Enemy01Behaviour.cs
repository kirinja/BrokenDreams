using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Behaviour : MonoBehaviour {

    public Vector2 vec;
    public Collider col;
    private Enemy01State state;

	// Use this for initialization
	void Start () {
        col = GetComponent<Collider>();
        //state = new Idle01(this, new Vector3(0,0,0));
        state = new Patrol01(this, vec);
	}
	
	// UpdateTime is called once per frame
	void Update () {
        state.Update();
	}

    public void OnCollisionEnter(Collision other)
    {
        state.Collision();
    }

    public void changeState(Enemy01State state)
    {
        this.state = state;
    }




}
