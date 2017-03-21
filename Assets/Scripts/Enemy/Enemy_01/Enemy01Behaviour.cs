using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Behaviour : MonoBehaviour {

    public Vector3 vec;
    private Collider col;
    private Enemy01State state;
    public float idleTime;
    public float patrolTime;

	// Use this for initialization
	void Start () {
        col = GetComponent<Collider>();
        //state = new Idle01(this, new Vector3(0,0,0));
        state = new Patrol01(this, vec);
        Debug.Log("Patrol");
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

    public Vector3 getVec()
    {
        return vec;
    }

    public void invertVec()
    {
        vec *= -1;
    }




}
